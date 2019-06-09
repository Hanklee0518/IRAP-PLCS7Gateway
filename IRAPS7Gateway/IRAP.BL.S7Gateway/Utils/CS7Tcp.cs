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
            EntryPoint = "CreatePlc", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int64 CreatePlc();


        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "DestoryPlc", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern void DestoryPlc(ref Int64 PlcHandle);


        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "ConnectPlc", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 ConnectPlc(Int64 uiPlcHandle, byte[] pcIP, int iRack, int iSlot, bool bIsS7_200, Int16 usLocalTSAP, Int16 usRemoteTSAP);


        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "DisconnectPlc", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 DisconnectPlc(Int64 PlcHandle);


        [DllImport("ButterflyS7.dll", EntryPoint = "GetPlcConnected", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 GetPlcConnected(Int64 PlcHandle, ref Int32 iResult);


        [DllImport("ButterflyS7.dll", EntryPoint = "ReadBool", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadBool(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, UInt32 iBitNum, ref UInt32 bValue);


        [DllImport("ButterflyS7.dll", EntryPoint = "WriteBool", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteBool(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, UInt32 iBitNum, UInt32 bValue);


        [DllImport("ButterflyS7.dll", EntryPoint = "ReadByte", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadByte(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, ref UInt32 ucValue);


        [DllImport("ButterflyS7.dll", EntryPoint = "WriteByte", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteByte(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, UInt32 ucValue);


        [DllImport("ButterflyS7.dll", EntryPoint = "ReadWord", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadWord(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, ref UInt32 uiValue);


        [DllImport("ButterflyS7.dll", EntryPoint = "WriteWord", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteWord(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, UInt32 usValue);


        [DllImport("ButterflyS7.dll", EntryPoint = "ReadInt", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadInt(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, ref Int16 sValue);


        [DllImport("ButterflyS7.dll", EntryPoint = "WriteInt", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteInt(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, Int32 sValue);


        [DllImport("ButterflyS7.dll", EntryPoint = "ReadDWord", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadDWord(Int64 Index, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, ref UInt32 uiValue);


        [DllImport("ButterflyS7.dll", EntryPoint = "WriteDWord", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteDWord(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, UInt32 usValue);


        [DllImport(
            "ButterflyS7.dll",
            EntryPoint = "ReadDInt", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadDInt(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, ref Int32 iValue);


        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "WriteDInt", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteDInt(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, Int32 iValue);


        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "ReadFloat", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadFloat(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, ref float bValue);

        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "WriteFloat", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteFloat(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, float fValue);


        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "ReadString", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadString(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, Int16 iLength, byte[] pcValue);

        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "WriteString", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteString(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, Int16 iLength, byte[] pcValue);


        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "ReadBlockAsByte", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadBlockAsByte(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, int iLength, byte[] pucValue);

        [DllImport(
            "ButterflyS7.dll", 
            EntryPoint = "WriteBlockAsByte", 
            CharSet = CharSet.Ansi, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteBlockAsByte(Int64 uiPlcHandle, int iAreaType, UInt32 iDBNum, UInt32 iByteNum, int iLength, byte[] pucValue);
    }
}
