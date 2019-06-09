using IRAP.BL.S7Gateway.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAP.BL.S7Gateway.Entities
{
    public class CustomTag
    {
        private uint db_Offset = 0;

        /// <summary>
        /// 是否使用
        /// </summary>
        protected bool used = false;

        public string TagName { get; set; } = "";

        public S7RegisterType DB_Type { get; set; } = S7RegisterType.DB;

        /// <summary>
        /// 偏移量
        /// </summary>
        public uint DB_Offset
        {
            get { return db_Offset; }
            set
            {
                db_Offset = value;
                used = true;
            }
        }

        /// <summary>
        /// 序号
        /// </summary>
        public int Ordinal { get; set; } = 0;

        /// <summary>
        /// 是否使用
        /// </summary>
        public bool Used
        {
            get { return used; }
        }
    }

    public abstract class CustomLengthTag : CustomTag
    {
        /// <summary>
        /// 数据长度
        /// </summary>
        public abstract int Length { get; }
    }

    public class ArrayCharOfTag : CustomLengthTag
    {
        private int length;

        /// <param name="length">字符串长度</param>
        public ArrayCharOfTag(int length)
        {
            this.length = length;
        }

        public override int Length => length;

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }

    public class BoolOfTag : CustomTag
    {
        /// <summary>
        /// 位置
        /// </summary>
        public uint Position { get; set; } = 0;

        /// <summary>
        /// 值
        /// </summary>
        public bool Value { get; set; }

        public void SetPosition(uint offset, uint position)
        {
            DB_Offset = offset;
            Position = position;
        }
    }

    public class ByteOfTag : CustomLengthTag
    {
        public override int Length => 1;

        /// <summary>
        /// 值
        /// </summary>
        public byte Value { get; set; }
    }

    public class DWordOfTag : CustomLengthTag
    {
        public override int Length => 4;

        /// <summary>
        /// 值
        /// </summary>
        public long Value { get; set; }
    }

    public class WordOfTag : CustomLengthTag
    {
        public override int Length => 2;

        /// <summary>
        /// 值
        /// </summary>
        public int Value { get; set; }
    }
}
