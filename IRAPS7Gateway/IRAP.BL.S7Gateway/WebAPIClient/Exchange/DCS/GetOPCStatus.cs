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
    /// 同步设备状态请求报文
    /// </summary>
    public class GetOPCStatusRequest : CustomPLCRequest
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public override string ExCode => "IRAP_DCS_GetOPCStatus";
        /// <summary>
        /// 标记组对象
        /// </summary>
        public GetOPCStatusParamXML ParamXML { get; set; } =
            new GetOPCStatusParamXML();
    }

    /// <summary>
    /// 同步设备状态的标记组
    /// </summary>
    public class GetOPCStatusParamXML
    {
        /// <summary>
        /// 设备运行模式
        /// </summary>
        public bool Equipment_Running_Mode { get; set; }
        /// <summary>
        /// 设备是否加电
        /// </summary>
        public bool Equipment_Power_On { get; set; }
        /// <summary>
        /// 设备是否失效
        /// </summary>
        public bool Equipment_Fail { get; set; }
        /// <summary>
        /// 工装是否失效
        /// </summary>
        public bool Tool_Fail { get; set; }
        /// <summary>
        /// 工序循环是否开始
        /// </summary>
        public bool Cycle_Started { get; set; }
        /// <summary>
        /// 设备饥饿状态
        /// </summary>
        public bool Equipment_Starvation { get; set; }
    }

    /// <summary>
    /// 同步设备状态响应报文
    /// </summary>
    public class GetOPCStatusResponse : CustomResponse
    {

    }

    /// <summary>
    /// 同步设备状态
    /// </summary>
    public class GetOPCStatus : CustomWebAPICall
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="webAPIUrl">WebAPI地址</param>
        /// <param name="contentType">报文类型</param>
        /// <param name="clientID">渠道标识</param>
        public GetOPCStatus(
            string webAPIUrl, 
            ContentType contentType, 
            string clientID) : base(webAPIUrl, contentType, clientID)
        {
            moduleType = ModuleType.Exchange;
            exCode = "IRAP_DCS_GetOPCStatus";
        }

        /// <summary>
        /// 请求报文
        /// </summary>
        public GetOPCStatusRequest Request { get; set; } = null;
        /// <summary>
        /// 响应报文
        /// </summary>
        public GetOPCStatusResponse Response { get; private set; } = null;

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
                        "请求报文[GetOPCStatusRequest]未实例化");
                errorMsg =
                    new ErrorMessage()
                    {
                        ErrCode = 999999,
                        ErrText = error.Message,
                    };
                return;
            }

            Response =
                Call<GetOPCStatusResponse>(Request, out errorMsg);
        }
    }
}
