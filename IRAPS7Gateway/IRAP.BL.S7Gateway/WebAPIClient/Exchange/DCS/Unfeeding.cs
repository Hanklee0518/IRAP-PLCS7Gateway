using IRAP.BL.S7Gateway.Utils;
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
    /// 料槽卸料请求报文
    /// </summary>
    public class UnfeedingRequest : CustomPLCRequest
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public override string ExCode => "IRAP_DCS_Unfeeding";
        /// <summary>
        /// 料槽卸料请求报文体
        /// </summary>
        public UnfeedingParamXML ParamXML { get; set; } =
            new UnfeedingParamXML();
    }

    /// <summary>
    /// 料槽卸料请求报文体
    /// </summary>
    public class UnfeedingParamXML
    {
        /// <summary>
        /// 物料追溯标识
        /// </summary>
        public string Material_Track_ID { get; set; } = "";
        /// <summary>
        /// 料槽编号
        /// </summary>
        public string Slot_Number { get; set; } = "";
        /// <summary>
        /// 卸料数量
        /// </summary>
        public uint Unfeeding_Quantity { get; set; } = 0;
    }

    /// <summary>
    /// 料槽卸料响应报文
    /// </summary>
    public class UnfeedingResponse : CustomResponse
    {

    }

    /// <summary>
    /// 料槽卸料
    /// </summary>
    public class Unfeeding : CustomWebAPICall
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="webAPIUrl">WebAPI地址</param>
        /// <param name="contentType">报文类型</param>
        /// <param name="clientID">渠道标识</param>
        /// <param name="logEntity">交易日志实体对象</param>
        public Unfeeding(
            string webAPIUrl,
            ContentType contentType,
            string clientID,
            DCSGatewayLogEntity logEntity) :
            base(webAPIUrl, contentType, clientID, logEntity)
        {
            moduleType = ModuleType.Exchange;
            ExCode = "IRAP_DCS_Unfeeding";
        }

        /// <summary>
        /// 请求报文
        /// </summary>
        public UnfeedingRequest Request { get; set; } = null;
        /// <summary>
        /// 响应报文
        /// </summary>
        public UnfeedingResponse Response { get; private set; } = null;

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
                        "请求报文[UnfeedingRequest]未实例化");
                errorMsg =
                    new ErrorMessage()
                    {
                        ErrCode = 999999,
                        ErrText = error.Message,
                    };
                return;
            }

            Response =
                Call<UnfeedingResponse>(Request, out errorMsg);
        }
    }
}
