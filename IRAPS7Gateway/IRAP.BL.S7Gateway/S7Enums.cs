using System.ComponentModel;

namespace IRAP.BL.S7Gateway.Enums
{
    /// <summary>
    /// S7 寄存器类别
    /// </summary>
    public enum S7RegisterType
    {
        /// <summary>
        /// I 区
        /// </summary>
        [Description("I 区")]
        I = 0x81,
        /// <summary>
        /// Q 区
        /// </summary>
        [Description("Q 区")]
        Q = 0x82,
        /// <summary>
        /// M 区
        /// </summary>
        [Description("M 区")]
        M = 0x83,
        /// <summary>
        /// DB 区
        /// </summary>
        [Description("DB 区")]
        DB = 0x84
    }
}