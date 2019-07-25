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
    /// 工件入站请求报文
    /// </summary>
    public class WIPMoveInRequest : CustomPLCRequest
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public override string ExCode => "IRAP_DCS_WIPMoveIn";
    }

    /// <summary>
    /// 工件入站响应报文
    /// </summary>
    public class WIPMoveInResponse : CustomResponse
    {

    }

    /// <summary>
    /// 工件入站
    /// </summary>
    public class WIPMoveIn : CustomWebAPICall
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="webAPIUrl">WebAPI地址</param>
        /// <param name="contentType">报文类型</param>
        /// <param name="clientID">渠道标识</param>
        public WIPMoveIn(
            string webAPIUrl, 
            ContentType contentType, 
            string clientID) : base(webAPIUrl, contentType, clientID)
        {
            moduleType = ModuleType.Exchange;
            exCode = "IRAP_DCS_WIPMoveIn";
        }

        /// <summary>
        /// 请求报文
        /// </summary>
        public WIPMoveInRequest Request { get; set; } = null;
        /// <summary>
        /// 响应报文
        /// </summary>
        public WIPMoveInResponse Response { get; private set; } = null;

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
                        "请求报文[WIPMoveInRequest]未实例化");
                errorMsg =
                    new ErrorMessage()
                    {
                        ErrCode = 999999,
                        ErrText = error.Message,
                    };
                return;
            }

            Response =
                Call<WIPMoveInResponse>(Request, out errorMsg);
        }
    }
}
