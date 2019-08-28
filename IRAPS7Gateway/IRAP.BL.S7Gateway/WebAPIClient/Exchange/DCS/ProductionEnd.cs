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
    /// 生产结束请求报文
    /// </summary>
    public class ProductionEndRequest : CustomPLCRequest
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public override string ExCode => "IRAP_DCS_ProductionEnd";
        /// <summary>
        /// 输入参数
        /// </summary>
        public ProductionEndParamXML ParamXML { get; set; } =
            new ProductionEndParamXML();
    }

    /// <summary>
    /// 输入参数
    /// </summary>
    public class ProductionEndParamXML
    {
        /// <summary>
        /// 工序生产结论
        /// </summary>
        public byte Operation_Conclusion { get; set; }
        /// <summary>
        /// 工艺参数集合
        /// </summary>
        public List<RecipeRow> RECIPE { get; } = new List<RecipeRow>();
        /// <summary>
        /// 属性集合
        /// </summary>
        public List<PropertyRow> PROPERTY { get; } = new List<PropertyRow>();
        /// <summary>
        /// 检验结论集合
        /// </summary>
        public List<TestResultRow> TestResult { get; } = new List<TestResultRow>();
        /// <summary>
        /// 工装寿命集合
        /// </summary>
        public List<ToolLifeRow> ToolLife { get; } = new List<ToolLifeRow>();
    }

    /// <summary>
    /// 工艺参数
    /// </summary>
    public class RecipeRow
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        public string TagName { get; set; }
        /// <summary>
        /// 标签值
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// 属性
    /// </summary>
    public class PropertyRow
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        public string TagName { get; set; }
        /// <summary>
        /// 标签值
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// 检验结果
    /// </summary>
    public class TestResultRow
    {
        /// <summary>
        /// 测试项
        /// </summary>
        public ushort Test_Item_Number { get; set; }
        /// <summary>
        /// 单项结论(P/F)
        /// </summary>
        public string Conclusion { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 度量值
        /// </summary>
        public uint Metric01 { get; set; }
        /// <summary>
        /// 低限值
        /// </summary>
        public uint Low_Limit { get; set; }
        /// <summary>
        /// 通过标准
        /// </summary>
        public string Criterion { get; set; }
        /// <summary>
        /// 高限值
        /// </summary>
        public uint High_Limit { get; set; }
    }

    /// <summary>
    /// 工装寿命
    /// </summary>
    public class ToolLifeRow
    {
        /// <summary>
        /// 工装编号
        /// </summary>
        public string Tool_Code { get; set; }
        /// <summary>
        /// 工装序列号
        /// </summary>
        public string Tool_SN { get; set; }
        /// <summary>
        /// 剩余使用寿命
        /// </summary>
        public uint Tool_Use_Life_In_Times { get; set; }
        /// <summary>
        /// 剩余保养寿命
        /// </summary>
        public uint Tool_PM_Life_In_Times { get; set; }
    }

    /// <summary>
    /// 生产结束响应报文
    /// </summary>
    public class ProductionEndResponse : CustomResponse
    {
        /// <summary>
        /// 防错校验结果
        /// </summary>
        public ProductionEndOutput Output { get; set; } =
            new ProductionEndOutput();
    }

    /// <summary>
    /// 生产结束响应报文输出参数
    /// </summary>
    public class ProductionEndOutput
    {
        /// <summary>
        /// 防错校验结果
        /// </summary>
        public byte Poka_Yoke_Result { get; set; } = 0;
    }

    /// <summary>
    /// 生产结束交易
    /// </summary>
    public class ProductionEnd : CustomWebAPICall
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="webAPIUrl">WebAPI地址</param>
        /// <param name="contentType">报文类型</param>
        /// <param name="clientID">渠道标识</param>
        /// <param name="logEntity">交易日志实体对象</param>
        public ProductionEnd(
            string webAPIUrl, 
            ContentType contentType, 
            string clientID,
            DCSGatewayLogEntity logEntity) : 
            base(webAPIUrl, contentType, clientID, logEntity)
        {
            moduleType = ModuleType.Exchange;
            ExCode = "IRAP_DCS_ProductionEnd";
        }

        /// <summary>
        /// 请求报文
        /// </summary>
        public ProductionEndRequest Request { get; set; } = null;
        /// <summary>
        /// 响应报文
        /// </summary>
        public ProductionEndResponse Response { get; private set; } = null;

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
                        "请求报文[ProductionEndRequest]未实例化");
                errorMsg =
                    new ErrorMessage()
                    {
                        ErrCode = 999999,
                        ErrText = error.Message,
                    };
                return;
            }

            Response =
                Call<ProductionEndResponse>(Request, out errorMsg);
        }
    }
}
