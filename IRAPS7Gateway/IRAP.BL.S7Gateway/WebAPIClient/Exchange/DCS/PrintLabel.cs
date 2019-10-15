/*----------------------------------------------------------------
// Copyright © 2019 Hanklee rights reserved. 
// CLR 版本：4.0.30319.42000
// 类 名 称：PrintLabel
// 文 件 名：PrintLabel
// 创 建 者：李智颖
// 创建日期：2019/10/15 18:12:39
// 版本	日期					修改人	
// v0.1	2019/10/15 18:12:39      李智颖
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAP.BL.S7Gateway.Utils;
using IRAP.BL.S7Gateway.WebAPIClient.Contents;
using IRAP.BL.S7Gateway.WebAPIClient.Enums;

namespace IRAP.BL.S7Gateway.WebAPIClient.Exchange.DCS
{
    /// <summary>
    /// 命名空间：IRAP.BL.S7Gateway.WebAPIClient.Exchange.DCS
    /// 创 建 者：李智颖
    /// 创建日期：2019/10/15 18:12:39
    /// 类    名：PrintLabel(请求标签打印)
    /// </summary>
    public class PrintLabel : CustomWebAPICall
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="webAPIUrl">WebAPI地址</param>
        /// <param name="contentType">报文类型</param>
        /// <param name="clientID">渠道标识</param>
        /// <param name="logEntity">交易日志实体对象</param>
        public PrintLabel(
            string webAPIUrl,
            ContentType contentType,
            string clientID,
            DCSGatewayLogEntity logEntity) : base(webAPIUrl, contentType, clientID, logEntity)
        {
            moduleType = ModuleType.Exchange;
            ExCode = "IRAP_DCS_PrintLabel";
        }

        /// <summary>
        /// 请求报文
        /// </summary>
        public PrintLabelRequest Request { get; set; } = null;
        /// <summary>
        /// 响应报文
        /// </summary>
        public PrintLabelResponse Response { get; private set; } = null;

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
                        "请求报文未实例化");
                errorMsg =
                    new ErrorMessage()
                    {
                        ErrCode = 999999,
                        ErrText = error.Message,
                    };
                return;
            }

            Response =
                Call<PrintLabelResponse>(Request, out errorMsg);
        }
    }

    /// <summary>
	/// 命名空间：IRAP.BL.S7Gateway.WebAPIClient.Exchange.DCS
	/// 创 建 者：李智颖
	/// 创建日期：2019/10/15, 18:22:39
	/// 类    名：PrintLabelRequest
	/// </summary>	
	public class PrintLabelRequest : CustomPLCRequest
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public override string ExCode => "IRAP_DCS_PrintLabel";
    }

    /// <summary>
	/// 命名空间：IRAP.BL.S7Gateway.WebAPIClient.Exchange.DCS
	/// 创 建 者：李智颖
	/// 创建日期：2019/10/15, 18:24:26
	/// 类    名：PrintLabelResponse
	/// </summary>	
	public class PrintLabelResponse : CustomResponse
    {
        /// <summary>
        /// 响应报文输出明细
        /// </summary>
        public PrintLabelResponseDetail Output { get; set; }
            = new PrintLabelResponseDetail();
    }

    /// <summary>
	/// 命名空间：
	/// 创 建 者：李智颖
	/// 创建日期：2019/10/15, 18:25:06
	/// 类    名：PrintLabelResponseDetail
	/// </summary>	
	public class PrintLabelResponseDetail
    {
        /// <summary>
        /// 打印状态
        /// </summary>
        public byte Print_Status { get; set; } = 0;
    }
}
