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
        public static IIRAPDCSTrade CreateInstance(string tradeName)
        {
            string className = $"IRAP.BL.S7Gateway.IRAPDCSTrade{tradeName}";
            IIRAPDCSTrade trade =
                (IIRAPDCSTrade)Assembly.Load("IRAP.BL.S7Gateway").CreateInstance(className);
            return trade;
        }
    }

    /// <summary>
    /// DCS交易父类
    /// </summary>
    public abstract class IRAPDCSTrade : IRAPBaseObject
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public IRAPDCSTrade()
        {
            _log = Logger.Get(GetType());
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
                    GlobalParams.Instance.WebAPI.ClientID);

            startDCSInvoking.Request =
                new StartDCSInvokingRequest()
                {
                    CommunityID = GlobalParams.Instance.CommunityID,
                    T133LeafID = device.T133LeafID,
                };

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
                return stringTag.Value;
            }
            else
            {
                return "";
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
    }

    /// <summary>
    /// 同步设备状态交易
    /// </summary>
    public class IRAPDCSTradeGetOPCStatus : IRAPDCSTrade, IIRAPDCSTrade
    {
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
                if (device.Groups["COMM"] is SiemensTagGroup comm)
                {
                    GetOPCStatus getOPCStatus =
                        new GetOPCStatus(
                            GlobalParams.Instance.WebAPI.URL,
                            GlobalParams.Instance.WebAPI.ContentType,
                            GlobalParams.Instance.WebAPI.ClientID)
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
    public class IRAPDCSTradeEquipmentRunningMode : IRAPDCSTradeGetOPCStatus { }

    /// <summary>
    /// 设备是否加电状态同步
    /// </summary>
    public class IRAPDCSTradeEquipmentPowerOn : IRAPDCSTradeGetOPCStatus { }

    /// <summary>
    /// 设备是否失效状态同步
    /// </summary>
    public class IRAPDCSTradeEquipmentFail : IRAPDCSTradeGetOPCStatus { }

    /// <summary>
    /// 工装是否失效状态同步
    /// </summary>
    public class IRAPDCSTradeToolFail : IRAPDCSTradeGetOPCStatus { }

    /// <summary>
    /// 工序循环是否开始状态同步
    /// </summary>
    public class IRAPDCSTradeCycleStarted : IRAPDCSTradeGetOPCStatus { }

    /// <summary>
    /// 设备饥饿状态同步
    /// </summary>
    public class IRAPDCSTradeEquipmentStarvation : IRAPDCSTradeGetOPCStatus { }

    /// <summary>
    /// 标识部件绑定交易
    /// </summary>
    public class IRAPDCSTradeRequestForIDBinding : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("标识部件绑定");

                    IDBinding idBinding = null;
                    SiemensSubTagGroup subTagGroup = signalTag.Parent as SiemensSubTagGroup;
                    if (subTagGroup != null)
                    {
                        idBinding =
                            new IDBinding(
                                GlobalParams.Instance.WebAPI.URL,
                                GlobalParams.Instance.WebAPI.ContentType,
                                GlobalParams.Instance.WebAPI.ClientID)
                            {
                                Request = new IDBindingRequest()
                                {
                                    CommunityID = GlobalParams.Instance.CommunityID,
                                    T133LeafID = device.T133LeafID,
                                    T216LeafID = device.T216LeafID,
                                    T107LeafID = ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
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

                        if (CallStartDCSInvoking(device, signalTag))
                        {
                            idBinding.Request.ParamXML.Product_Number = ReadStringValue(device, idbGroup.Tags, "Product_Number");
                            idBinding.Request.ParamXML.ID_Part_Number = ReadStringValue(device, idbGroup.Tags, "ID_Part_Number");
                            idBinding.Request.ParamXML.ID_Part_WIP_Code = ReadStringValue(device, idbGroup.Tags, "ID_Part_WIP_Code");
                            idBinding.Request.ParamXML.ID_Part_SN_Scanner_Code = ReadStringValue(device, idbGroup.Tags, "ID_Part_SN_Scanner_Code");
                            idBinding.Request.ParamXML.Sequence_Number = ReadIntValue(device, idbGroup.Tags, "Sequence_Number");

                            if (idBinding.Do())
                            {
                                if (idBinding.Error.ErrCode == 0)
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
                                        rltTag.Value = (short)idBinding.Response.Part_Number_Feedback;
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
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("申请产品序列号");

                    string key = "SNRequest";
                    SiemensSubTagGroup subTagGroup = signalTag.Parent as SiemensSubTagGroup;
                    if (device.Groups[key] is SiemensTagGroup snrGroup)
                    {
                        if (CallStartDCSInvoking(device, signalTag))
                        {
                            SNRequest snRequest =
                                new SNRequest(
                                    GlobalParams.Instance.WebAPI.URL,
                                    GlobalParams.Instance.WebAPI.ContentType,
                                    GlobalParams.Instance.WebAPI.ClientID)
                                {
                                    Request = new SNRequestRequest()
                                    {
                                        CommunityID = GlobalParams.Instance.CommunityID,
                                        T133LeafID = device.T133LeafID,
                                        T216LeafID = device.T216LeafID,
                                        T107LeafID = ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
                                        ParamXML = new SNRequestParamXML()
                                        {
                                            Product_Number = ReadStringValue(device, snrGroup.Tags, "Product_Number"),
                                        },
                                    },
                                };

                            if (snRequest.Do())
                            {
                                if (snRequest.Error.ErrCode == 0)
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
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("工件入站交易处理");

                    WIPMoveIn wipMoveIn = null;
                    SiemensSubTagGroup subTagGroup = signalTag.Parent as SiemensSubTagGroup;
                    if (subTagGroup != null)
                    {
                        wipMoveIn =
                            new WIPMoveIn(
                                GlobalParams.Instance.WebAPI.URL,
                                GlobalParams.Instance.WebAPI.ContentType,
                                GlobalParams.Instance.WebAPI.ClientID)
                            {
                                Request = new WIPMoveInRequest()
                                {
                                    CommunityID = GlobalParams.Instance.CommunityID,
                                    T133LeafID = device.T133LeafID,
                                    T216LeafID = device.T216LeafID,
                                    T107LeafID = ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
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

                    if (CallStartDCSInvoking(device, signalTag))
                    {
                        if (wipMoveIn.Do())
                        {
                            if (wipMoveIn.Error.ErrCode == 0)
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
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("生产结束交易处理");

                    ProductionEnd productionEnd = null;
                    SiemensSubTagGroup subTagGroup = signalTag.Parent as SiemensSubTagGroup;
                    if (subTagGroup != null)
                    {
                        productionEnd =
                            new ProductionEnd(
                                GlobalParams.Instance.WebAPI.URL,
                                GlobalParams.Instance.WebAPI.ContentType,
                                GlobalParams.Instance.WebAPI.ClientID)
                            {
                                Request = new ProductionEndRequest()
                                {
                                    CommunityID = GlobalParams.Instance.CommunityID,
                                    T133LeafID = device.T133LeafID,
                                    T216LeafID = device.T216LeafID,
                                    T107LeafID = ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
                                    WIP_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_Code"),
                                    WIP_ID_Type_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_ID_Type_Code"),
                                    WIP_ID_Code = ReadStringValue(device, subTagGroup.Tags, "WIP_ID_Code"),
                                },
                            };

                        #region 填充 RECIPE 组
                        if (device.Groups["RECIPE"] is SiemensTagGroup recipeGroup)
                        {
                            foreach (SiemensTag recipeTag in recipeGroup.Tags)
                            {
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

                    if (CallStartDCSInvoking(device, signalTag))
                    {
                        if (productionEnd.Do())
                        {
                            if (productionEnd.Error.ErrCode == 0)
                            {
                                _log.Debug(
                                    $"[{id.ToString()}|({productionEnd.Error.ErrCode})" +
                                    $"{productionEnd.Error.ErrText}");

                                var writeTag = subTagGroup.Tags["Poka_Yoke_Result"];
                                if (writeTag != null)
                                {
                                    if (writeTag is SiemensByteOfTag rltTag)
                                    {
                                        rltTag.Value = productionEnd.Response.Poka_Yoke_Result;
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
    public class IRAPDCSTradeWIPMoveout : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 交易执行
        /// </summary>
        /// <param name="device">Tag对象所属Device对象</param>
        /// <param name="signalTag">信号Tag对象</param>
        /// <returns>待回写到PLC的标记列表</returns>
        public List<SiemensTag> Do(SiemensDevice device, SiemensTag signalTag)
        {
            List<SiemensTag> rlt = new List<SiemensTag>();

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("工件离站交易处理");

                    OperationCycleEnd operationCycleEnd = null;
                    SiemensSubTagGroup subTagGroup = signalTag.Parent as SiemensSubTagGroup;
                    if (subTagGroup != null)
                    {
                        operationCycleEnd =
                            new OperationCycleEnd(
                                GlobalParams.Instance.WebAPI.URL,
                                GlobalParams.Instance.WebAPI.ContentType,
                                GlobalParams.Instance.WebAPI.ClientID)
                            {
                                Request = new OperationCycleEndRequest()
                                {
                                    CommunityID = GlobalParams.Instance.CommunityID,
                                    T133LeafID = device.T133LeafID,
                                    T216LeafID = device.T216LeafID,
                                    T107LeafID = ReadIntValue(device, subTagGroup.Tags, "WIP_Station_LeafID"),
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

                    if (CallStartDCSInvoking(device, signalTag))
                    {
                        if (operationCycleEnd.Do())
                        {
                            if (operationCycleEnd.Error.ErrCode == 0)
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

                    tag.Value = false;
                    rlt.Add(tag);

                    _log.Info("工件离站处理完成");
                }
            }

            return rlt;
        }
    }
}