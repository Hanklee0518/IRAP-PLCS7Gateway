using IRAP.MESGateway.Tools.Entities;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRAP.MESGateway.Tools.Utils
{
    internal class ServiceHelper
    {
        private static ServiceHelper _instance = null;
        public static ServiceHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceHelper();
                }
                return _instance;
            }
        }

        private ServiceHelper()
        {
            InitDCSGatewayServices();
        }

        public string ServiceCommName
        {
            get { return "IRAP DCSGateway for S7PLC - "; }
        }

        public ServiceEntityCollection Services { get; } =
            new ServiceEntityCollection();

        private void InitDCSGatewayServices()
        {
            Services.Clear();

            ServiceController[] controllers = ServiceController.GetServices();
            foreach (ServiceController controller in controllers)
            {
                if (controller.ServiceName.ToUpper().Contains(ServiceCommName.ToUpper()))
                {
                    Services.Add(
                        new ServiceEntity()
                        {
                            ServiceName = controller.ServiceName,
                            Status = controller.Status,
                            Service = controller,
                        });
                }
            }
        }

        private void RunProcess(string cmd)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = cmd,
                RedirectStandardInput = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                UseShellExecute = true,
                Verb = "RunAs",
            };

            Process process = new Process()
            {
                StartInfo = startInfo,
            };
            process.Start();
            process.WaitForExit();
            process.Close();
        }

        public string GetServiceName(string DeviceName)
        {
            return $"{ServiceCommName}{DeviceName}";
        }

        /// <summary>
        /// 安装Windows服务
        /// </summary>
        /// <param name="servInstallPath">Windows服务程序文件路径</param>
        public void InstallDCSGatewayService(string servInstallPath)
        {
            try
            {
                #region 写入服务安装脚本文件
                string directory = Path.GetDirectoryName(servInstallPath);
                string scriptPath = Path.Combine(directory, "Install.cmd");
                FileStream fs =
                    new FileStream(
                        scriptPath,
                        FileMode.Create);
                byte[] data =
                    Encoding.Default.GetBytes(
                        $@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe {servInstallPath}");
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Close();
                #endregion

                if (File.Exists(scriptPath))
                {
                    RunProcess(scriptPath);
                }

                #region 删除服务安装脚本文件
                File.Delete(scriptPath);
                #endregion
            }
            catch (Exception error)
            {
                throw error;
            }
        }

        /// <summary>
        /// 卸载Windows服务
        /// </summary>
        /// <param name="servInstallPath">Windows服务程序文件路径</param>
        public void UninstallDCSGatewayService(string servInstallPath)
        {
            try
            {
                #region 写入服务卸载脚本文件
                string directory = Path.GetDirectoryName(servInstallPath);
                string scriptPath = Path.Combine(directory, "Uninstall.cmd");
                FileStream fs =
                    new FileStream(
                        scriptPath,
                        FileMode.Create);
                byte[] data =
                    Encoding.Default.GetBytes(
                        $@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u {servInstallPath}");
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Close();
                #endregion

                if (File.Exists(scriptPath))
                {
                    RunProcess(scriptPath);
                }

                #region 删除服务卸载脚本文件
                File.Delete(scriptPath);
                #endregion
            }
            catch (Exception error)
            {
                throw error;
            }
        }

        /// <summary>
        /// 检查服务是否已经安装
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns>true: 服务存在; false: 服务不存在</returns>
        public bool ServiceExisted(string serviceName)
        {
            ServiceController[] controllers = ServiceController.GetServices();
            foreach (ServiceController controller in controllers)
            {
                if (controller.ServiceName == serviceName)
                {
                    return true;
                }
            }

            if (Services[serviceName] != null)
            {
                Services.Remove(serviceName);
            }
            return false;
        }

        /// <summary>
        /// Windows服务是否已经启动
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns>true: 已启动; false: 未启动(或服务不存在)</returns>
        public bool ServiceStarted(string serviceName)
        {
            ServiceEntity serviceEntity = Services[serviceName];
            if (serviceEntity != null)
            {
                serviceEntity.Service.Refresh();
                serviceEntity.Status = serviceEntity.Service.Status;
                return serviceEntity.Status == ServiceControllerStatus.Stopped;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 启动Windows服务
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns>true: 已启动; false: 启动状态未知（超时）</returns>
        public bool StartService(string serviceName)
        {
            if (ServiceExisted(serviceName))
            {
                ServiceEntity serviceEntity = Services[serviceName];
                if (serviceEntity.Status != ServiceControllerStatus.Running)
                {
                    if (serviceEntity.Status == ServiceControllerStatus.Stopped)
                    {
                        serviceEntity.Service.Start();
                    }
                    for (int i = 0; i < 60; i++)
                    {
                        Thread.Sleep(1000);
                        serviceEntity.Service.Refresh();
                        serviceEntity.Status = serviceEntity.Service.Status;
                        if (serviceEntity.Status == ServiceControllerStatus.Running)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 停止Windows服务
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns>true: 已停止; false: 状态未知</returns>
        public bool StopService(string serviceName)
        {
            if (ServiceExisted(serviceName))
            {
                ServiceEntity serviceEntity = Services[serviceName];
                if (serviceEntity.Status == ServiceControllerStatus.Running)
                {
                    serviceEntity.Service.Stop();
                    for (int i = 0; i < 60; i++)
                    {
                        Thread.Sleep(1000);
                        serviceEntity.Service.Refresh();
                        serviceEntity.Status = serviceEntity.Service.Status;
                        if (serviceEntity.Status == ServiceControllerStatus.Stopped)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public List<ServiceEntity> GetDCSGatewayServices()
        {
            Services.Clear();
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController service in services)
            {
                if (service.ServiceName.ToUpper().Contains(ServiceCommName.ToUpper()))
                {
                    Services.Add(
                        new ServiceEntity()
                        {
                            ServiceName = service.ServiceName,
                            Status = service.Status,
                            Service = service,
                        });
                }
            }

            return Services.ExtractToList();
        }

        /// <summary>
        /// 获取指定服务名的运行目录
        /// </summary>
        /// <param name="servName">服务名</param>
        /// <returns></returns>
        public string GetServiceHomePath(string servName)
        {
            if (servName == "")
            {
                return "";
            }
            else
            {
                if (ServiceExisted(servName))
                {
                    string key = $@"SYSTEM\CurrentControlSet\Services\{servName}";
                    return
                        Registry
                            .LocalMachine
                            .OpenSubKey(key)
                            .GetValue("ImagePath")
                            .ToString()
                            .Replace("\"", string.Empty);
                }
                else
                {
                    return "";
                }
            }
        }
    }

    public class ServiceEntity
    {
        public ServiceEntity()
        {
        }

        public string ServiceName { get; set; }
        public ServiceControllerStatus Status { get; set; }
        public ServiceController Service { get; set; }
        public string ServiceHomePath
        {
            get
            {
                if (ServiceName == "" || Service == null)
                {
                    return "";
                }
                else
                {
                    string key = $@"SYSTEM\CurrentControlSet\Services\{ServiceName}";
                    return
                        Registry
                            .LocalMachine
                            .OpenSubKey(key)
                            .GetValue("ImagePath")
                            .ToString()
                            .Replace("\"", string.Empty);
                }
            }
        }

        public static int CompareByServiceName(ServiceEntity a, ServiceEntity b)
        {
            return string.Compare(a.ServiceName, b.ServiceName);
        }
    }

    internal class ServiceEntityCollection : IEnumerable
    {
        private Dictionary<string, ServiceEntity> services =
            new Dictionary<string, ServiceEntity>();

        public ServiceEntityCollection() { }

        public ServiceEntity this[int index]
        {
            get
            {
                if (index >= 0 && index < services.Count)
                {
                    return services.ElementAt(index).Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public ServiceEntity this[string key]
        {
            get
            {
                if (services.TryGetValue(key, out ServiceEntity rlt))
                {
                    return rlt;
                }
                else
                {
                    return null;
                }
            }
        }
        public int Count
        {
            get { return services.Count; }
        }

        public void Add(ServiceEntity entity)
        {
            if (services.ContainsKey(entity.ServiceName))
            {
                return;
            }
            else
            {
                services.Add(entity.ServiceName, entity);
            }
        }

        public void Remove(ServiceEntity entity)
        {
            Remove(entity.ServiceName);
        }
        public void Remove(string key)
        {
            if (services.ContainsKey(key))
            {
                services.Remove(key);
            }
        }

        public void Clear()
        {
            services.Clear();
        }

        public List<ServiceEntity> ExtractToList()
        {
            return services.Values.ToList();
        }

        public IEnumerator GetEnumerator()
        {
            foreach (ServiceEntity entity in services.Values)
            {
                yield return entity;
            }
        }
    }

    internal class DCSGatewayServiceController
    {
        private DeviceEntity device = null;

        public DCSGatewayServiceController(DeviceEntity device)
        {
            this.device = device ?? throw new Exception("DeviceEntity对象不能是 null");

            ResetServName();
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServName { get; private set; } = "";
        /// <summary>
        /// 服务文件名
        /// </summary>
        public string ServFilePath
        {
            get
            {
                if (!CanDeploy)
                {
                    return ServiceHelper.Instance.GetServiceHomePath(ServName);
                }
                else
                {
                    return
                        ParamHelper.Instance.GenerateSerivcePath(
                            device.Parent.Name,
                            device.Name);
                }
            }
        }
        /// <summary>
        /// 能否部署当前服务
        /// </summary>
        public bool CanDeploy
        {
            get =>
                File.Exists(ParamHelper.Instance.ServPackagePath) &&
                !ServiceHelper.Instance.ServiceExisted(ServName);
        }

        /// <summary>
        /// 能否更新配置数据
        /// </summary>
        /// <returns>true: 能; false: 不能</returns>
        public bool CanUpdateParams()
        {
            return ServiceHelper.Instance.ServiceExisted(ServName);
        }

        /// <summary>
        /// 服务程序是否需要升级
        /// </summary>
        /// <returns>true: 需要升级; false: 不需要升级</returns>
        public bool NeedUpgradeServiceExecute()
        {
            if (CanDeploy)
            {
                return false;
            }
            else
            {
                FileVersionInfo fileVersion;
                try
                {
                    fileVersion = FileVersionInfo.GetVersionInfo(ServFilePath);
                }
                catch { return true; }

                Version version =
                    new Version(
                        fileVersion.FileMajorPart,
                        fileVersion.FileMinorPart,
                        fileVersion.FileBuildPart,
                        fileVersion.FilePrivatePart);

                return
                    version.CompareTo(ParamHelper.Instance.BaseServiceVersion) < 0;
            }
        }

        public void ResetServName()
        {
            ServName = ServiceHelper.Instance.GetServiceName(device.Name);
        }

        /// <summary>
        /// 复制DCSGateway服务所需的程序文件到目标文件夹
        /// </summary>
        public void CopyDCSGatewayServiceFile()
        {
            string dstPath = Path.GetDirectoryName(ServFilePath);
            string srcFile = "";
            string dstFile = "";
            try
            {
                string[] files = Directory.GetFiles(ParamHelper.Instance.ServPackageDirectory);
                for (int i = 0; i < files.Length; i++)
                {
                    srcFile = files[i];
                    dstFile = $"{dstPath}\\{Path.GetFileName(srcFile)}";
                    File.Copy(srcFile, dstFile, true);
                }
            }
            catch (Exception error)
            {
                throw new Exception(
                    $"复制文件[{srcFile}]到[{dstFile}]时出错: {error.Message}");
            }


            #region 更改服务程序配置文件中的部分特殊配置项
            Configuration config = ConfigurationManager.OpenExeConfiguration(ServFilePath);
            foreach (string key in config.AppSettings.Settings.AllKeys)
            {
                if (key == "CommunityID")
                {
                    config.AppSettings.Settings.Remove("CommunityID");
                    continue;
                }
                if (key == "WebAPIUrl")
                {
                    config.AppSettings.Settings.Remove("WebAPIUrl");
                    continue;
                }
                if (key == "DeviceName")
                {
                    config.AppSettings.Settings.Remove("DeviceName");
                    continue;
                }
                if (key == "MongoDBConnectionString")
                {
                    config.AppSettings.Settings.Remove("MongoDBConnectionString");
                    continue;
                }
            }
            config.AppSettings.Settings.Add("CommunityID", ParamHelper.Instance.CommunityID.ToString());
            config.AppSettings.Settings.Add("WebAPIUrl", ParamHelper.Instance.WebAPIUrl);
            config.AppSettings.Settings.Add("DeviceName", device.Name);
            config.AppSettings.Settings.Add("MongoDBConnectionString", ParamHelper.Instance.ConnectionStringWithMongoDB);
            config.Save();
            #endregion
        }

        public void UpdateDeviceTagsParam()
        {
            device.ExportToXml(
                $"{Path.GetDirectoryName(ServFilePath)}\\" +
                $"{device.Name}.xml");
        }

        /// <summary>
        /// 部署DCSGateway服务
        /// </summary>
        public void Deploy()
        {
            #region 创建DCSGateway服务目标文件夹
            string path = Path.GetDirectoryName(ServFilePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            #endregion

            #region 复制DCSGateway服务所需的程序文件到目标文件夹
            CopyDCSGatewayServiceFile();
            #endregion

            #region 导出设备配置文件到目标文件夹
            UpdateDeviceTagsParam();
            #endregion

            #region 安装注册Windows服务
            ServiceHelper.Instance.InstallDCSGatewayService(ServFilePath);
            #endregion

            #region 将新安装注册的服务加入当面服务列表中
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController service in services)
            {
                if (service.ServiceName == ServName)
                {
                    ServiceHelper.Instance.Services.Add(
                        new ServiceEntity()
                        {
                            ServiceName = service.ServiceName,
                            Status = service.Status,
                            Service = service,
                        });
                    break;
                }
            }
            #endregion
        }

        /// <summary>
        /// 卸载DCSGateway服务
        /// </summary>
        public void Unregist()
        {
            ServiceHelper.Instance.UninstallDCSGatewayService(
                device.Service.ServFilePath);
            ServiceHelper.Instance.Services.Remove(
                ServiceHelper.Instance.Services[ServName]);
        }
    }
}
