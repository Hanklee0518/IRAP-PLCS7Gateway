using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAP.BL.S7Gateway.WebAPIClient.Enums
{
    /// <summary>
    /// 报文类别枚举类型
    /// </summary>
    [Flags()]
    public enum ContentType
    {
        /// <summary>
        /// JSON格式的报文
        /// </summary>
        [Description("JSON格式的报文")]
        json = 1,
        /// <summary>
        /// XML格式的报文
        /// </summary>
        [Description("XML格式的报文")]
        xml,
    }

    /// <summary>
    /// 交互性枚举类别
    /// </summary>
    [Flags()]
    public enum Interactiveness
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Description("全部")]
        All = 0,
        /// <summary>
        /// 周期读
        /// </summary>
        [Description("周期读")]
        CycleRead,
        /// <summary>
        /// 事件读写
        /// </summary>
        [Description("事件读写")]
        EventRW,
    }

    /// <summary>
    /// 模块类别枚举类型
    /// </summary>
    [Flags()]
    public enum ModuleType
    {
        /// <summary>
        /// 授权认证类别
        /// </summary>
        [Description("授权认证类别")]
        OpenAuth = 1,
        /// <summary>
        /// 数据交换类别
        /// </summary>
        [Description("数据交换类别")]
        Exchange,
    }
}
