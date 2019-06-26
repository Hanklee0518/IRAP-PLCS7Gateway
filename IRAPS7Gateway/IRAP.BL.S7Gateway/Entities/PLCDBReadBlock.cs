using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRAP.BL.S7Gateway.Entities
{
    /// <summary>
    /// PLC数据块读取缓冲区
    /// </summary>
    public class PLCDBReadBlock
    {
        /// <summary>
        /// 起始偏移量
        /// </summary>
        public int Start_Offset { get; private set; }

        /// <summary>
        /// 长度
        /// </summary>
        public int BufferLength { get; private set; }

        /// <summary>
        /// 初始化起始偏移量和总长度
        /// </summary>
        /// <param name="offset">起始偏移量</param>
        /// <param name="length">长度</param>
        public void Set(int offset, int length)
        {
            Start_Offset = offset;
            BufferLength = length;
        }

        /// <summary>
        /// 根据偏移量和长度，设置起始偏移量和总长度
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <param name="length">长度</param>
        public void Add(int offset, int length)
        {
            if (BufferLength == 0)
            {
                Set(offset, length);
                return;
            }

            uint old_offset_end = (uint)(Start_Offset + BufferLength - 1);
            if (offset < Start_Offset)
            {
                BufferLength = (int)(old_offset_end - offset + 1);
                Start_Offset = offset;
            }
            else
            {
                uint new_offset_end = (uint)(offset + length - 1);
                if (old_offset_end < new_offset_end)
                {
                    BufferLength = (int)(new_offset_end - Start_Offset + 1);
                }
            }
        }
    }
}