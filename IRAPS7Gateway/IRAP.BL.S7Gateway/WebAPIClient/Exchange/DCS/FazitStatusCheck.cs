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
    /// Fazit状态检测请求报文
    /// </summary>
    public class FazitStatusCheckRequest : CustomPLCRequest
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public override string ExCode => "IRAP_DCS_FazitStatusCheck";
    }

    /// <summary>
    /// Fazit状态检测响应报文
    /// </summary>
    public class FazitStatusCheckResponse : CustomResponse
    {
        /// <summary>
        /// Fazit状态检测响应报文体
        /// </summary>
        public FazitStatusCheckOutput Output { get; set; } =
            new FazitStatusCheckOutput();
    }

    /// <summary>
    /// Fazit状态检测响应报文体
    /// </summary>
    public class FazitStatusCheckOutput
    {
        /// <summary>
        /// Fazit状态
        /// </summary>
        public byte DMC_Fazit_Status { get; set; }
        /// <summary>
        /// Fazit注册请求时间
        /// </summary>
        public string Fazit_Request_Time { get; set; }
    }

    /// <summary>
    /// Fazit状态检测
    /// </summary>
    public class FazitStatusCheck : CustomWebAPICall
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="webAPIUrl">WebAPI地址</param>
        /// <param name="contentType">报文类型</param>
        /// <param name="clientID">渠道标识</param>
        /// <param name="logEntity">交易日志实体对象</param>
        public FazitStatusCheck(
            string webAPIUrl,
            ContentType contentType,
            string clientID,
            DCSGatewayLogEntity logEntity) : 
            base(webAPIUrl, contentType, clientID, logEntity)
        {
            moduleType = ModuleType.Exchange;
            ExCode = "IRAP_DCS_FazitStatusCheck";
        }

        /// <summary>
        /// 请求报文
        /// </summary>
        public FazitStatusCheckRequest Request { get; set; } = null;
        /// <summary>
        /// 响应报文
        /// </summary>
        public FazitStatusCheckResponse Response { get; private set; } = null;

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
                        "请求报文[FazitStatusCheckRequest]未实例化");
                errorMsg =
                    new ErrorMessage()
                    {
                        ErrCode = 999999,
                        ErrText = error.Message,
                    };
                return;
            }

            Response =
                Call<FazitStatusCheckResponse>(Request, out errorMsg);
        }
    }
}
