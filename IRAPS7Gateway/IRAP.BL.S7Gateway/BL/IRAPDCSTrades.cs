using IRAP.BL.S7Gateway.Entities;
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
        /// <returns></returns>
        public static IIRAPDCSTrade CreateInstance(string tagName)
        {
            string className = $"IRAP.BL.S7Gateway.IRAPDCSTrade{tagName}";
            IIRAPDCSTrade trade =
                (IIRAPDCSTrade)Assembly.Load("IRAP.BL.S7Gateway").CreateInstance(className);
            return trade;
        }
    }

    public abstract class IRAPDCSTrade : IRAPBaseObject
    {
    }

    /// <summary>
    /// 工件入站交易
    /// </summary>
    public class IRAPDCSTradeWIPMoveIn : IRAPDCSTrade, IIRAPDCSTrade
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public IRAPDCSTradeWIPMoveIn()
        {
            _log = Logger.Get<IRAPDCSTradeWIPMoveIn>();
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

            if (signalTag is SiemensBoolOfTag)
            {
                SiemensBoolOfTag tag = signalTag as SiemensBoolOfTag;
                if (tag.Value)
                {
                    _log.Info("工件入站交易处理");

                    string key = "";
                    CustomGroup parent = signalTag.Parent;
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

                    CustomTag wipCode = device.FindTag(key, "WIP_Code");
                    if (wipCode != null)
                    {
                        device.ReadTagValue(wipCode as SiemensTag);
                        _log.Debug(
                            $"{device.Name}->{wipCode.Name}->{(wipCode as SiemensArrayCharOfTag).Value}");
                    }

                    tag.Value = false;
                    rlt.Add(tag);
                }
            }

            return rlt;
        }
    }
}