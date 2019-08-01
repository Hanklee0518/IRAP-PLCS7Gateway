using IRAP.BL.S7Gateway.WebAPIClient.Contents;
using IRAP.BL.S7Gateway.WebAPIClient.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAP.BL.S7Gateway.WebAPIClient.Exchange.DCS
{
    /// <summary>
    /// 停滞超时告警请求报文
    /// </summary>
    public class StagnationWarnningRequest : CustomPLCRequest
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public override string ExCode => "IRAP_DCS_StagnationWarnning";
        /// <summary>
        /// 停滞超时告警请求报文体
        /// </summary>
        public StagnationWarnningParamXML ParamXML { get; set; } =
            new StagnationWarnningParamXML();
    }

    /// <summary>
    /// 停滞超时告警请求报文体
    /// </summary>
    public class StagnationWarnningParamXML
    {
        /// <summary>
        /// 停滞时间(s)
        /// </summary>
        public uint Time_In_Seconds { get; set; }
        /// <summary>
        /// 告警阀门值(s)
        /// </summary>
        public ushort Threshold { get; set; }
    }

    /// <summary>
    /// 停滞超时告警响应报文
    /// </summary>
    public class StagnationWarnningResponse : CustomResponse { }

    /// <summary>
    /// 停滞超时告警
    /// </summary>
    public class StagnationWarnning : CustomWebAPICall
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="webAPIUrl">WebAPI地址</param>
        /// <param name="contentType">报文类型</param>
        /// <param name="clientID">渠道标识</param>
        public StagnationWarnning(
            string webAPIUrl,
            ContentType contentType,
            string clientID) : base(webAPIUrl, contentType, clientID)
        {
            moduleType = ModuleType.Exchange;
            exCode = "IRAP_DCS_StagnationWarnning";
        }

        /// <summary>
        /// 请求报文
        /// </summary>
        public StagnationWarnningRequest Request { get; set; } = null;
        /// <summary>
        /// 响应报文
        /// </summary>
        public StagnationWarnningResponse Response { get; private set; } = null;

        /// <summary>
        /// 发送请求报文并接受响应报文
        /// </summary>
        /// <param name="errorMsg">交易执行结果对象</param>
        protected override void Communicate(out ErrorMessage errorMsg)
        {
            if (Request == null)
            {
                Exception error =
                    new Exception(
                        "请求报文[StagnationWarnningRequest]未实例化");
                errorMsg =
                    new ErrorMessage()
                    {
                        ErrCode = 999999,
                        ErrText = error.Message,
                    };
                return;
            }

            Response =
                Call<StagnationWarnningResponse>(Request, out errorMsg);
        }
    }
}
