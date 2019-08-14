using System;
using System.ComponentModel;
using System.Reflection;

[Flags()]
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
/// 数据块循环读取模式
/// </summary>
public enum CycleReadMode
{
    /// <summary>
    /// 整个数据块
    /// </summary>
    [Description("整个数据块")]
    FullBlock = 1,
    /// <summary>
    /// 控制数据块
    /// </summary>
    [Description("控制数据块")]
    ControlBlock
}

/// <summary>
/// 标记数据类型
/// </summary>
public enum TagDataType
{
    Bool = 1,
    Byte,
    Word,
    Int,
    DWord,
    Real,
    ArrayChar
}

public class EnumHelper
{
    public static string GetEnumDescription(Enum enumValue)
    {
        string str = enumValue.ToString();
        FieldInfo field = enumValue.GetType().GetField(str);
        object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (objs == null || objs.Length == 0)
        {
            return str;
        }
        DescriptionAttribute da = (DescriptionAttribute)objs[0];
        return da.Description;
    }
}