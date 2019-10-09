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
    /// 料槽加料防错请求报文
    /// </summary>
    public class PokaYokeFeedingRequest : CustomPLCRequest
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public override string ExCode => "IRAP_DCS_PokaYokeFeeding";
        /// <summary>
        /// 料槽加料防错请求报文体
        /// </summary>
        public PokaYokeFeedingParamXML ParamXML { get; set; } =
            new PokaYokeFeedingParamXML();
    }

    /// <summary>
    /// 料槽加料防错请求报文体
    /// </summary>
    public class PokaYokeFeedingParamXML
    {
        /// <summary>
        /// 物料追溯标识
        /// </summary>
        public string Material_Track_ID { get; set; } = "";
        /// <summary>
        /// 料槽编号
        /// </summary>
        public string Slot_Number { get; set; } = "";
    }

    /// <summary>
    /// 料槽加料防错响应报文
    /// </summary>
    public class PokaYokeFeedingResponse : CustomResponse
    {
        /// <summary>
        /// 料槽加料防错响应报文体
        /// </summary>
        public PokaYokeFeedingOutput Output { get; set; } =
            new PokaYokeFeedingOutput();
    }

    /// <summary>
    /// 料槽加料防错响应报文体
    /// </summary>
    public class PokaYokeFeedingOutput
    {
        /// <summary>
        /// 防错结果：1-通过；2-不通过
        /// </summary>
        public uint Poka_Yoke_Result { get; set; } = 0;
    }

    /// <summary>
    /// 料槽加料防错
    /// </summary>
    public class PokaYokeFeeding : CustomWebAPICall
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="webAPIUrl">WebAPI地址</param>
        /// <param name="contentType">报文类型</param>
        /// <param name="clientID">渠道标识</param>
        /// <param name="logEntity">交易日志实体对象</param>
        public PokaYokeFeeding(
            string webAPIUrl,
            ContentType contentType,
            string clientID,
            DCSGatewayLogEntity logEntity) :
            base(webAPIUrl, contentType, clientID, logEntity)
        {
            moduleType = ModuleType.Exchange;
            ExCode = "IRAP_DCS_PokaYokeFeeding";
        }

        /// <summary>
        /// 请求报文
        /// </summary>
        public PokaYokeFeedingRequest Request { get; set; } = null;
        /// <summary>
        /// 响应报文
        /// </summary>
        public PokaYokeFeedingResponse Response { get; private set; } = null;

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
                        "请求报文[PokaYokeFeedingRequest]未实例化");
                errorMsg =
                    new ErrorMessage()
                    {
                        ErrCode = 999999,
                        ErrText = error.Message,
                    };
                return;
            }

            Response =
                Call<PokaYokeFeedingResponse>(Request, out errorMsg);
        }
    }
}
