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
    /// 请求标签元素请求报文
    /// </summary>
    public class LBLElementRequest : CustomPLCRequest
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public override string ExCode => "IRAP_DCS_LBLElement";
    }

    /// <summary>
    /// 请求标签元素响应报文
    /// </summary>
    public class LBLElementResponse : CustomResponse
    {
        /// <summary>
        /// 输出参数
        /// </summary>
        public LBLElementOutput Output { get; set; } =
            new LBLElementOutput();
    }

    /// <summary>
    /// 请求标签元素响应报文输出参数
    /// </summary>
    public class LBLElementOutput
    {
        /// <summary>
        /// 客户部件编号
        /// </summary>
        public string Customer_Part_Number { get; set; }
        /// <summary>
        /// 产品序列号
        /// </summary>
        public string Product_Serial_Number { get; set; }
        /// <summary>
        /// 产品型号标识
        /// </summary>
        public string Model_ID { get; set; }
        /// <summary>
        /// 客户赋予的供应商代码
        /// </summary>
        public string Vendor_Code_Of_Us { get; set; }
        /// <summary>
        /// 代理商部件编号
        /// </summary>
        public string Sales_Part_Number { get; set; }
        /// <summary>
        /// 硬件版本号
        /// </summary>
        public string Hardware_Version { get; set; }
        /// <summary>
        /// 软件版本号
        /// </summary>
        public string Software_Version { get; set; }
        /// <summary>
        /// 生产批次号
        /// </summary>
        public string Lot_Number { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public string MFG_Date { get; set; }
        /// <summary>
        /// 班次编号
        /// </summary>
        public string Shift_Number { get; set; }
        /// <summary>
        /// 炉次号
        /// </summary>
        public string Oven_Number { get; set; }
        /// <summary>
        /// 客户邓白氏码
        /// </summary>
        public string Customer_Duns_Code { get; set; }
        /// <summary>
        /// OEM品牌
        /// </summary>
        public string OEM_Brand { get; set; }
        /// <summary>
        /// 自定义固定字串1
        /// </summary>
        public string Fixed_String_1 { get; set; }
        /// <summary>
        /// 自定义固定字串2
        /// </summary>
        public string Fixed_String_2 { get; set; }
        /// <summary>
        /// 衍生字串1
        /// </summary>
        public string Derived_String_1 { get; set; }
        /// <summary>
        /// 衍生字串2
        /// </summary>
        public string Derived_String_2 { get; set; }
    }

    /// <summary>
    /// 请求标签元素交易
    /// </summary>
    public class LBLElement : CustomWebAPICall
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="webAPIUrl">WebAPI地址</param>
        /// <param name="contentType">报文类型</param>
        /// <param name="clientID">渠道标识</param>
        public LBLElement(
            string webAPIUrl,
            ContentType contentType,
            string clientID) : base(webAPIUrl, contentType, clientID)
        {
            moduleType = ModuleType.Exchange;
            exCode = "IRAP_DCS_LBLElement";
        }

        /// <summary>
        /// 请求报文
        /// </summary>
        public LBLElementRequest Request { get; set; } = null;
        /// <summary>
        /// 响应报文
        /// </summary>
        public LBLElementResponse Response { get; private set; } = null;

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
                        "请求报文[LBLElementRequest]未实例化");
                errorMsg =
                    new ErrorMessage()
                    {
                        ErrCode = 999999,
                        ErrText = error.Message,
                    };
                return;
            }

            Response =
                Call<LBLElementResponse>(Request, out errorMsg);
        }
    }
}
