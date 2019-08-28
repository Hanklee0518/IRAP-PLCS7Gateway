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
    /// 序列请求请求报文
    /// </summary>
    public class SNRequestRequest : CustomPLCRequest
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public override string ExCode => "IRAP_DCS_SNRequest";
        /// <summary>
        /// 输入参数对象
        /// </summary>
        public SNRequestParamXML ParamXML { get; set; } =
            new SNRequestParamXML();
    }

    /// <summary>
    /// 序列请求交易的输入参数
    /// </summary>
    public class SNRequestParamXML
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        public string Product_Number { get; set; } = "";
    }

    /// <summary>
    /// 序列请求交易的响应报文
    /// </summary>
    public class SNRequestResponse : CustomResponse
    {
        /// <summary>
        /// 输出参数
        /// </summary>
        public SNRequestOutput Output { get; set; } =
            new SNRequestOutput();
    }

    /// <summary>
    /// 序列请求交易的输出参数
    /// </summary>
    public class SNRequestOutput
    {
        /// <summary>
        /// 产品序列号
        /// </summary>
        public string Serial_Number { get; set; } = "";
    }

    /// <summary>
    /// 序列请求（SNRequest）
    /// </summary>
    public class SNRequest : CustomWebAPICall
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="webAPIUrl">WebAPI地址</param>
        /// <param name="contentType">报文类型</param>
        /// <param name="clientID">渠道标识</param>
        /// <param name="logEntity">交易日志实体对象</param>
        public SNRequest(
            string webAPIUrl,
            ContentType contentType,
            string clientID,
            DCSGatewayLogEntity logEntity) : 
            base(webAPIUrl, contentType, clientID, logEntity)
        {
            moduleType = ModuleType.Exchange;
            ExCode = "IRAP_DCS_SNRequest";
        }

        /// <summary>
        /// 请求报文
        /// </summary>
        public SNRequestRequest Request { get; set; } = null;
        /// <summary>
        /// 响应报文
        /// </summary>
        public SNRequestResponse Response { get; private set; } = null;

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
                        "请求报文[SNRequestRequest]未实例化");
                errorMsg =
                    new ErrorMessage()
                    {
                        ErrCode = 999999,
                        ErrText = error.Message,
                    };
                return;
            }

            Response =
                Call<SNRequestResponse>(Request, out errorMsg);
        }
    }
}
