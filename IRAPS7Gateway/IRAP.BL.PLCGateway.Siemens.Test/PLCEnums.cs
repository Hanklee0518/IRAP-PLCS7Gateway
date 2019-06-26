using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IRAP.BL.PLCGateway.Siemens.Test
{
    /// <summary>
    /// 西门子S7系列PLC寄存器类别
    /// </summary>
    public enum SiemensRegisterType
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

    /// <summary>
    /// TagGroup类别
    /// </summary>
    public enum TagGroupType
    {
        /// <summary>
        /// 单组
        /// </summary>
        [Description("单组")]
        Single = 0,
        /// <summary>
        /// 包含子组
        /// </summary>
        [Description("包含子组")]
        Multiple = 1
    }

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