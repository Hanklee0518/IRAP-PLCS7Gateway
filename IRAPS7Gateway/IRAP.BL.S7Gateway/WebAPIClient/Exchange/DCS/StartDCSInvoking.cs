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
    /// StartDCSInvoking交易请求报文
    /// </summary>
    public class StartDCSInvokingRequest : CustomRequest
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public override string ExCode => "IRAP_DCS_StartDCSInvoking";
        /// <summary>
        /// 设备叶标识
        /// </summary>
        public int T133LeafID { get; set; }
    }

    /// <summary>
    /// StartDCSInvoking交易响应报文
    /// </summary>
    public class StartDCSInvokingResponse : CustomResponse
    {

    }

    /// <summary>
    /// StartDCSInvoking交易
    /// </summary>
    public class StartDCSInvoking : CustomWebAPICall
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="webAPIUrl">WebAPI地址</param>
        /// <param name="contentType">报文类型</param>
        /// <param name="clientID">渠道标识</param>
        public StartDCSInvoking(
            string webAPIUrl, 
            ContentType contentType, 
            string clientID) : base(webAPIUrl, contentType, clientID)
        {
            moduleType = ModuleType.Exchange;
            exCode = "IRAP_DCS_StartDCSInvoking";
        }

        /// <summary>
        /// 请求报文
        /// </summary>
        public StartDCSInvokingRequest Request { get; set; } = null;
        /// <summary>
        /// 响应报文
        /// </summary>
        public StartDCSInvokingResponse Response { get; private set; } = null;

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
                        "请求报文[StartDCSInvokingRequest]未实例化");
                errorMsg =
                    new ErrorMessage()
                    {
                        ErrCode = 999999,
                        ErrText = error.Message,
                    };
                return;
            }

            Response =
                Call<StartDCSInvokingResponse>(Request, out errorMsg);
        }
    }
}
