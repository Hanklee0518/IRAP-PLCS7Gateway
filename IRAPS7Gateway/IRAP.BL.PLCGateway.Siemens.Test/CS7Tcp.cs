using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IRAP.BL.S7Gateway
{
    public class CS7TcpClient
    {
        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_CreatePlc", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int64 CreatePlc();


        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_DestoryPlc", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern void DestoryPlc(ref Int64 PlcHandle);


        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_ConnectPlc", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 ConnectPlc(Int64 uiPlcHandle, byte[] pcIP, int iRack, int iSlot, bool bIsS7_200, Int16 usLocalTSAP, Int16 usRemoteTSAP);


        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_DisconnectPlc", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 DisconnectPlc(Int64 PlcHandle);


        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_GetPlcConnected",
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 GetPlcConnected(Int64 PlcHandle, ref Int32 iResult);


        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_ReadBool", 
            CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadBool(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, UInt32 iBitNum, ref UInt32 bValue);


        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_WriteBool",
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteBool(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, UInt32 iBitNum, UInt32 bValue);


        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_ReadByte",
            CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadByte(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, ref UInt32 ucValue);


        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_WriteByte",
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteByte(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, UInt32 ucValue);


        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_ReadWord",
            CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadWord(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, ref UInt32 uiValue);


        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_WriteWord",
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteWord(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, UInt32 usValue);


        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_ReadInt", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadInt(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, ref Int16 sValue);


        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_WriteInt", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteInt(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, Int32 sValue);


        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_ReadDWord", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadDWord(Int64 Index, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, ref UInt32 uiValue);


        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_WriteDWord",
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteDWord(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, UInt32 usValue);


        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_ReadDInt", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadDInt(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, ref Int32 iValue);


        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_WriteDInt", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteDInt(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, Int32 iValue);


        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_ReadFloat", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadFloat(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, ref float bValue);

        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_WriteFloat", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteFloat(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, float fValue);


        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_ReadString", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadString(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, Int16 iLength, byte[] pcValue);

        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_WriteString", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteString(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, Int16 iLength, byte[] pcValue);


        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_ReadBlockAsByte", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadBlockAsByte(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, int iLength, byte[] pucValue);

        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "BfS7_WriteBlockAsByte", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteBlockAsByte(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, int iLength, byte[] pucValue);

        /// <summary>
        /// 根据错误代码获取提示文本
        /// </summary>
        /// <param name="iErrorCode">错误代码</param>
        /// <param name="pcText">返回错误提示信息的缓冲区</param>
        /// <param name="iTextLen">缓冲区长度</param>
        /// <returns>0-成功；非0-错误代码</returns>
        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "BfS7_GetErrorMsg",
            CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int GetErrorMsg(int iErrorCode, byte[] pcText, int iTextLen);
    }
}
