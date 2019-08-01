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
    /// 容器绑定请求报文
    /// </summary>
    public class ContainerNumberBindingRequest : CustomPLCRequest
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public override string ExCode => "IRAP_DCS_ContainerNumberBinding";
        /// <summary>
        /// 请求报文体
        /// </summary>
        public ContainerNumberBindingParamXML ParamXML { get; set; } =
            new ContainerNumberBindingParamXML();
    }

    /// <summary>
    /// 容器绑定请求报文报文体
    /// </summary>
    public class ContainerNumberBindingParamXML
    {
        /// <summary>
        /// 容器编号（工装板编号）
        /// </summary>
        public string Container_Number_pallet_code { get; set; }
    }

    /// <summary>
    /// 容器绑定响应报文
    /// </summary>
    public class ContainerNumberBindingResponse : CustomResponse
    {

    }

    /// <summary>
    /// 容器绑定
    /// </summary>
    public class ContainerNumberBinding : CustomWebAPICall
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="webAPIUrl">WebAPI地址</param>
        /// <param name="contentType">报文类型</param>
        /// <param name="clientID">渠道标识</param>
        public ContainerNumberBinding(
            string webAPIUrl,
            ContentType contentType,
            string clientID) : base(webAPIUrl, contentType, clientID)
        {
            moduleType = ModuleType.Exchange;
            exCode = "IRAP_DCS_ContainerNumberBinding";
        }

        /// <summary>
        /// 请求报文
        /// </summary>
        public ContainerNumberBindingRequest Request { get; set; } = null;
        /// <summary>
        /// 响应报文
        /// </summary>
        public ContainerNumberBindingResponse Response { get; private set; } = null;

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
                        "请求报文[ContainerNumberBindingRequest]未实例化");
                errorMsg =
                    new ErrorMessage()
                    {
                        ErrCode = 999999,
                        ErrText = error.Message,
                    };
                return;
            }

            Response =
                Call<ContainerNumberBindingResponse>(Request, out errorMsg);
        }
    }
}
