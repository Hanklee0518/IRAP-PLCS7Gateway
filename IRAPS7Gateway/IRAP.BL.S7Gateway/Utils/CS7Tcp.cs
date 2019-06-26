using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IRAP.BL.S7Gateway
{
    /// <summary>
    /// 西门子S7系列PLC读写API
    /// </summary>
    public class CS7TcpClient
    {
        /// <summary>
        /// 创建连接对象
        /// </summary>
        /// <returns>PLC对象的识别码，读写时使用</returns>
        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_CreatePlc", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern long CreatePlc();

        /// <summary>
        /// 销毁连接对象
        /// </summary>
        /// <param name="PlcHandle">PLC对象识别码</param>
        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_DestoryPlc", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern void DestoryPlc(ref long PlcHandle);

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="pcIP">PLC的IP地址</param>
        /// <param name="iRack">机架号（一般为0）</param>
        /// <param name="iSlot">
        /// 槽号（S7-1200和S7-1500为0，S7-200和S7-200 smart为1，S7-300和S7-400为2）
        /// </param>
        /// <param name="bIsS7_200">
        /// S7-200的CP243模块需要用以太网向导手动配置模块的TSAP值。
        /// 此位为true时usLocalTSAP和usRemoteTSAP生效
        /// </param>
        /// <param name="usLocalTSAP">PC端的TSAP，即CP243模块端配置的RemoteTSAP</param>
        /// <param name="usRemoteTSAP">CP243端的TSAP，即CP243模块端配置的LocalTSAP</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_ConnectPlc", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ConnectPlc(
            long uiPlcHandle, 
            byte[] pcIP, 
            int iRack, 
            int iSlot, 
            bool bIsS7_200, 
            ushort usLocalTSAP, 
            ushort usRemoteTSAP);

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="PlcHandle">PLC对象识别码</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_DisconnectPlc", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int DisconnectPlc(long PlcHandle);

        /// <summary>
        /// 读取指定位置的bool值
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">字节编号</param>
        /// <param name="iBitNum">字节中的位编号（由低到高:0~7）</param>
        /// <param name="bValue">bool值</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_ReadBool", 
            CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadBool(
            long uiPlcHandle, 
            int iAreaType,
            int iDBNum,
            int iByteNum,
            int iBitNum, 
            ref bool bValue);

        /// <summary>
        /// 将bool值写入指定位置
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">字节编号</param>
        /// <param name="iBitNum">字节中的位编号（由低到高:0~7）</param>
        /// <param name="bValue">bool值</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_WriteBool",
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteBool(
            long uiPlcHandle, 
            int iAreaType,
            int iDBNum,
            int iByteNum,
            int iBitNum, 
            bool bValue);

        /// <summary>
        /// 读取指定位置的byte值
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">字节编号</param>
        /// <param name="ucValue">byte值</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_ReadByte",
            CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadByte(
            long uiPlcHandle, 
            int iAreaType,
            int iDBNum,
            int iByteNum,
            ref byte ucValue);

        /// <summary>
        /// 将byte值写入指定位置
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">字节编号</param>
        /// <param name="ucValue">byte值</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_WriteByte",
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteByte(
            long uiPlcHandle, 
            int iAreaType,
            int iDBNum,
            int iByteNum, 
            byte ucValue);

        /// <summary>
        /// 读取指定位置的ushort值值（无符号整型，2字节）
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">起始字节编号</param>
        /// <param name="usValue">ushort值</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_ReadWord",
            CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadWord(
            long uiPlcHandle, 
            int iAreaType,
            int iDBNum,
            int iByteNum, 
            ref ushort usValue);

        /// <summary>
        /// 将ushort值（无符号整型，2字节）写入指定位置
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">起始字节编号</param>
        /// <param name="usValue">ushort值</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_WriteWord",
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteWord(
            long uiPlcHandle, 
            int iAreaType,
            int iDBNum,
            int iByteNum, 
            ushort usValue);

        /// <summary>
        /// 读取指定位置的short值（有符号整型，2字节）
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">起始字节编号</param>
        /// <param name="sValue">short值</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_ReadInt", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadInt(
            long uiPlcHandle, 
            int iAreaType,
            int iDBNum,
            int iByteNum, 
            ref short sValue);

        /// <summary>
        /// 将short值（有符号整型，2字节）写入指定位置
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">起始字节编号</param>
        /// <param name="sValue">short值</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_WriteInt", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteInt(
            long uiPlcHandle, 
            int iAreaType,
            int iDBNum,
            int iByteNum,
            short sValue);

        /// <summary>
        /// 读取指定位置的uint值（无符号整型，4字节）
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">起始字节编号</param>
        /// <param name="uiValue">uint值</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_ReadDWord", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadDWord(
            long uiPlcHandle,
            int iAreaType,
            int iDBNum,
            int iByteNum, 
            ref uint uiValue);

        /// <summary>
        /// 将uint值（无符号整型，4字节）写入指定位置
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">起始字节编号</param>
        /// <param name="uiValue">uint值</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_WriteDWord",
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteDWord(
            long uiPlcHandle, 
            int iAreaType,
            int iDBNum,
            int iByteNum, 
            uint uiValue);

        /// <summary>
        /// 读取指定位置的int值（有符号整型，4字节）
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">起始字节编号</param>
        /// <param name="iValue">int值</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_ReadDInt", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadDInt(
            long uiPlcHandle, 
            int iAreaType,
            int iDBNum,
            int iByteNum, 
            ref int iValue);

        /// <summary>
        /// 将int值（有符号整型，4字节）写入指定位置
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">起始字节编号</param>
        /// <param name="iValue">int值</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_WriteDInt", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteDInt(
            long uiPlcHandle, 
            int iAreaType,
            int iDBNum,
            int iByteNum,
            int iValue);

        /// <summary>
        /// 读取指定位置的float值（4字节单精度）
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">起始字节编号</param>
        /// <param name="fValue">float值</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_ReadFloat", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadFloat(
            long uiPlcHandle,
            int iAreaType,
            int iDBNum,
            int iByteNum, 
            ref float fValue);

        /// <summary>
        /// 将float值（4字节单精度）写入指定位置
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">起始字节编号</param>
        /// <param name="fValue">float值</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_WriteFloat", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteFloat(
            long uiPlcHandle,
            int iAreaType,
            int iDBNum,
            int iByteNum,
            float fValue);

        /// <summary>
        /// 读取指定位置的字符串值
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">起始字节编号</param>
        /// <param name="sLength">Buffer的长度</param>
        /// <param name="pcValue">byte类型的数组，存放字符串ASCII的二进制值，需要自行转换</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_ReadString", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadString(
            long uiPlcHandle, 
            int iAreaType,
            int iDBNum,
            int iByteNum,
            short sLength,
            byte[] pcValue);

        /// <summary>
        /// 将字符串值写入指定位置
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">起始字节编号</param>
        /// <param name="sLength">Buffer的长度</param>
        /// <param name="pcValue">byte类型的数组，存放字符串ASCII的二进制值，需要自行转换</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_WriteString", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteString(
            long uiPlcHandle, 
            int iAreaType,
            int iDBNum,
            int iByteNum, 
            short sLength, 
            byte[] pcValue);

        /// <summary>
        /// 按字节读取数据块内容
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">起始字节编号</param>
        /// <param name="iLength">Buffer的长度</param>
        /// <param name="pucValue">byte类型的数组</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_ReadBlockAsByte", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadBlockAsByte(
            long uiPlcHandle, 
            int iAreaType,
            int iDBNum,
            int iByteNum, 
            int iLength, 
            byte[] pucValue);

        /// <summary>
        /// 将数组内容写入指定位置数据块中
        /// </summary>
        /// <param name="uiPlcHandle">PLC对象识别码</param>
        /// <param name="iAreaType">PLC寄存器代码</param>
        /// <param name="iDBNum">DB块编号，非DB寄存器的变量忽略此参数</param>
        /// <param name="iByteNum">起始字节编号</param>
        /// <param name="iLength">Buffer的长度</param>
        /// <param name="pucValue">byte类型的数组</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_WriteBlockAsByte", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteBlockAsByte(
            long uiPlcHandle,
            int iAreaType,
            int iDBNum,
            int iByteNum, 
            int iLength, 
            byte[] pucValue);

        /// <summary>
        /// 根据错误代码获取提示文本
        /// </summary>
        /// <param name="iErrorCode">错误代码</param>
        /// <param name="pcText">返回错误提示信息的缓冲区</param>
        /// <param name="iTextLen">缓冲区长度</param>
        /// <returns>0:成功；非0为错误代码</returns>
        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_GetErrorMsg",
            CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int GetErrorMsg(int iErrorCode, byte[] pcText, int iTextLen);
    }
}
