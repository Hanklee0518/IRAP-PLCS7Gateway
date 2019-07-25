using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAP.BL.S7Gateway.WebAPIClient.Contents
{
    /// <summary>
    /// 适用于设备PLC数据采集与集控的请求报文父类
    /// </summary>
    public abstract class CustomPLCRequest : CustomRequest
    {
        /// <summary>
        /// 用户代码，默认：Anonymous
        /// </summary>
        public string UserCode { get; set; } = "Anonyumous";
        /// <summary>
        /// 系统登录标识，默认：1
        /// </summary>
        public long SysLogID { get; set; } = 1;
        /// <summary>
        /// 设备叶标识
        /// </summary>
        public int T133LeafID { get; set; } = 0;
        /// <summary>
        /// 工序叶标识
        /// </summary>
        public int T216LeafID { get; set; } = 0;
        /// <summary>
        /// 产品叶标识
        /// </summary>
        public int T102LeafID { get; set; } = 0;
        /// <summary>
        /// 工位叶标识
        /// </summary>
        public int T107LeafID { get; set; } = 0;
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
    }
}
