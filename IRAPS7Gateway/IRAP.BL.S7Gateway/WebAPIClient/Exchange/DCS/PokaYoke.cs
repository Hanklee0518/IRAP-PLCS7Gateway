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
    /// 防错验证请求报文
    /// </summary>
    public class PokaYokeRequest : CustomPLCRequest
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public override string ExCode => "IRAP_DCS_PokaYoke";
        /// <summary>
        /// 防错验证请求报文体
        /// </summary>
        public PokaYokeParamXML ParamXML { get; set; } = null;
    }

    /// <summary>
    /// 防错验证请求报文体
    /// </summary>
    public class PokaYokeParamXML
    {
        /// <summary>
        /// 工件来源标识
        /// </summary>
        public string WIP_Src_Code { get; set; }
        /// <summary>
        /// 工件可识读标识
        /// </summary>
        public string WIP_ID_Code { get; set; }
        /// <summary>
        /// 容器编号（工装板编号）
        /// </summary>
        public string Container_Number_pallet_code { get; set; }
    }

    /// <summary>
    /// 防错验证响应报文
    /// </summary>
    public class PokaYokeResponse : CustomResponse
    {
        /// <summary>
        /// 防错验证响应报文体
        /// </summary>
        public PokaYokeOutput Output { get; set; } =
            new PokaYokeOutput();
    }

    /// <summary>
    /// 防错验证响应报文体
    /// </summary>
    public class PokaYokeOutput
    {
        /// <summary>
        /// 响应报文体中的WIPStations节点
        /// </summary>
        public PokaYokeOutputWIPStation WIPStations { get; set; } =
            new PokaYokeOutputWIPStation();
        /// <summary>
        /// 响应报文体中的WIPOntoLine节点
        /// </summary>
        public PokaYokeOutputWIPOntoLine WIPOntoLine { get; set; } =
            new PokaYokeOutputWIPOntoLine();
    }

    /// <summary>
    /// 响应报文体中的WIPStations节点
    /// </summary>
    public class PokaYokeOutputWIPStation
    {
        /// <summary>
        /// 防错结果
        /// </summary>
        public byte Poka_Yoke_Result { get; set; } = 0;
        /// <summary>
        /// 产品编号
        /// </summary>
        public string Product_Number { get; set; } = "";
        /// <summary>
        /// 产品叶标识
        /// </summary>
        public int T102LeafID { get; set; } = 0;
    }

    /// <summary>
    /// 响应报文体中的WIPOntoLine节点
    /// </summary>
    public class PokaYokeOutputWIPOntoLine
    {
        /// <summary>
        /// 子在制品数量
        /// </summary>
        public byte Number_Of_Sub_WIPs { get; set; }
        /// <summary>
        /// 子在制品集合
        /// </summary>
        public List<PokaYokeOutputWIPOntoLineSubWIP> SubWIPs { get; set; } =
            new List<PokaYokeOutputWIPOntoLineSubWIP>();
    }

    /// <summary>
    /// 子在制品
    /// </summary>
    public class PokaYokeOutputWIPOntoLineSubWIP
    {
        /// <summary>
        /// 工件主标识代码
        /// </summary>
        public string WIP_Code { get; set; } = "";
        /// <summary>
        /// 工件可识读码类型代码
        /// </summary>
        public string WIP_ID_Type_Code { get; set; } = "";
        /// <summary>
        /// 工件可识读码
        /// </summary>
        public string WIP_ID_Code { get; set; } = "";
        /// <summary>
        /// 生产工单号
        /// </summary>
        public string PWO_Number { get; set; } = "";
        /// <summary>
        /// 在制品子容器编号
        /// </summary>
        public string Sub_Container_Number { get; set; } = "";
        /// <summary>
        /// 在制品数量
        /// </summary>
        public uint WIP_Quantity { get; set; } = 0;
    }

    /// <summary>
    /// 防错验证
    /// </summary>
    public class PokaYoke : CustomWebAPICall
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="webAPIUrl">WebAPI地址</param>
        /// <param name="contentType">报文类型</param>
        /// <param name="clientID">渠道标识</param>
        public PokaYoke(
            string webAPIUrl,
            ContentType contentType,
            string clientID) : base(webAPIUrl, contentType, clientID)
        {
            moduleType = ModuleType.Exchange;
            exCode = "IRAP_DCS_PokaYoke";
        }

        /// <summary>
        /// 请求报文
        /// </summary>
        public PokaYokeRequest Request { get; set; } = null;
        /// <summary>
        /// 响应报文
        /// </summary>
        public PokaYokeResponse Response { get; private set; } = null;

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
                        "请求报文[PokaYokeRequest]未实例化");
                errorMsg =
                    new ErrorMessage()
                    {
                        ErrCode = 999999,
                        ErrText = error.Message,
                    };
                return;
            }

            Response =
                Call<PokaYokeResponse>(Request, out errorMsg);
        }
    }
}
