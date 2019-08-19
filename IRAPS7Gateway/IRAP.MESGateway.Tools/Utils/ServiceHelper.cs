using IRAP.MESGateway.Tools.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Install;
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

        private readonly string serviceCommName = "IRAP DCSGateway for S7PLC";

        public ServiceEntityCollection Services { get; } =
            new ServiceEntityCollection();

        private void InitDCSGatewayServices()
        {
            Services.Clear();

            ServiceController[] controllers = ServiceController.GetServices();
            foreach (ServiceController controller in controllers)
            {
                if (controller.ServiceName.ToUpper().Contains(serviceCommName.ToUpper()))
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

        public string GetServiceName(string DeviceName)
        {
            return $"{serviceCommName} - {DeviceName}";
        }

        /// <summary>
        /// 安装Windows服务
        /// </summary>
        /// <param name="serviceInstallPath">Windows服务程序文件路径</param>
        public void InstallDCSGatewayService(
            string serviceInstallPath)
        {
            IDictionary dictionary = new Hashtable();
            try
            {
                using (AssemblyInstaller assemblyInstaller = new AssemblyInstaller
                {
                    Path = serviceInstallPath,
                    UseNewContext = true
                })
                {
                    assemblyInstaller.Install(dictionary);
                    assemblyInstaller.Commit(dictionary);
                    assemblyInstaller.Dispose();
                }
            }
            catch (Exception error)
            {
                throw error;
            }
        }

        /// <summary>
        /// 卸载Windows服务
        /// </summary>
        /// <param name="serviceIntallPath">Windows服务程序文件路径</param>
        public void UninstallDCSGatewayService(string serviceIntallPath)
        {
            IDictionary dictionary = new Hashtable();
            try
            {
                using (AssemblyInstaller assemblyInstaller = new AssemblyInstaller
                {
                    UseNewContext = true,
                    Path = serviceIntallPath,
                })
                {
                    assemblyInstaller.Uninstall(dictionary);
                    assemblyInstaller.Dispose();
                }
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
                    if (serviceEntity.Status == ServiceControllerStatus.StartPending)
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
    }

    internal class ServiceEntity
    {
        public string ServiceName { get; set; }
        public ServiceControllerStatus Status { get; set; }
        public ServiceController Service { get; set; }
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
        private string servFilePath = "";
        private DeviceEntity device = null;
        private readonly string servPackagePath =
            $"{Application.StartupPath}\\BaseService";

        public DCSGatewayServiceController(DeviceEntity device)
        {
            this.device = device ?? throw new Exception("DeviceEntity对象不能是 null");

            ResetServName();
        }

        public string ServName { get; private set; } = "";
        public string ServFilePath { get { return servFilePath; } }

        /// <summary>
        /// 能否部署当前服务
        /// </summary>
        /// <returns>true: 能; false: 不能</returns>
        public bool CanDeploy()
        {
            string servBaseExecuteFilePath =
                $"{servPackagePath}\\{ParamHelper.Instance.ServiceExecuteName}";
            return
                File.Exists(servBaseExecuteFilePath) &&
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
            throw new NotImplementedException();
        }

        public void ResetServName()
        {
            ServName = ServiceHelper.Instance.GetServiceName(device.Name);
            servFilePath =
                ParamHelper.Instance.GenerateSerivcePath(
                    device.Parent.Name,
                    device.Name);
        }

        /// <summary>
        /// 部署DCSGateway服务
        /// </summary>
        public void Deploy()
        {
            #region 创建DCSGateway服务目标文件夹
            string path = Path.GetDirectoryName(servFilePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            #endregion

            #region 复制DCSGateway服务所需的程序文件到目标文件夹
            string srcFile = "";
            string dstFile = "";
            try
            {
                string[] files = Directory.GetFiles(servPackagePath);
                for (int i = 0; i < files.Length; i++)
                {
                    srcFile = files[i];
                    dstFile = $"{path}\\{Path.GetFileName(srcFile)}";
                    File.Copy(srcFile, dstFile, true);
                }
            }
            catch (Exception error)
            {
                throw new Exception($"复制文件[{srcFile}]到[{dstFile}]时出错: {error.Message}");
            }
            #endregion

            #region 导出设备配置文件到目标文件夹
            device.ExportToXml($"{path}\\{device.Name}.xml");
            #endregion

            #region 更改服务程序配置文件中的部分特殊配置项
            Configuration config = ConfigurationManager.OpenExeConfiguration(servFilePath);
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
            }
            config.AppSettings.Settings.Add("CommunityID", ParamHelper.Instance.CommunityID.ToString());
            config.AppSettings.Settings.Add("WebAPIUrl", ParamHelper.Instance.WebAPIUrl);
            config.AppSettings.Settings.Add("DeviceName", device.Name);
            config.Save();
            #endregion

            #region 安装注册Windows服务
            ServiceHelper.Instance.InstallDCSGatewayService(servFilePath);
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
