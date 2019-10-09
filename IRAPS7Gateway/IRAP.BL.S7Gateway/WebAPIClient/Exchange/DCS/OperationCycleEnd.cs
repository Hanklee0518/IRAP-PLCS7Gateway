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
    /// 工件离站请求报文
    /// </summary>
    public class OperationCycleEndRequest : CustomPLCRequest
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public override string ExCode => "IRAP_DCS_OperationCycleEnd";
    }

    /// <summary>
    /// 工件离站响应报文
    /// </summary>
    public class OperationCycleEndResponse : CustomResponse { }

    /// <summary>
    /// 工件离站
    /// </summary>
    public class OperationCycleEnd : CustomWebAPICall
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="webAPIUrl">WebAPI地址</param>
        /// <param name="contentType">报文类型</param>
        /// <param name="clientID">渠道标识</param>
        /// <param name="logEntity">交易日志实体对象</param>
        public OperationCycleEnd(
            string webAPIUrl, 
            ContentType contentType, 
            string clientID,
            DCSGatewayLogEntity logEntity) : 
            base(webAPIUrl, contentType, clientID, logEntity)
        {
            moduleType = ModuleType.Exchange;
            ExCode = "IRAP_DCS_OperationCycleEnd";
        }

        /// <summary>
        /// 请求报文
        /// </summary>
        public OperationCycleEndRequest Request { get; set; } = null;
        /// <summary>
        /// 响应报文
        /// </summary>
        public OperationCycleEndResponse Response { get; private set; } = null;

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
                        "请求报文[OperationCycleEndRequest]未实例化");
                errorMsg =
                    new ErrorMessage()
                    {
                        ErrCode = 999999,
                        ErrText = error.Message,
                    };
                return;
            }

            Response =
                Call<OperationCycleEndResponse>(Request, out errorMsg);
        }
    }
}
