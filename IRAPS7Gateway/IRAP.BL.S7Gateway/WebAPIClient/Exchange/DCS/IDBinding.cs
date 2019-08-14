using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAP.BL.S7Gateway.WebAPIClient.Contents;
using IRAP.BL.S7Gateway.WebAPIClient.Enums;

namespace IRAP.BL.S7Gateway.WebAPIClient.Exchange.DCS
{
    /// <summary>
    /// 标识部件绑定请求报文
    /// </summary>
    public class IDBindingRequest : CustomPLCRequest
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public override string ExCode => "IRAP_DCS_IDBinding";
        /// <summary>
        /// 标记组对象
        /// </summary>
        public IDBindingParamXML ParamXML { get; set; } =
            new IDBindingParamXML();
    }

    /// <summary>
    /// 标识部件绑定交易的标识组
    /// </summary>
    public class IDBindingParamXML
    {
        /// <summary>
        /// 生产产品的产品编号
        /// </summary>
        public string Product_Number { get; set; }
        /// <summary>
        /// 标识部件编号
        /// </summary>
        public string ID_Part_Number { get; set; }
        /// <summary>
        /// 标识部件主标识
        /// </summary>
        public string ID_Part_WIP_Code { get; set; }
        /// <summary>
        /// 标识部件序列号
        /// </summary>
        public string ID_Part_SN_Scanner_Code { get; set; }
        /// <summary>
        /// 标识部件序号
        /// </summary>
        public int Sequence_Number { get; set; }
    }

    /// <summary>
    /// 标识部件绑定响应报文
    /// </summary>
    public class IDBindingResponse : CustomResponse
    {
        /// <summary>
        /// 输出参数
        /// </summary>
        public IDBindingOutput Output { get; set; } =
            new IDBindingOutput();
    }

    /// <summary>
    /// 标识部件绑定响应报文输出参数
    /// </summary>
    public class IDBindingOutput
    {
        /// <summary>
        /// MES 返回的标识部件序号
        /// </summary>
        public int Part_Number_Feedback { get; set; }
    }

    /// <summary>
    /// 标识部件绑定（IDBinding）
    /// </summary>
    public class IDBinding : CustomWebAPICall
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="webAPIUrl">WebAPI地址</param>
        /// <param name="contentType">报文类型</param>
        /// <param name="clientID">渠道标识</param>
        public IDBinding(
            string webAPIUrl, 
            ContentType contentType, 
            string clientID) : base(webAPIUrl, contentType, clientID)
        {
            moduleType = ModuleType.Exchange;
            exCode = "IRAP_DCS_IDBinding";
        }

        /// <summary>
        /// 请求报文
        /// </summary>
        public IDBindingRequest Request { get; set; } = null;
        /// <summary>
        /// 响应报文
        /// </summary>
        public IDBindingResponse Response { get; private set; } = null;

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
                        "请求报文[IDBindingRequest]未实例化");
                errorMsg =
                    new ErrorMessage()
                    {
                        ErrCode = 999999,
                        ErrText = error.Message,
                    };
                return;
            }

            Response =
                Call<IDBindingResponse>(Request, out errorMsg);
        }
    }
}
