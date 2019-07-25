using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAP.BL.S7Gateway.WebAPIClient.Contents
{
    /// <summary>
    /// 请求报文父类
    /// </summary>
    public abstract class CustomRequest
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public abstract string ExCode { get; }
        /// <summary>
        /// 社区标识
        /// </summary>
        public int CommunityID { get; set; }
    }

    /// <summary>
    /// 响应报文父类
    /// </summary>
    public class CustomResponse
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public string ExCode { get; set; }
        /// <summary>
        /// 交易结果代码（0-交易成功；非0-交易失败）
        /// </summary>
        public int ErrCode { get; set; }
        /// <summary>
        /// 交易结果消息字符串
        /// </summary>
        public string ErrText { get; set; }
    }
}
