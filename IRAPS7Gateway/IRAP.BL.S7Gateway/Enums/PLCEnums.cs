using System.ComponentModel;

namespace IRAP.BL.S7Gateway.Enums
{
    /// <summary>
    /// PLC类型（当前版本支持的PLC类型）
    /// </summary>
    public enum PLCType
    {
        /// <summary>
        /// 西门子PLC
        /// </summary>
        [Description("西门子PLC")]
        SIEMENS = 1
    }

    /// <summary>
    /// Tag类别
    /// </summary>
    public enum TagType
    {
        /// <summary>
        /// 控制类
        /// </summary>
        [Description("控制类")]
        C = 0,
        /// <summary>
        /// 信息类
        /// </summary>
        [Description("信息类")]
        A,
    }
}