using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IRAP.BL.S7Gateway.Utils
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class Tools
    {
        /// <summary>
        /// 比较两个 Byte 数组的内容是否一致
        /// </summary>
        /// <param name="x">Byte 数组</param>
        /// <param name="y">Byte 数组</param>
        /// <returns>True:内容一致；False:内容有差异</returns>
        public static bool ByteEquals(byte[] x, byte[] y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            if (x.Length == y.Length)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i] != y[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 计算数据块内容的哈希值
        /// </summary>
        /// <param name="buffer">数据块内容</param>
        /// <returns>byte数组的哈希值</returns>
        public static byte[] CalculateHash(byte[] buffer)
        {
            HashAlgorithm hash = HashAlgorithm.Create();
            return hash.ComputeHash(buffer);
        }

        /// <summary>
        /// 获取字节中指定位置的bool量
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="pos">位置</param>
        /// <returns>bool值</returns>
        public static bool GetBitValue(byte data, int pos)
        {
            return (data >> pos & 0x1) == 1;
        }

        /// <summary>
        /// 获取byte数组中指定位置的ushort值（无符号整型，2字节）
        /// </summary>
        /// <param name="buffer">byte数组</param>
        /// <param name="pos">位置</param>
        /// <returns>ushort（2字节的无符号整型）</returns>
        public static ushort GetWordValue(byte[] buffer, int pos)
        {
            byte[] value = new byte[2];
            Array.Copy(buffer, pos, value, 0, 2);

            return BitConverter.ToUInt16(value, 0);
        }

        /// <summary>
        /// 获取byte数组中指定位置的short值（有符号整型，2字节）
        /// </summary>
        /// <param name="buffer">byte数组</param>
        /// <param name="pos">位置</param>
        /// <returns>short（有符号整型，2字节）</returns>
        public static short GetIntValue(byte[] buffer, int pos)
        {
            short value =
                (short)((buffer[pos] & 0xff) << 8 | (buffer[pos + 1] & 0xff));
            return value;
        }

        /// <summary>
        /// 获取byte数组中指定位置的uint值（无符号整型，4字节）
        /// </summary>
        /// <param name="buffer">byte数组</param>
        /// <param name="pos">位置</param>
        /// <returns>uint（无符号整型，4字节）</returns>
        public static uint GetDWordValue(byte[] buffer, int pos)
        {
            byte[] value = new byte[4];
            Array.Copy(buffer, pos, value, 0, 4);

            return BitConverter.ToUInt32(value, 0);
        }

        /// <summary>
        /// 获取byte数组中指定位置的float值（4字节单精度）
        /// </summary>
        /// <param name="buffer">byte数组</param>
        /// <param name="pos">位置</param>
        /// <returns>float（4字节单精度）</returns>
        public static float GetRealValue(byte[] buffer, int pos)
        {
            byte[] value = new byte[4];
            Array.Copy(buffer, pos, value, 0, 4);

            return BitConverter.ToSingle(value, 0);
        }

        /// <summary>
        /// 获取byte数组中指定位置的字符串
        /// </summary>
        /// <param name="buffer">byte数组</param>
        /// <param name="pos">位置</param>
        /// <param name="length">字符串长度</param>
        /// <returns>string</returns>
        public static string GetStringValue(byte[] buffer, int pos, int length)
        {
            byte[] value = new byte[length];
            Array.Copy(buffer, pos, value, 0, length);

            return Encoding.Default.GetString(value);
        }

        /// <summary>
        /// byte数组转换成 BCD 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string BytesToBCD(byte[] buffer)
        {
            string tmp = "";
            for (int i = 0; i < buffer.Length; i++)
            {
                tmp += $"{string.Format("{0:x2}", buffer[i])}";
            }
            return tmp;
        }
    }
}
