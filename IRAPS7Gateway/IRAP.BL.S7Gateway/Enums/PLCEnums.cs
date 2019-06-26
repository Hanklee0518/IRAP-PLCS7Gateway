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
}