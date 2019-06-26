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
        /// <returns></returns>
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
        /// <returns></returns>
        public static byte[] CalculateHash(byte[] buffer)
        {
            HashAlgorithm hash = HashAlgorithm.Create();
            return hash.ComputeHash(buffer);
        }
    }
}
