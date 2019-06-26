using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRAP.BL.PLCGateway.Siemens.Test
{
    public class Tools
    {
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
    }
}