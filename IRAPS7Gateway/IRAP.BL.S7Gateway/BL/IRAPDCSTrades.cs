using IRAP.BL.S7Gateway.Entities;
using IRAP.BL.S7Gateway.Utils;
using IRAP.BL.S7Gateway.WebAPIClient.Exchange.DCS;
using Logrila.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace IRAP.BL.S7Gateway
{

    /// <summary>
    /// IRAP DCS 交易接口
    /// </summary>
    public interface IIRAPDCSTrade
    {
        /// <summary>
        /// 交易执行
        /// </summary>
        /// <returns>待回写到PLC的标记列表</returns>
        List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag);
    }

    /// <summary>
    /// 交易对象工厂类
    /// </summary>
    public class IRAPDCSTraderCreator : IRAPBaseObject
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public IRAPDCSTraderCreator()
        {
            _log = Logger.Get<IRAPDCSTraderCreator>();
        }

        /// <summary>
        /// 创建一个实现IIRAPDCSTrade接口的对象
        /// </summary>
        /// <returns>已实例化的IIRAPDCSTrade接口</returns>
        public static IIRAPDCSTrade CreateInstance(string tradeName, DCSGatewayLogEntity log)
        {
            object[] parameters = new object[] { log };
            string className = $"IRAP.BL.S7Gateway.IRAPDCSTrade{tradeName}";
            IIRAPDCSTrade trade =
                (IIRAPDCSTrade)Assembly
                    .Load("IRAP.BL.S7Gateway")
                    .CreateInstance(
                        className,
                        true,
                        BindingFlags.Default,
                        null,
                        parameters,
                        null,
                        null);
            return trade;
        }
    }

    /// <summary>
    /// DCS交易父类
    /// </summary>
    public abstract class IRAPDCSTrade : IRAPBaseObject
    {
        internal DCSGatewayLogEntity logEntity = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        public IRAPDCSTrade(DCSGatewayLogEntity log)
        {
            _log = Logger.Get(GetType());
            logEntity = log;
        }

        /// <summary>
        /// 调用StartDCSInvoking交易
        /// </summary>
        /// <param name="device">西门子设备对象</param>
        /// <param name="tag">西门子PLC标签对象</param>
        /// <returns>true-交易成功；false-交易失败</returns>
        protected bool CallStartDCSInvoking(SiemensDevice device, SiemensTag tag)
        {
            StartDCSInvoking startDCSInvoking =
                new StartDCSInvoking(
                    GlobalParams.Instance.WebAPI.URL,
                    GlobalParams.Instance.WebAPI.ContentType,
                    GlobalParams.Instance.WebAPI.ClientID,
                    logEntity)
                {
                    Request =
                        new StartDCSInvokingRequest()
                        {
                            CommunityID = GlobalParams.Instance.CommunityID,
                            T133LeafID = device.T133LeafID,
                        }
                };

            try
            {
                if (startDCSInvoking.Do())
                {
                    if (startDCSInvoking.Error.ErrCode == 0)
                    {
                        _log.Debug(
                            $"[{id.ToString()}|({startDCSInvoking.Error.ErrCode})" +
                            $"{startDCSInvoking.Error.ErrText}");
                        return true;
                    }
                    else
                    {
                        _log.Error(
                            $"[{id.ToString()}|({startDCSInvoking.Error.ErrCode})" +
                            $"{startDCSInvoking.Error.ErrText}");
                        return false;
                    }
                }
                else
                {
                    _log.Error(
                        $"[{id.ToString()}|({startDCSInvoking.Error.ErrCode})" +
                        $"{startDCSInvoking.Error.ErrText}");
                    return false;
                }
            }
            catch (Exception error)
            {
                _log.Error(error.Message, error);
                logEntity.Errors.Add(error);
                return false;
            }
        }

        /// <summary>
        /// 获取指定标记对象在Device的标记字典中的关键字
        /// </summary>
        /// <param name="tag">标记对象</param>
        /// <returns>关键字</returns>
        protected string GetTagKeyValue(SiemensTag tag)
        {
            string key = "";
            CustomGroup parent = tag.Parent;
            if (parent is SiemensSubTagGroup)
            {
                key =
                    $"{(parent as SiemensSubTagGroup).Parent.Name}" +
                    $".{(parent as SiemensSubTagGroup).Prefix}";
            }
            else
            {
                key = (parent as SiemensTagGroup).Name;
            }
            return key;
        }

        /// <summary>
        /// 从标记组中查找指定标记名的标记，并返回该标记的bool值
        /// </summary>
        /// <param name="tags">标记组</param>
        /// <param name="tagName">标记名称</param>
        /// <returns>标记的bool值</returns>
        protected bool GetBoolValue(SiemensTagCollection tags, string tagName)
        {
            var tag = tags[tagName] as SiemensTag;
            if (tag is SiemensBoolOfTag)
            {
                return (tag as SiemensBoolOfTag).Value;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 从标记组中查找指定标记名的标记，从PLC中实时读取并返回该标记的字符串值
        /// </summary>
        /// <param name="device">西门子设备</param>
        /// <param name="tags">标记组</param>
        /// <param name="tagName">标记名称</param>
        /// <returns>标记的字符串值</returns>
        protected string ReadStringValue(
            SiemensDevice device,
            SiemensTagCollection tags,
            string tagName)
        {
            var tag = tags[tagName];
            if (tag is SiemensArrayCharOfTag ltag)
            {
                var stringTag =
                    device.ReadTagValue(ltag) as SiemensArrayCharOfTag;
                return stringTag.Value.Trim();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 从标记组中查找指定标记名的标记，从PLC中实时读取并返回该标记的byte值
        /// </summary>
        /// <param name="device">西门子设备</param>
        /// <param name="tags">标记组</param>
        /// <param name="tagName">标记名称</param>
        /// <returns>标记的byte值</returns>
        protected byte ReadByteValue(
            SiemensDevice device,
            SiemensTagCollection tags,
            string tagName)
        {
            var tag = tags[tagName];
            if (tag is SiemensByteOfTag)
            {
                var intTag =
                    device.ReadTagValue(
                        tag as SiemensByteOfTag) as SiemensByteOfTag;
                return intTag.Value;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 从标记组中查找指定标记名的标记，从PLC中实时读取并返回该标记的short值
        /// </summary>
        /// <param name="device">西门子设备</param>
        /// <param name="tags">标记组</param>
        /// <param name="tagName">标记名称</param>
        /// <returns>标记的short值</returns>
        protected short ReadIntValue(
            SiemensDevice device,
            SiemensTagCollection tags,
            string tagName)
        {
            var tag = tags[tagName];
            if (tag is SiemensIntOfTag)
            {
                var intTag =
                    device.ReadTagValue(
                        tag as SiemensIntOfTag) as SiemensIntOfTag;
                return intTag.Value;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 从标记组中查找指定标记名的标记，从PLC中实时读取并返回该标记的uint值
        /// </summary>
        /// <param name="device">西门子设备</param>
        /// <param name="tags">标记组</param>
        /// <param name="tagName">标记名称</param>
        /// <returns>标记的uint值</returns>
        protected uint ReadDWordValue(
            SiemensDevice device,
            SiemensTagCollection tags,
            string tagName)
        {
            var tag = tags[tagName];
            if (tag is SiemensDWordOfTag)
            {
                var intTag =
                    device.ReadTagValue(
                        tag as SiemensDWordOfTag) as SiemensDWordOfTag;
                return intTag.Value;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 从标记组中查找指定标记名的标记，从PLC中实时读取并返回该标记的ushort值
        /// </summary>
        /// <param name="device">西门子设备</param>
        /// <param name="tags">标记组</param>
        /// <param name="tagName">标记名称</param>
        /// <returns>标记的ushort值</returns>
        protected ushort ReadWordValue(
            SiemensDevice device,
            SiemensTagCollection tags,
            string tagName)
        {
            var tag = tags[tagName];
            if (tag is SiemensWordOfTag)
            {
                var intTag =
                    device.ReadTagValue(
                        tag as SiemensWordOfTag) as SiemensWordOfTag;
                return intTag.Value;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 查找指定名称的Tag，将Tag值写入找到的Tag中，并加入待回写PLC的Tag列表中
        /// </summary>
        /// <param name="tags">待回写PLC的Tag列表</param>
        /// <param name="group">待查找Tag所在的容器</param>
        /// <param name="tagName">待查找的Tag名称</param>
        /// <param name="tagValue">Tag值</param>
        protected void WriteTagValueBack(
            List<SiemensTag> tags,
            SiemensTagGroup group,
            string tagName,
            object tagValue)
        {
            if (tags == null || group == null)
            {
                return;
            }

            if (group.Tags[tagName] is SiemensTag writeTag)
            {
                writeTag.Value = tagValue;
                tags.Add(writeTag);
                if (writeTag is SiemensArrayCharOfTag)
                {
                    if (group.Tags[$"{tagName}_Length"] is SiemensTag lengthTag)
                    {
                        lengthTag.Value = ((string)tagValue).Length;
                        tags.Add(lengthTag);
                    }
                }
            }
        }

        /// <summary>
        /// 查找指定名称的Tag，将Tag值写入找到的Tag中，并加入待回写PLC的Tag列表中
        /// </summary>
        /// <param name="tags">待回写PLC的Tag列表</param>
        /// <param name="group">待查找Tag所在的容器</param>
        /// <param name="tagName">待查找的Tag名称</param>
        /// <param name="tagValue">Tag值</param>
        protected void WriteTagValueBack(
            List<SiemensTag> tags,
            SiemensSubTagGroup group,
            string tagName,
            object tagValue)
        {
            if (tags == null || group == null)
            {
                return;
            }

            if (group.Tags[tagName] is SiemensTag writeTag)
            {
                writeTag.Value = tagValue;
                tags.Add(writeTag);
                if (writeTag is SiemensArrayCharOfTag)
                {
                    if (group.Tags[$"{tagName}_Length"] is SiemensTag lengthTag)
                    {
                        lengthTag.Value = ((string)tagValue).Length;
                        tags.Add(lengthTag);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 同步设备状态交易
    /// </summary>
    public class IRAPDCSTradeGetOPCStatus : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeGetOPCStatus(DCSGatewayLogEntity log) : base(log)
        {
        }

        /// <summary>
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns></returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();

            if (device != null)
            {
                _log.Info("同步设备状态处理");
                logEntity.DeviceName = device.Name;
                logEntity.ActionName = "同步设备状态";

                if (device.Groups["COMM"] is SiemensTagGroup comm)
                {
                    GetOPCStatus getOPCStatus =
                        new GetOPCStatus(
                            GlobalParams.Instance.WebAPI.URL,
                            GlobalParams.Instance.WebAPI.ContentType,
                            GlobalParams.Instance.WebAPI.ClientID,
                                logEntity)
                        {
                            Request = new GetOPCStatusRequest()
                            {
                                CommunityID = GlobalParams.Instance.CommunityID,
                                T133LeafID = device.T133LeafID,
                                T216LeafID = device.T216LeafID,
                                ParamXML = new GetOPCStatusParamXML()
                                {
                                    Equipment_Running_Mode =
                                        GetBoolValue(comm.Tags, "Equipment_Running_Mode"),
                                    Equipment_Power_On =
                                        GetBoolValue(comm.Tags, "Equipment_Power_On"),
                                    Equipment_Fail =
                                        GetBoolValue(comm.Tags, "Equipment_Fail"),
                                    Tool_Fail =
                                        GetBoolValue(comm.Tags, "Tool_Fail"),
                                    Cycle_Started =
                                        GetBoolValue(comm.Tags, "Cycle_Started"),
                                    Equipment_Starvation =
                                        GetBoolValue(comm.Tags, "Equipment_Starvation"),
                                },
                            },
                        };

                    try
                    {
                        if (getOPCStatus.Do())
                        {
                            if (getOPCStatus.Error.ErrCode == 0)
                            {
                                _log.Debug(
                                    $"[{id.ToString()}|({getOPCStatus.Error.ErrCode})" +
                                    $"{getOPCStatus.Error.ErrText}");
                            }
                            else
                            {
                                _log.Error(
                                    $"[{id.ToString()}|({getOPCStatus.Error.ErrCode})" +
                                    $"{getOPCStatus.Error.ErrText}");
                            }
                        }
                        else
                        {
                            _log.Error(
                                $"[{id.ToString()}|({getOPCStatus.Error.ErrCode})" +
                                $"{getOPCStatus.Error.ErrText}");
                        }
                    }
                    catch (Exception error)
                    {
                        _log.Error(error.Message, error);
                    }
                }
                else
                {
                    _log.Error($"未找到设备[{device.Name}]的COMM标记组");
                }
                _log.Info("处理完成");
            }

            return rlt;
        }
    }

    /// <summary>
    /// 设备运行模式状态同步
    /// </summary>
    public class IRAPDCSTradeEquipmentRunningMode : IRAPDCSTradeGetOPCStatus
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeEquipmentRunningMode(DCSGatewayLogEntity log) : base(log)
        {
        }
    }

    /// <summary>
    /// 设备是否加电状态同步
    /// </summary>
    public class IRAPDCSTradeEquipmentPowerOn : IRAPDCSTradeGetOPCStatus
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeEquipmentPowerOn(DCSGatewayLogEntity log) : base(log)
        {
        }
    }

    /// <summary>
    /// 设备是否失效状态同步
    /// </summary>
    public class IRAPDCSTradeEquipmentFail : IRAPDCSTradeGetOPCStatus
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeEquipmentFail(DCSGatewayLogEntity log) : base(log)
        {
        }
    }

    /// <summary>
    /// 工装是否失效状态同步
    /// </summary>
    public class IRAPDCSTradeToolFail : IRAPDCSTradeGetOPCStatus
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeToolFail(DCSGatewayLogEntity log) : base(log)
        {
        }
    }

    /// <summary>
    /// 工序循环是否开始状态同步
    /// </summary>
    public class IRAPDCSTradeCycleStarted : IRAPDCSTradeGetOPCStatus
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeCycleStarted(DCSGatewayLogEntity log) : base(log)
        {
        }
    }

    /// <summary>
    /// 设备饥饿状态同步
    /// </summary>
    public class IRAPDCSTradeEquipmentStarvation : IRAPDCSTradeGetOPCStatus
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeEquipmentStarvation(DCSGatewayLogEntity log) : base(log)
        {
        }
    }

    /// <summary>
    /// 标识部件绑定交易
    /// </summary>
    public class IRAPDCSTradeRequestForIDBinding : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeRequestForIDBinding(DCSGatewayLogEntity log) : base(log)
        {
        }

        /// <summary>
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();
            logEntity.DeviceName = device.Name;

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("标识部件绑定");
                    logEntity.ActionName = "标识部件绑定";

                    IDBinding idBinding = null;
                    SiemensSubTagGroup subTagGroup = signalTag.Parent as SiemensSubTagGroup;
                    if (subTagGroup != null)
                    {
                        idBinding =
                            new IDBinding(
                                GlobalParams.Instance.WebAPI.URL,
                                GlobalParams.Instance.WebAPI.ContentType,
                                GlobalParams.Instance.WebAPI.ClientID,
                                logEntity)
                            {
                                Request = new IDBindingRequest()
                                {
                                    CommunityID = GlobalParams.Instance.CommunityID,
                                    T133LeafID = device.T133LeafID,
                                    T216LeafID = device.T216LeafID,
                                    T102LeafID = subTagGroup.T102LeafID,
                                    T107LeafID = device.T107LeafID,//ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
                                    WIP_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_Code"),
                                    WIP_ID_Type_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_ID_Type_Code"),
                                    WIP_ID_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_ID_Code"),
                                },
                            };
                    }

                    string key = "IDBinding";
                    if (device.Groups[key] is SiemensTagGroup idbGroup)
                    {
                        if (idBinding == null)
                        {
                            _log.Error($"[{signalTag.Name}标记不在WIPStations的子组中，交易无法继续");
                            return rlt;
                        }

                        try
                        {
                            if (CallStartDCSInvoking(device, signalTag))
                            {
                                idBinding.Request.ParamXML.Product_Number = ReadStringValue(device, idbGroup.Tags, "Product_Number");
                                idBinding.Request.ParamXML.ID_Part_Number = ReadStringValue(device, idbGroup.Tags, "ID_Part_Number");
                                idBinding.Request.ParamXML.ID_Part_WIP_Code = ReadStringValue(device, idbGroup.Tags, "ID_Part_WIP_Code");
                                idBinding.Request.ParamXML.ID_Part_SN_Scanner_Code = ReadStringValue(device, idbGroup.Tags, "ID_Part_SN_Scanner_Code");
                                idBinding.Request.ParamXML.Sequence_Number = ReadIntValue(device, idbGroup.Tags, "Sequence_Number");

                                if (idBinding.Do())
                                {
                                    if (idBinding.Error.ErrCode >= 0)
                                    {
                                        _log.Debug(
                                            $"[{id.ToString()}|({idBinding.Error.ErrCode})" +
                                            $"{idBinding.Error.ErrText}");
                                    }
                                    else
                                    {
                                        _log.Error(
                                            $"[{id.ToString()}|({idBinding.Error.ErrCode})" +
                                            $"{idBinding.Error.ErrText}");
                                    }

                                    var fdTag = device.FindTag(key, "Part_Number_Feedback");
                                    if (fdTag != null)
                                    {
                                        if (fdTag is SiemensIntOfTag rltTag)
                                        {
                                            rltTag.Value = (short)idBinding.Response.Output.Part_Number_Feedback;
                                            rlt.Add(rltTag);
                                        }
                                    }
                                }
                                else
                                {
                                    _log.Error(
                                        $"[{id.ToString()}|({idBinding.Error.ErrCode})" +
                                        $"{idBinding.Error.ErrText}");
                                }
                            }
                        }
                        catch (Exception error)
                        {
                            _log.Error(error.Message, error);
                        }
                    }
                    else
                    {
                        _log.Error($"未找到设备[{device.Name}]的IDBinding标记组");
                    }

                    _log.Info("标识部件绑定处理完成");

                    tag.Value = false;
                    rlt.Add(tag);
                }
            }

            return rlt;
        }
    }

    /// <summary>
    /// 申请产品序列号交易
    /// </summary>
    public class IRAPDCSTradeSerialNumberRequest : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeSerialNumberRequest(DCSGatewayLogEntity log) : base(log)
        {
        }

        /// <summary>
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();
            logEntity.DeviceName = device.Name;

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("申请产品序列号");
                    logEntity.ActionName = "申请产品序列号";

                    string key = "SNRequest";
                    SiemensSubTagGroup subTagGroup = signalTag.Parent as SiemensSubTagGroup;
                    if (device.Groups[key] is SiemensTagGroup snrGroup)
                    {
                        try
                        {
                            if (CallStartDCSInvoking(device, signalTag))
                            {
                                SNRequest snRequest =
                                    new SNRequest(
                                        GlobalParams.Instance.WebAPI.URL,
                                        GlobalParams.Instance.WebAPI.ContentType,
                                        GlobalParams.Instance.WebAPI.ClientID,
                                logEntity)
                                    {
                                        Request = new SNRequestRequest()
                                        {
                                            CommunityID = GlobalParams.Instance.CommunityID,
                                            T133LeafID = device.T133LeafID,
                                            T216LeafID = device.T216LeafID,
                                            T102LeafID = subTagGroup.T102LeafID,
                                            T107LeafID = device.T107LeafID,//ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
                                            ParamXML = new SNRequestParamXML()
                                            {
                                                Product_Number = ReadStringValue(device, snrGroup.Tags, "Product_Number"),
                                            },
                                        },
                                    };

                                if (snRequest.Do())
                                {
                                    if (snRequest.Error.ErrCode >= 0)
                                    {
                                        _log.Debug(
                                            $"[{id.ToString()}|({snRequest.Error.ErrCode})" +
                                            $"{snRequest.Error.ErrText}");

                                        var writeTag = device.FindTag(key, "Serial_Number");
                                        if (writeTag != null)
                                        {
                                            if (writeTag is SiemensArrayCharOfTag rltTag)
                                            {
                                                rltTag.Value = snRequest.Response.Output.Serial_Number;
                                                rlt.Add(rltTag);
                                            }
                                        }
                                        writeTag = device.FindTag(key, "Length");
                                        if (writeTag != null)
                                        {
                                            if (writeTag is SiemensIntOfTag rltTag)
                                            {
                                                rltTag.Value = (short)snRequest.Response.Output.Serial_Number.Length;
                                                rlt.Add(rltTag);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _log.Error(
                                            $"[{id.ToString()}|({snRequest.Error.ErrCode})" +
                                            $"{snRequest.Error.ErrText}");
                                    }
                                }
                                else
                                {
                                    _log.Error(
                                        $"[{id.ToString()}|({snRequest.Error.ErrCode})" +
                                        $"{snRequest.Error.ErrText}");
                                }
                            }
                        }
                        catch (Exception error)
                        {
                            _log.Error(error.Message, error);
                        }
                    }
                    else
                    {
                        _log.Error($"未找到设备[{device.Name}]的SNRequest标记组");
                    }

                    _log.Info("申请产品序列号处理完成");

                    tag.Value = false;
                    rlt.Add(tag);
                }
            }

            return rlt;
        }
    }

    /// <summary>
    /// 工件入站交易
    /// </summary>
    public class IRAPDCSTradeWIPMoveIn : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeWIPMoveIn(DCSGatewayLogEntity log) : base(log)
        {
        }

        /// <summary>
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();
            logEntity.DeviceName = device.Name;

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("工件入站交易处理");
                    logEntity.ActionName = "工件入站";

                    WIPMoveIn wipMoveIn = null;
                    SiemensSubTagGroup subTagGroup = signalTag.Parent as SiemensSubTagGroup;
                    if (subTagGroup != null)
                    {
                        wipMoveIn =
                            new WIPMoveIn(
                                GlobalParams.Instance.WebAPI.URL,
                                GlobalParams.Instance.WebAPI.ContentType,
                                GlobalParams.Instance.WebAPI.ClientID,
                                logEntity)
                            {
                                Request = new WIPMoveInRequest()
                                {
                                    CommunityID = GlobalParams.Instance.CommunityID,
                                    T133LeafID = device.T133LeafID,
                                    T216LeafID = device.T216LeafID,
                                    T102LeafID = subTagGroup.T102LeafID,
                                    T107LeafID = device.T107LeafID,//ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
                                    WIP_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_Code"),
                                    WIP_ID_Type_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_ID_Type_Code"),
                                    WIP_ID_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_ID_Code"),
                                },
                            };
                    }

                    if (wipMoveIn == null)
                    {
                        _log.Error($"[{signalTag.Name}标记不在WIPStations的子组中，交易无法继续");
                        return rlt;
                    }

                    try
                    {
                        if (CallStartDCSInvoking(device, signalTag))
                        {
                            if (wipMoveIn.Do())
                            {
                                if (wipMoveIn.Error.ErrCode >= 0)
                                {
                                    _log.Debug(
                                        $"[{id.ToString()}|({wipMoveIn.Error.ErrCode})" +
                                        $"{wipMoveIn.Error.ErrText}");
                                }
                                else
                                {
                                    _log.Error(
                                        $"[{id.ToString()}|({wipMoveIn.Error.ErrCode})" +
                                        $"{wipMoveIn.Error.ErrText}");
                                }
                            }
                            else
                            {
                                _log.Error(
                                    $"[{id.ToString()}|({wipMoveIn.Error.ErrCode})" +
                                    $"{wipMoveIn.Error.ErrText}");
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        _log.Error(error.Message, error);
                    }

                    tag.Value = false;
                    rlt.Add(tag);

                    _log.Info("工件入站处理完成");
                }
            }

            return rlt;
        }
    }

    /// <summary>
    /// 生产结束交易
    /// </summary>
    public class IRAPDCSTradeProductionEnd : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeProductionEnd(DCSGatewayLogEntity log) : base(log)
        {
        }

        /// <summary>
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();
            logEntity.DeviceName = device.Name;

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("生产结束交易处理");
                    logEntity.ActionName = "生产结束";

                    ProductionEnd productionEnd = null;
                    SiemensSubTagGroup subTagGroup = signalTag.Parent as SiemensSubTagGroup;
                    if (subTagGroup != null)
                    {
                        productionEnd =
                            new ProductionEnd(
                                GlobalParams.Instance.WebAPI.URL,
                                GlobalParams.Instance.WebAPI.ContentType,
                                GlobalParams.Instance.WebAPI.ClientID,
                                logEntity)
                            {
                                Request = new ProductionEndRequest()
                                {
                                    CommunityID = GlobalParams.Instance.CommunityID,
                                    T133LeafID = device.T133LeafID,
                                    T216LeafID = device.T216LeafID,
                                    T102LeafID = subTagGroup.T102LeafID,
                                    T107LeafID = device.T107LeafID,//ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
                                    WIP_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_Code"),
                                    WIP_ID_Type_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_ID_Type_Code"),
                                    WIP_ID_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_ID_Code"),
                                },
                            };

                        #region 填充 RECIPE 组
                        productionEnd.Request.ParamXML.Operation_Conclusion =
                            ReadByteValue(device, subTagGroup.Tags, "Operation_Conclusion");
                        if (device.Groups["RECIPE"] is SiemensTagGroup recipeGroup)
                        {
                            foreach (SiemensTag recipeTag in recipeGroup.Tags)
                            {
                                device.ReadTagValue(recipeTag);
                                productionEnd.Request.ParamXML.RECIPE.Add(
                                    new RecipeRow()
                                    {
                                        TagName = recipeTag.Name,
                                        Value = recipeTag.Value.ToString(),
                                    });
                            }
                        }
                        #endregion

                        #region 填充 PROPERTY 组
                        if (device.Groups["PROPERTY"] is SiemensTagGroup propertyGroup)
                        {
                            foreach (SiemensTag propertyTag in propertyGroup.Tags)
                            {
                                device.ReadTagValue(propertyTag);
                                productionEnd.Request.ParamXML.PROPERTY.Add(
                                    new PropertyRow()
                                    {
                                        TagName = propertyTag.Name,
                                        Value = propertyTag.Value.ToString(),
                                    });
                            }
                        }
                        #endregion

                        #region 填充 TestResult 组
                        if (device.Groups["TestResult"] is SiemensTagGroup trGroup)
                        {
                            foreach (SiemensSubTagGroup subGroup in trGroup.SubGroups)
                            {
                                productionEnd.Request.ParamXML.TestResult.Add(
                                    new TestResultRow()
                                    {
                                        Test_Item_Number = (ushort)ReadIntValue(device, subGroup.Tags, "Test_Item_Number"),
                                        Conclusion = ReadStringValue(device, subGroup.Tags, "Conclusion"),
                                        Remark = ReadStringValue(device, subGroup.Tags, "Remark"),
                                        Metric01 = (uint)ReadIntValue(device, subGroup.Tags, "Metric01"),
                                        Low_Limit = (uint)ReadIntValue(device, subGroup.Tags, "Low_Limit"),
                                        Criterion = ReadStringValue(device, subGroup.Tags, "Criterion"),
                                        High_Limit = (uint)ReadIntValue(device, subGroup.Tags, "High_Limit"),
                                    });
                            }
                        }
                        #endregion

                        #region 填充 ToolLife 组
                        if (device.Groups["ToolLife"] is SiemensTagGroup tlGroup)
                        {
                            foreach (SiemensSubTagGroup subGroup in tlGroup.SubGroups)
                            {
                                productionEnd.Request.ParamXML.ToolLife.Add(
                                    new ToolLifeRow()
                                    {
                                        Tool_Code = ReadStringValue(device, subGroup.Tags, "Tool_Code"),
                                        Tool_SN = ReadStringValue(device, subGroup.Tags, "Tool_SN"),
                                        Tool_Use_Life_In_Times = (uint)ReadIntValue(device, subGroup.Tags, "Tool_Use_Life_In_Times"),
                                        Tool_PM_Life_In_Times = (uint)ReadIntValue(device, subGroup.Tags, "Tools_PM_Life_In_Times"),
                                    });
                            }
                        }
                        #endregion
                    }

                    if (productionEnd == null)
                    {
                        _log.Error($"[{signalTag.Name}标记不在WIPStations的子组中，交易无法继续");
                        return rlt;
                    }

                    try
                    {
                        if (CallStartDCSInvoking(device, signalTag))
                        {
                            if (productionEnd.Do())
                            {
                                if (productionEnd.Error.ErrCode >= 0)
                                {
                                    _log.Debug(
                                        $"[{id.ToString()}|({productionEnd.Error.ErrCode})" +
                                        $"{productionEnd.Error.ErrText}");

                                    var writeTag = subTagGroup.Tags["Poka_Yoke_Result"];
                                    if (writeTag != null)
                                    {
                                        if (writeTag is SiemensByteOfTag rltTag)
                                        {
                                            rltTag.Value = productionEnd.Response.Output.Poka_Yoke_Result;
                                            rlt.Add(rltTag);
                                        }
                                    }
                                }
                                else
                                {
                                    _log.Error(
                                        $"[{id.ToString()}|({productionEnd.Error.ErrCode})" +
                                        $"{productionEnd.Error.ErrText}");
                                }
                            }
                            else
                            {
                                _log.Error(
                                    $"[{id.ToString()}|({productionEnd.Error.ErrCode})" +
                                    $"{productionEnd.Error.ErrText}");
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        _log.Error(error.Message, error);
                    }

                    tag.Value = false;
                    rlt.Add(tag);

                    _log.Info("生产结束处理完成");
                }
            }

            return rlt;
        }
    }

    /// <summary>
    /// 工件离站交易
    /// </summary>
    public class IRAPDCSTradeWIPMoveOut : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeWIPMoveOut(DCSGatewayLogEntity log) : base(log)
        {
        }

        /// <summary>
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();
            logEntity.DeviceName = device.Name;

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("工件离站交易处理");
                    logEntity.ActionName = "工件离站";

                    OperationCycleEnd operationCycleEnd = null;
                    SiemensSubTagGroup subTagGroup = signalTag.Parent as SiemensSubTagGroup;
                    if (subTagGroup != null)
                    {
                        operationCycleEnd =
                            new OperationCycleEnd(
                                GlobalParams.Instance.WebAPI.URL,
                                GlobalParams.Instance.WebAPI.ContentType,
                                GlobalParams.Instance.WebAPI.ClientID,
                                logEntity)
                            {
                                Request = new OperationCycleEndRequest()
                                {
                                    CommunityID = GlobalParams.Instance.CommunityID,
                                    T133LeafID = device.T133LeafID,
                                    T216LeafID = device.T216LeafID,
                                    T102LeafID = subTagGroup.T102LeafID,
                                    T107LeafID = device.T107LeafID,//ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
                                    WIP_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_Code"),
                                    WIP_ID_Type_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_ID_Type_Code"),
                                    WIP_ID_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_ID_Code"),
                                },
                            };
                    }

                    if (operationCycleEnd == null)
                    {
                        _log.Error($"[{signalTag.Name}标记不在WIPStations的子组中，交易无法继续");
                        return rlt;
                    }

                    try
                    {
                        if (CallStartDCSInvoking(device, signalTag))
                        {
                            if (operationCycleEnd.Do())
                            {
                                if (operationCycleEnd.Error.ErrCode >= 0)
                                {
                                    _log.Debug(
                                        $"[{id.ToString()}|({operationCycleEnd.Error.ErrCode})" +
                                        $"{operationCycleEnd.Error.ErrText}");
                                }
                                else
                                {
                                    _log.Error(
                                        $"[{id.ToString()}|({operationCycleEnd.Error.ErrCode})" +
                                        $"{operationCycleEnd.Error.ErrText}");
                                }
                            }
                            else
                            {
                                _log.Error(
                                    $"[{id.ToString()}|({operationCycleEnd.Error.ErrCode})" +
                                    $"{operationCycleEnd.Error.ErrText}");
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        _log.Error(error.Message, error);
                    }

                    tag.Value = false;
                    rlt.Add(tag);

                    _log.Info("工件离站处理完成");
                }
            }

            return rlt;
        }
    }

    /// <summary>
    /// 请求标签元素交易
    /// </summary>
    public class IRAPDCSTradeLabelElementsRequest : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeLabelElementsRequest(DCSGatewayLogEntity log) : base(log)
        {
        }

        /// <summary>
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();
            logEntity.DeviceName = device.Name;

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("请求标签元素交易处理");
                    logEntity.ActionName = "请求标签元素";

                    LBLElement lblElement = null;
                    SiemensSubTagGroup subTagGroup = signalTag.Parent as SiemensSubTagGroup;
                    if (subTagGroup != null)
                    {
                        lblElement =
                            new LBLElement(
                                GlobalParams.Instance.WebAPI.URL,
                                GlobalParams.Instance.WebAPI.ContentType,
                                GlobalParams.Instance.WebAPI.ClientID,
                                logEntity)
                            {
                                Request = new LBLElementRequest()
                                {
                                    CommunityID = GlobalParams.Instance.CommunityID,
                                    T133LeafID = device.T133LeafID,
                                    T216LeafID = device.T216LeafID,
                                    T102LeafID = subTagGroup.T102LeafID,
                                    T107LeafID = device.T107LeafID,//ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
                                    WIP_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_Code"),
                                    WIP_ID_Type_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_ID_Type_Code"),
                                    WIP_ID_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_ID_Code"),
                                },
                            };
                    }

                    if (lblElement == null)
                    {
                        _log.Error($"[{signalTag.Name}标记不在WIPStations的子组中，交易无法继续");
                        return rlt;
                    }

                    try
                    {
                        if (CallStartDCSInvoking(device, signalTag))
                        {
                            if (lblElement.Do())
                            {
                                if (lblElement.Error.ErrCode >= 0)
                                {
                                    _log.Debug(
                                        $"[{id.ToString()}|({lblElement.Error.ErrCode})" +
                                        $"{lblElement.Error.ErrText}");

                                    if (device.Groups["LBLElement"] is SiemensTagGroup group)
                                    {
                                        WriteTagValueBack(rlt, group, "Custom_Part_Number", lblElement.Response.Output.Customer_Part_Number);
                                        WriteTagValueBack(rlt, group, "Product_Serial_Number", lblElement.Response.Output.Product_Serial_Number);
                                        WriteTagValueBack(rlt, group, "Model_ID", lblElement.Response.Output.Model_ID);
                                        WriteTagValueBack(rlt, group, "Vendor_Code_Of_Us", lblElement.Response.Output.Vendor_Code_Of_Us);
                                        WriteTagValueBack(rlt, group, "Sales_Part_Number", lblElement.Response.Output.Sales_Part_Number);
                                        WriteTagValueBack(rlt, group, "Hardware_Version", lblElement.Response.Output.Hardware_Version);
                                        WriteTagValueBack(rlt, group, "Software_Version", lblElement.Response.Output.Software_Version);
                                        WriteTagValueBack(rlt, group, "Lot_Number", lblElement.Response.Output.Lot_Number);
                                        WriteTagValueBack(rlt, group, "MFG_Date", lblElement.Response.Output.MFG_Date);
                                        WriteTagValueBack(rlt, group, "Shift_Number", lblElement.Response.Output.Shift_Number);
                                        WriteTagValueBack(rlt, group, "Oven_Number", lblElement.Response.Output.Oven_Number);
                                        WriteTagValueBack(rlt, group, "Customer_Duns_Code", lblElement.Response.Output.Customer_Duns_Code);
                                        WriteTagValueBack(rlt, group, "OEM_Brand", lblElement.Response.Output.OEM_Brand);
                                        WriteTagValueBack(rlt, group, "Fixed_String_1", lblElement.Response.Output.Fixed_String_1);
                                        WriteTagValueBack(rlt, group, "Fixed_String_2", lblElement.Response.Output.Fixed_String_2);
                                        WriteTagValueBack(rlt, group, "Derived_String_1", lblElement.Response.Output.Derived_String_1);
                                        WriteTagValueBack(rlt, group, "Derived_String_2", lblElement.Response.Output.Derived_String_2);
                                    }
                                }
                                else
                                {
                                    _log.Error(
                                        $"[{id.ToString()}|({lblElement.Error.ErrCode})" +
                                        $"{lblElement.Error.ErrText}");
                                }
                            }
                            else
                            {
                                _log.Error(
                                    $"[{id.ToString()}|({lblElement.Error.ErrCode})" +
                                    $"{lblElement.Error.ErrText}");
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        _log.Error(error.Message, error);
                    }


                    tag.Value = false;
                    rlt.Add(tag);

                    _log.Info("请求标签元素处理完成");
                }
            }

            return rlt;
        }
    }

    /// <summary>
    /// 请求防错交易
    /// </summary>
    public class IRAPDCSTradeRequestForPokaYoke : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeRequestForPokaYoke(DCSGatewayLogEntity log) : base(log)
        {
        }

        /// <summary>
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();
            logEntity.DeviceName = device.Name;

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("请求防错交易处理");
                    logEntity.ActionName = "请求防错";

                    PokaYoke pokaYoke = null;
                    SiemensSubTagGroup subWIPStation = signalTag.Parent as SiemensSubTagGroup;
                    if (subWIPStation != null)
                    {
                        pokaYoke =
                            new PokaYoke(
                                GlobalParams.Instance.WebAPI.URL,
                                GlobalParams.Instance.WebAPI.ContentType,
                                GlobalParams.Instance.WebAPI.ClientID,
                                logEntity)
                            {
                                Request = new PokaYokeRequest()
                                {
                                    CommunityID = GlobalParams.Instance.CommunityID,
                                    T133LeafID = device.T133LeafID,
                                    T216LeafID = device.T216LeafID,
                                    T102LeafID = subWIPStation.T102LeafID,
                                    T107LeafID = device.T107LeafID,//ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
                                    WIP_Code = ReadStringValue(device, subWIPStation.Tags, "WIP_Code"),
                                    WIP_ID_Type_Code = ReadStringValue(device, subWIPStation.Tags, "WIP_ID_Type_Code"),
                                    WIP_ID_Code = ReadStringValue(device, subWIPStation.Tags, "WIP_ID_Code"),
                                },
                            };
                    }

                    if (pokaYoke == null)
                    {
                        _log.Error($"[{signalTag.Name}标记不在WIPStations的子组中，交易无法继续");
                        return rlt;
                    }

                    if (device.Groups["WIPOntoLine"] is SiemensTagGroup wipOntoLine)
                    {
                        string wipIDCode = ReadStringValue(device, wipOntoLine.Tags, "WIP_ID_Code");
                        if (wipIDCode != "")
                        {
                            pokaYoke.Request.ParamXML =
                                new PokaYokeParamXML()
                                {
                                    WIP_Src_Code = ReadStringValue(device, wipOntoLine.Tags, "WIP_Src_Code"),
                                    WIP_ID_Code = wipIDCode,
                                    Container_Number_pallet_code = ReadStringValue(device, wipOntoLine.Tags, "Container_Number_pallet_code"),
                                };
                        }
                    }

                    try
                    {
                        if (CallStartDCSInvoking(device, signalTag))
                        {
                            if (pokaYoke.Do())
                            {
                                if (pokaYoke.Error.ErrCode >= 0)
                                {
                                    _log.Debug(
                                        $"[{id.ToString()}|({pokaYoke.Error.ErrCode})" +
                                        $"{pokaYoke.Error.ErrText}");

                                    WriteTagValueBack(rlt, subWIPStation, "Poka_Yoke_Result", pokaYoke.Response.Output.WIPStations.Poka_Yoke_Result);
                                    WriteTagValueBack(rlt, subWIPStation, "Product_Number", pokaYoke.Response.Output.WIPStations.Product_Number);
                                    subWIPStation.T102LeafID = pokaYoke.Response.Output.WIPStations.T102LeafID;

                                    if (pokaYoke.Response.Output.WIPOntoLine != null)
                                    {
                                        if (device.Groups["WIPOntoLine"] is SiemensTagGroup group)
                                        {
                                            WriteTagValueBack(rlt, group, "Number_Of_Sub_WIPs", pokaYoke.Response.Output.WIPOntoLine.Number_Of_Sub_WIPs);
                                            int j = 0;
                                            for (int i = 0; i < pokaYoke.Response.Output.WIPOntoLine.SubWIPs.Count; i++)
                                            {
                                                j = i;
                                                PokaYokeOutputWIPOntoLineSubWIP subWIP = pokaYoke.Response.Output.WIPOntoLine.SubWIPs[i];
                                                if (i < group.SubGroups.Count)
                                                {
                                                    SiemensSubTagGroup subWIPGroup = group.SubGroups[i] as SiemensSubTagGroup;
                                                    WriteTagValueBack(rlt, subWIPGroup, "WIP_Code", subWIP.WIP_Code);
                                                    WriteTagValueBack(rlt, subWIPGroup, "WIP_ID_Type_Code", subWIP.WIP_ID_Type_Code);
                                                    WriteTagValueBack(rlt, subWIPGroup, "WIP_ID_Code", subWIP.WIP_ID_Code);
                                                    WriteTagValueBack(rlt, subWIPGroup, "PWO_Number", subWIP.PWO_Number);
                                                    WriteTagValueBack(rlt, subWIPGroup, "Sub_Container_Number", subWIP.Sub_Container_Number);
                                                    WriteTagValueBack(rlt, subWIPGroup, "WIP_Quantity", subWIP.WIP_Quantity);
                                                    break;
                                                }
                                            }

                                            for (int i = j + 1; i < group.SubGroups.Count; i++)
                                            {
                                                SiemensSubTagGroup subWIPGroup = group.SubGroups[i] as SiemensSubTagGroup;
                                                WriteTagValueBack(rlt, subWIPGroup, "WIP_Code", "");
                                                WriteTagValueBack(rlt, subWIPGroup, "WIP_ID_Type_Code", "");
                                                WriteTagValueBack(rlt, subWIPGroup, "WIP_ID_Code", "");
                                                WriteTagValueBack(rlt, subWIPGroup, "PWO_Number", "");
                                                WriteTagValueBack(rlt, subWIPGroup, "Sub_Container_Number", "");
                                                WriteTagValueBack(rlt, subWIPGroup, "WIP_Quantity", 0);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    _log.Error(
                                        $"[{id.ToString()}|({pokaYoke.Error.ErrCode})" +
                                        $"{pokaYoke.Error.ErrText}");
                                }
                            }
                            else
                            {
                                _log.Error(
                                    $"[{id.ToString()}|({pokaYoke.Error.ErrCode})" +
                                    $"{pokaYoke.Error.ErrText}");
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        _log.Error(error.Message, error);
                    }

                    tag.Value = false;
                    rlt.Add(tag);

                    _log.Info("请求防错处理完成");
                }
            }

            return rlt;
        }
    }

    /// <summary>
    /// 设备故障告警交易
    /// </summary>
    public class IRAPDCSTradeTriggerEquipmentFailAndon : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeTriggerEquipmentFailAndon(DCSGatewayLogEntity log) : base(log)
        {
        }

        /// <summary>
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();
            logEntity.DeviceName = device.Name;

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("设备故障告警交易处理");
                    logEntity.ActionName = "设备故障告警";

                    EquipFailAndonCall equipFailAndonCall = null;
                    SiemensSubTagGroup subWIPStation = signalTag.Parent as SiemensSubTagGroup;
                    if (subWIPStation != null)
                    {
                        equipFailAndonCall =
                            new EquipFailAndonCall(
                                GlobalParams.Instance.WebAPI.URL,
                                GlobalParams.Instance.WebAPI.ContentType,
                                GlobalParams.Instance.WebAPI.ClientID,
                                logEntity)
                            {
                                Request = new EquipFailAndonCallRequest()
                                {
                                    CommunityID = GlobalParams.Instance.CommunityID,
                                    T133LeafID = device.T133LeafID,
                                    T216LeafID = device.T216LeafID,
                                    T102LeafID = subWIPStation.T102LeafID,
                                    T107LeafID = device.T107LeafID,//ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
                                    WIP_Code = ReadStringValue(device, subWIPStation.Tags, "WIP_Code"),
                                    WIP_ID_Type_Code = ReadStringValue(device, subWIPStation.Tags, "WIP_ID_Type_Code"),
                                    WIP_ID_Code = ReadStringValue(device, subWIPStation.Tags, "WIP_ID_Code"),
                                },
                            };
                    }

                    if (equipFailAndonCall == null)
                    {
                        _log.Error($"[{signalTag.Name}标记不在WIPStations的子组中，交易无法继续");
                        return rlt;
                    }

                    if (device.Groups["EquipmentFail"] is SiemensTagGroup srcGroup)
                    {
                        equipFailAndonCall.Request.ParamXML.Equipment_Failures_Group_1 = ReadDWordValue(device, srcGroup.Tags, "Equipment_Failures_Group_1");
                        equipFailAndonCall.Request.ParamXML.Equipment_Failures_Group_2 = ReadDWordValue(device, srcGroup.Tags, "Equipment_Failures_Group_2");
                        equipFailAndonCall.Request.ParamXML.Equipment_Failures_Group_3 = ReadDWordValue(device, srcGroup.Tags, "Equipment_Failures_Group_3");
                        equipFailAndonCall.Request.ParamXML.Equipment_Failures_Group_4 = ReadDWordValue(device, srcGroup.Tags, "Equipment_Failures_Group_4");
                        equipFailAndonCall.Request.ParamXML.Equipment_Failures_Group_5 = ReadDWordValue(device, srcGroup.Tags, "Equipment_Failures_Group_5");
                        equipFailAndonCall.Request.ParamXML.Equipment_Failures_Group_6 = ReadDWordValue(device, srcGroup.Tags, "Equipment_Failures_Group_6");
                        equipFailAndonCall.Request.ParamXML.Equipment_Failures_Group_7 = ReadDWordValue(device, srcGroup.Tags, "Equipment_Failures_Group_7");
                        equipFailAndonCall.Request.ParamXML.Equipment_Failures_Group_8 = ReadDWordValue(device, srcGroup.Tags, "Equipment_Failures_Group_8");
                        equipFailAndonCall.Request.ParamXML.Failure_Code = ReadStringValue(device, srcGroup.Tags, "Failure_Code");
                    }

                    try
                    {
                        if (CallStartDCSInvoking(device, signalTag))
                        {
                            if (equipFailAndonCall.Do())
                            {
                                if (equipFailAndonCall.Error.ErrCode >= 0)
                                {
                                    _log.Debug(
                                        $"[{id.ToString()}|({equipFailAndonCall.Error.ErrCode})" +
                                        $"{equipFailAndonCall.Error.ErrText}");
                                }
                                else
                                {
                                    _log.Error(
                                        $"[{id.ToString()}|({equipFailAndonCall.Error.ErrCode})" +
                                        $"{equipFailAndonCall.Error.ErrText}");
                                }
                            }
                            else
                            {
                                _log.Error(
                                    $"[{id.ToString()}|({equipFailAndonCall.Error.ErrCode})" +
                                    $"{equipFailAndonCall.Error.ErrText}");
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        _log.Error(error.Message, error);
                    }

                    tag.Value = false;
                    rlt.Add(tag);

                    _log.Info("设备故障告警处理完成");
                }
            }

            return rlt;
        }
    }

    /// <summary>
    /// 料槽加料防错交易
    /// </summary>
    public class IRAPDCSTradeRequestForFeedingPokaYoke : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeRequestForFeedingPokaYoke(DCSGatewayLogEntity log) : base(log)
        {
        }

        /// <summary>
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();
            logEntity.DeviceName = device.Name;

            if (signalTag is SiemensByteOfTag)
            {
                SiemensByteOfTag tag = signalTag as SiemensByteOfTag;
                if (tag.Value == 1)
                {
                    _log.Info("料槽加料防错交易处理");
                    logEntity.ActionName = "料槽加料防错";

                    PokaYokeFeeding pokaYokeFeeding = null;
                    SiemensTagGroup feeding = signalTag.Parent as SiemensTagGroup;
                    if (feeding != null)
                    {
                        pokaYokeFeeding =
                            new PokaYokeFeeding(
                                GlobalParams.Instance.WebAPI.URL,
                                GlobalParams.Instance.WebAPI.ContentType,
                                GlobalParams.Instance.WebAPI.ClientID,
                                logEntity)
                            {
                                Request = new PokaYokeFeedingRequest()
                                {
                                    CommunityID = GlobalParams.Instance.CommunityID,
                                    T133LeafID = device.T133LeafID,
                                    T216LeafID = device.T216LeafID,
                                    T107LeafID = device.T107LeafID,//ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
                                    WIP_Code = ReadStringValue(device, feeding.Tags, "WIP_Code"),
                                    WIP_ID_Type_Code = ReadStringValue(device, feeding.Tags, "WIP_ID_Type_Code"),
                                    WIP_ID_Code = ReadStringValue(device, feeding.Tags, "WIP_ID_Code"),
                                },
                            };
                    }

                    if (pokaYokeFeeding == null)
                    {
                        _log.Error($"[{signalTag.Name}标记不在FEEDING的子组中，交易无法继续");
                        return rlt;
                    }

                    pokaYokeFeeding.Request.ParamXML.Material_Track_ID = ReadStringValue(device, feeding.Tags, "Material_Track_ID");
                    pokaYokeFeeding.Request.ParamXML.Slot_Number = ReadStringValue(device, feeding.Tags, "Slot_Number");

                    try
                    {
                        if (CallStartDCSInvoking(device, signalTag))
                        {
                            if (pokaYokeFeeding.Do())
                            {
                                if (pokaYokeFeeding.Error.ErrCode >= 0)
                                {
                                    _log.Debug(
                                        $"[{id.ToString()}|({pokaYokeFeeding.Error.ErrCode})" +
                                        $"{pokaYokeFeeding.Error.ErrText}");

                                    WriteTagValueBack(rlt, feeding, "Poka_Yoke_Result", pokaYokeFeeding.Response.Output.Poka_Yoke_Result);
                                }
                                else
                                {
                                    _log.Error(
                                        $"[{id.ToString()}|({pokaYokeFeeding.Error.ErrCode})" +
                                        $"{pokaYokeFeeding.Error.ErrText}");
                                    WriteTagValueBack(rlt, feeding, "Poka_Yoke_Result", (byte)2);
                                }
                            }
                            else
                            {
                                _log.Error(
                                    $"[{id.ToString()}|({pokaYokeFeeding.Error.ErrCode})" +
                                    $"{pokaYokeFeeding.Error.ErrText}");
                                WriteTagValueBack(rlt, feeding, "Poka_Yoke_Result", (byte)2);
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        _log.Error(error.Message, error);
                    }

                    tag.Value = 0;
                    rlt.Add(tag);

                    _log.Info("料槽加料防错处理完成");
                }
            }

            return rlt;
        }
    }

    /// <summary>
    /// 料槽卸料交易
    /// </summary>
    public class IRAPDCSTradeUnfeedingEnd : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeUnfeedingEnd(DCSGatewayLogEntity log) : base(log)
        {
        }

        /// <summary>
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();
            logEntity.DeviceName = device.Name;

            if (signalTag is SiemensByteOfTag)
            {
                SiemensByteOfTag tag = signalTag as SiemensByteOfTag;
                if (tag.Value == 1)
                {
                    _log.Info("料槽卸料交易处理");
                    logEntity.ActionName = "料槽卸料";

                    Unfeeding unfeeding = null;
                    SiemensTagGroup unfeedingGroup = signalTag.Parent as SiemensTagGroup;
                    if (unfeedingGroup != null)
                    {
                        unfeeding =
                            new Unfeeding(
                                GlobalParams.Instance.WebAPI.URL,
                                GlobalParams.Instance.WebAPI.ContentType,
                                GlobalParams.Instance.WebAPI.ClientID,
                                logEntity)
                            {
                                Request = new UnfeedingRequest()
                                {
                                    CommunityID = GlobalParams.Instance.CommunityID,
                                    T133LeafID = device.T133LeafID,
                                    T216LeafID = device.T216LeafID,
                                    T107LeafID = device.T107LeafID,//ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
                                    WIP_Code = ReadStringValue(device, unfeedingGroup.Tags, "WIP_Code"),
                                    WIP_ID_Type_Code = ReadStringValue(device, unfeedingGroup.Tags, "WIP_ID_Type_Code"),
                                    WIP_ID_Code = ReadStringValue(device, unfeedingGroup.Tags, "WIP_ID_Code"),
                                },
                            };
                    }

                    if (unfeeding == null)
                    {
                        _log.Error($"[{signalTag.Name}标记不在UNFEEDING的子组中，交易无法继续");
                        return rlt;
                    }

                    unfeeding.Request.ParamXML.Material_Track_ID = ReadStringValue(device, unfeedingGroup.Tags, "Material_Track_ID");
                    unfeeding.Request.ParamXML.Slot_Number = ReadStringValue(device, unfeedingGroup.Tags, "Slot_Number");
                    unfeeding.Request.ParamXML.Unfeeding_Quantity = ReadDWordValue(device, unfeedingGroup.Tags, "Unfeeding_Quantity");

                    try
                    {
                        if (CallStartDCSInvoking(device, signalTag))
                        {
                            if (unfeeding.Do())
                            {
                                if (unfeeding.Error.ErrCode >= 0)
                                {
                                    _log.Debug(
                                        $"[{id.ToString()}|({unfeeding.Error.ErrCode})" +
                                        $"{unfeeding.Error.ErrText}");
                                }
                                else
                                {
                                    _log.Error(
                                        $"[{id.ToString()}|({unfeeding.Error.ErrCode})" +
                                        $"{unfeeding.Error.ErrText}");
                                    WriteTagValueBack(rlt, unfeedingGroup, "Poka_Yoke_Result", (byte)2);
                                }
                            }
                            else
                            {
                                _log.Error(
                                    $"[{id.ToString()}|({unfeeding.Error.ErrCode})" +
                                    $"{unfeeding.Error.ErrText}");
                                WriteTagValueBack(rlt, unfeedingGroup, "Poka_Yoke_Result", (byte)2);
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        _log.Error(error.Message, error);
                    }

                    tag.Value = 0;
                    rlt.Add(tag);

                    _log.Info("料槽卸料处理完成");
                }
            }

            return rlt;
        }
    }

    /// <summary>
    /// 停滞告警交易
    /// </summary>
    public class IRAPDCSTradeStagnationWarnning : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeStagnationWarnning(DCSGatewayLogEntity log) : base(log)
        {
        }

        /// <summary>
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();
            logEntity.DeviceName = device.Name;

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("停滞告警交易处理");
                    logEntity.ActionName = "停滞告警";

                    StagnationWarnning stagnationWarnning = null;
                    SiemensSubTagGroup wipStation = signalTag.Parent as SiemensSubTagGroup;
                    if (wipStation != null)
                    {
                        stagnationWarnning =
                            new StagnationWarnning(
                                GlobalParams.Instance.WebAPI.URL,
                                GlobalParams.Instance.WebAPI.ContentType,
                                GlobalParams.Instance.WebAPI.ClientID,
                                logEntity)
                            {
                                Request = new StagnationWarnningRequest()
                                {
                                    CommunityID = GlobalParams.Instance.CommunityID,
                                    T133LeafID = device.T133LeafID,
                                    T216LeafID = device.T216LeafID,
                                    T102LeafID = wipStation.T102LeafID,
                                    T107LeafID = device.T107LeafID,//ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
                                    WIP_Code = ReadStringValue(device, wipStation.Tags, "WIP_Code"),
                                    WIP_ID_Type_Code = ReadStringValue(device, wipStation.Tags, "WIP_ID_Type_Code"),
                                    WIP_ID_Code = ReadStringValue(device, wipStation.Tags, "WIP_ID_Code"),
                                },
                            };
                    }

                    if (stagnationWarnning == null)
                    {
                        _log.Error($"[{signalTag.Name}标记不在WIPStations的子组中，交易无法继续");
                        return rlt;
                    }

                    if (device.Groups["Stagnation"] is SiemensTagGroup stagnation)
                    {
                        stagnationWarnning.Request.ParamXML.Time_In_Seconds = ReadDWordValue(device, stagnation.Tags, "Material_Track_ID");
                        stagnationWarnning.Request.ParamXML.Threshold = ReadWordValue(device, stagnation.Tags, "Slot_Number");
                    }

                    try
                    {
                        if (CallStartDCSInvoking(device, signalTag))
                        {
                            if (stagnationWarnning.Do())
                            {
                                if (stagnationWarnning.Error.ErrCode >= 0)
                                {
                                    _log.Debug(
                                        $"[{id.ToString()}|({stagnationWarnning.Error.ErrCode})" +
                                        $"{stagnationWarnning.Error.ErrText}");
                                }
                                else
                                {
                                    _log.Error(
                                        $"[{id.ToString()}|({stagnationWarnning.Error.ErrCode})" +
                                        $"{stagnationWarnning.Error.ErrText}");
                                }
                            }
                            else
                            {
                                _log.Error(
                                    $"[{id.ToString()}|({stagnationWarnning.Error.ErrCode})" +
                                    $"{stagnationWarnning.Error.ErrText}");
                                WriteTagValueBack(rlt, wipStation, "Poka_Yoke_Result", (byte)2);
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        _log.Error(error.Message, error);
                    }

                    tag.Value = false;
                    rlt.Add(tag);

                    _log.Info("停滞告警处理完成");
                }
            }

            return rlt;
        }
    }

    /// <summary>
    /// Fazit Response状态检测交易
    /// </summary>
    public class IRAPDCSTradeFazitStatusCheck : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeFazitStatusCheck(DCSGatewayLogEntity log) : base(log)
        {
        }

        /// <summary>
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();
            logEntity.DeviceName = device.Name;

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("Fazit Response状态检测交易处理");
                    logEntity.ActionName = "Fazit Response状态检测";

                    if (!(signalTag.Parent is SiemensSubTagGroup wipStation))
                    {
                        return rlt;
                    }

                    try
                    {
                        if (!CallStartDCSInvoking(device, signalTag))
                        {
                            tag.Value = false;
                            rlt.Add(tag);
                            return rlt;
                        }

                        #region 首先需要进行 DMC 的校验
                        {
                            PokaYoke pokaYoke =
                                new PokaYoke(
                                    GlobalParams.Instance.WebAPI.URL,
                                    GlobalParams.Instance.WebAPI.ContentType,
                                    GlobalParams.Instance.WebAPI.ClientID,
                                logEntity)
                                {
                                    Request = new PokaYokeRequest()
                                    {
                                        CommunityID = GlobalParams.Instance.CommunityID,
                                        T133LeafID = device.T133LeafID,
                                        T216LeafID = device.T216LeafID,
                                        T102LeafID = wipStation.T102LeafID,
                                        T107LeafID = device.T107LeafID,//ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
                                        WIP_Code = ReadStringValue(device, wipStation.Tags, "WIP_Code"),
                                        WIP_ID_Type_Code = ReadStringValue(device, wipStation.Tags, "WIP_ID_Type_Code"),
                                        WIP_ID_Code = ReadStringValue(device, wipStation.Tags, "WIP_ID_Code"),
                                    },
                                };

                            if (pokaYoke.Do())
                            {
                                if (pokaYoke.Error.ErrCode >= 0)
                                {
                                    _log.Debug(
                                        $"[{id.ToString()}|({pokaYoke.Error.ErrCode})" +
                                        $"{pokaYoke.Error.ErrText}");

                                    WriteTagValueBack(rlt, wipStation, "Poka_Yoke_Result", pokaYoke.Response.Output.WIPStations.Poka_Yoke_Result);
                                    WriteTagValueBack(rlt, wipStation, "Product_Number", pokaYoke.Response.Output.WIPStations.Product_Number);
                                    wipStation.T102LeafID = pokaYoke.Response.Output.WIPStations.T102LeafID;
                                }
                                else
                                {
                                    _log.Error(
                                        $"[{id.ToString()}|({pokaYoke.Error.ErrCode})" +
                                        $"{pokaYoke.Error.ErrText}");
                                }
                            }
                            else
                            {
                                _log.Error(
                                    $"[{id.ToString()}|({pokaYoke.Error.ErrCode})" +
                                    $"{pokaYoke.Error.ErrText}");
                            }

                            // 如果未通过 PokaYoke 校验，直接返回
                            if (pokaYoke.Response.Output.WIPStations.Poka_Yoke_Result != 1)
                            {
                                tag.Value = false;
                                rlt.Add(tag);
                                return rlt;
                            }
                        }
                        #endregion

                        #region DMC 校验通过后，进行 DMC 的 Fazit 状态检查
                        {
                            FazitStatusCheck fazitStatusCheck =
                                new FazitStatusCheck(
                                    GlobalParams.Instance.WebAPI.URL,
                                    GlobalParams.Instance.WebAPI.ContentType,
                                    GlobalParams.Instance.WebAPI.ClientID,
                                    logEntity)
                                {
                                    Request = new FazitStatusCheckRequest()
                                    {
                                        CommunityID = GlobalParams.Instance.CommunityID,
                                        T133LeafID = device.T133LeafID,
                                        T216LeafID = device.T216LeafID,
                                        T102LeafID = wipStation.T102LeafID,
                                        T107LeafID = ReadIntValue(device, wipStation.Tags, "WIP_Station_LeafID"),
                                        WIP_Code = ReadStringValue(device, wipStation.Tags, "WIP_Code"),
                                        WIP_ID_Type_Code = ReadStringValue(device, wipStation.Tags, "WIP_ID_Type_Code"),
                                        WIP_ID_Code = ReadStringValue(device, wipStation.Tags, "WIP_ID_Code"),
                                    }
                                };

                            //DateTime start = DateTime.Now;
                            //TimeSpan timeSpan = start - start;
                            byte fazitStatus = 0xff;
                            //do
                            //{
                            if (fazitStatusCheck.Do())
                            {
                                _log.Debug(
                                    $"[{id.ToString()}|({fazitStatusCheck.Error.ErrCode})" +
                                    $"{fazitStatusCheck.Error.ErrText}");

                                switch (fazitStatusCheck.Response.Output.DMC_Fazit_Status)
                                {
                                    case 0:
                                        fazitStatus = 1;
                                        break;
                                    case 1:
                                        fazitStatus = 20;
                                        break;
                                    default:
                                        fazitStatus = 0xff;
                                        break;
                                }
                            }
                            else
                            {
                                _log.Error(
                                    $"[{id.ToString()}|({fazitStatusCheck.Error.ErrCode})" +
                                    $"{fazitStatusCheck.Error.ErrText}");
                            }

                            //    Thread.Sleep(5000);
                            //    timeSpan = DateTime.Now - start;
                            //} while (timeSpan.TotalSeconds < 300 && fazitStatus == 0xff);  // 循环执行 300 秒，以获得FazitStatus

                            if (fazitStatus == 0xff)
                            {
                                WriteTagValueBack(rlt, wipStation, "Poka_Yoke_Result", 20);
                            }
                            else
                            {
                                WriteTagValueBack(rlt, wipStation, "Poka_Yoke_Result", fazitStatus);
                            }
                        }
                        #endregion
                    }
                    catch (Exception error)
                    {
                        _log.Error(error.Message, error);
                    }

                    tag.Value = false;
                    rlt.Add(tag);

                    _log.Info("Fazit Response状态检测处理完成");
                }
            }

            return rlt;
        }
    }

    /// <summary>
    /// 料槽缺料检测请求交易
    /// </summary>
    public class IRAPDCSTradeShortageMaterialCheck : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeShortageMaterialCheck(DCSGatewayLogEntity log) : base(log)
        {
        }

        /// <summary>
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();
            logEntity.DeviceName = device.Name;

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("料槽缺料检测交易处理");
                    logEntity.ActionName = "料槽缺料检测";

                    ShortageMaterialCheck shortageMaterialCheck = null;
                    SiemensSubTagGroup wipStation = signalTag.Parent as SiemensSubTagGroup;
                    if (wipStation != null)
                    {
                        shortageMaterialCheck =
                            new ShortageMaterialCheck(
                                GlobalParams.Instance.WebAPI.URL,
                                GlobalParams.Instance.WebAPI.ContentType,
                                GlobalParams.Instance.WebAPI.ClientID,
                                logEntity)
                            {
                                Request = new ShortageMaterialCheckRequest()
                                {
                                    CommunityID = GlobalParams.Instance.CommunityID,
                                    T133LeafID = device.T133LeafID,
                                    T216LeafID = device.T216LeafID,
                                    T102LeafID = wipStation.T102LeafID,
                                    T107LeafID = device.T107LeafID,//ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
                                    WIP_Code = ReadStringValue(device, wipStation.Tags, "WIP_Code"),
                                    WIP_ID_Type_Code = ReadStringValue(device, wipStation.Tags, "WIP_ID_Type_Code"),
                                    WIP_ID_Code = ReadStringValue(device, wipStation.Tags, "WIP_ID_Code"),
                                },
                            };
                    }

                    if (shortageMaterialCheck == null)
                    {
                        _log.Error($"[{signalTag.Name}标记不在WIPStations的子组中，交易无法继续");
                        return rlt;
                    }

                    try
                    {
                        if (CallStartDCSInvoking(device, signalTag))
                        {
                            if (shortageMaterialCheck.Do())
                            {
                                if (shortageMaterialCheck.Error.ErrCode >= 0)
                                {
                                    _log.Debug(
                                        $"[{id.ToString()}|({shortageMaterialCheck.Error.ErrCode})" +
                                        $"{shortageMaterialCheck.Error.ErrText}");

                                    WriteTagValueBack(rlt, wipStation, "Poka_Yoke_Feedback_Mark", shortageMaterialCheck.Response.Output.Poka_Yoke_Feedback_Mark);
                                }
                                else
                                {
                                    _log.Error(
                                        $"[{id.ToString()}|({shortageMaterialCheck.Error.ErrCode})" +
                                        $"{shortageMaterialCheck.Error.ErrText}");
                                }
                            }
                            else
                            {
                                _log.Error(
                                    $"[{id.ToString()}|({shortageMaterialCheck.Error.ErrCode})" +
                                    $"{shortageMaterialCheck.Error.ErrText}");
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        _log.Error(error.Message, error);
                    }

                    tag.Value = false;
                    rlt.Add(tag);

                    _log.Info("料槽缺料检测处理完成");
                }
            }

            return rlt;
        }
    }

    /// <summary>
    /// 容器绑定交易
    /// </summary>
    public class IRAPDCSTradeContainerNumberBinding : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">交易日志实体对象</param>
        public IRAPDCSTradeContainerNumberBinding(DCSGatewayLogEntity log) : base(log)
        {
        }

        /// <summary>
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();
            logEntity.DeviceName = device.Name;

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("容器绑定交易处理");
                    logEntity.ActionName = "容器绑定";

                    ContainerNumberBinding containerBinding = null;
                    SiemensSubTagGroup wipStation = signalTag.Parent as SiemensSubTagGroup;
                    if (wipStation != null)
                    {
                        containerBinding =
                            new ContainerNumberBinding(
                                GlobalParams.Instance.WebAPI.URL,
                                GlobalParams.Instance.WebAPI.ContentType,
                                GlobalParams.Instance.WebAPI.ClientID,
                                logEntity)
                            {
                                Request = new ContainerNumberBindingRequest()
                                {
                                    CommunityID = GlobalParams.Instance.CommunityID,
                                    T133LeafID = device.T133LeafID,
                                    T216LeafID = device.T216LeafID,
                                    T102LeafID = wipStation.T102LeafID,
                                    T107LeafID = device.T107LeafID,//ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
                                    WIP_Code = ReadStringValue(device, wipStation.Tags, "WIP_Code"),
                                    WIP_ID_Type_Code = ReadStringValue(device, wipStation.Tags, "WIP_ID_Type_Code"),
                                    WIP_ID_Code = ReadStringValue(device, wipStation.Tags, "WIP_ID_Code"),
                                },
                            };
                        containerBinding.Request.ParamXML.Container_Number_pallet_code =
                            ReadStringValue(
                                device,
                                wipStation.Tags,
                                "Container_Number_pallet_code");
                    }

                    if (containerBinding == null)
                    {
                        _log.Error($"[{signalTag.Name}标记不在WIPStations的子组中，交易无法继续");
                        return rlt;
                    }

                    try
                    {
                        if (CallStartDCSInvoking(device, signalTag))
                        {
                            if (containerBinding.Do())
                            {
                                if (containerBinding.Error.ErrCode >= 0)
                                {
                                    _log.Debug(
                                        $"[{id.ToString()}|({containerBinding.Error.ErrCode})" +
                                        $"{containerBinding.Error.ErrText}");
                                }
                                else
                                {
                                    _log.Error(
                                        $"[{id.ToString()}|({containerBinding.Error.ErrCode})" +
                                        $"{containerBinding.Error.ErrText}");
                                }
                            }
                            else
                            {
                                _log.Error(
                                    $"[{id.ToString()}|({containerBinding.Error.ErrCode})" +
                                    $"{containerBinding.Error.ErrText}");
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        _log.Error(error.Message, error);
                    }

                    tag.Value = false;
                    rlt.Add(tag);

                    _log.Info("容器绑定处理完成");
                }
            }

            return rlt;
        }
    }
}