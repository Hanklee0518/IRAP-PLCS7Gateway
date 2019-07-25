using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAP.BL.S7Gateway.WebAPIClient
{
    /// <summary>
    /// 错误消息类
    /// </summary>
    public class ErrorMessage
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public string ExCode { get; set; }
        /// <summary>
        /// 错误编号（0：没有错误）
        /// </summary>
        public int ErrCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrText { get; set; }
        /// <summary>
        /// 原始的请求报文
        /// </summary>
        public string SourceREQContent { get; set; }
        /// <summary>
        /// 原始的响应报文
        /// </summary>
        public string SourceRESPContent { get; set; }
    }
}
