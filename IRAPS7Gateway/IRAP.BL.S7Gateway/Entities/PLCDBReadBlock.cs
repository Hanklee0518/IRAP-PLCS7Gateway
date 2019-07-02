using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRAP.BL.S7Gateway.Entities
{
    /// <summary>
    /// PLC数据块读取缓冲区定义
    /// </summary>
    public class PLCDBBlock
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

    /// <summary>
    /// PLC数据块读取缓冲区定义集合
    /// </summary>
    public class PLCDBBlockCollection : IEnumerable
    {
        /// <summary>
        /// 需读取的数据块定义集合
        /// </summary>
        private Dictionary<string, PLCDBBlock> _items =
            new Dictionary<string, PLCDBBlock>();

        /// <summary>
        /// 根据索引号获取集合中的PLCDBReadBlock对象
        /// </summary>
        /// <param name="index">索引号</param>
        /// <returns>PLCDBReadBlock对象，若索引号超出范围则返回null</returns>
        public PLCDBBlock this[int index]
        {
            get
            {
                if (index >= 0 && index < _items.Count)
                {
                    return _items.ElementAt(index).Value;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 根据关键字获取PLCDBReadBlock对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>PLCDBReadBlock对象，若未找到则返回null</returns>
        public PLCDBBlock this[string key]
        {
            get
            {
                _items.TryGetValue(key, out PLCDBBlock rlt);
                return rlt;
            }
        }

        /// <summary>
        /// 当前集合中的PLCDBReadBlock对象个数
        /// </summary>
        public int Count { get { return _items.Count; } }

        /// <summary>
        /// 在集合中新增一个PLCDBReadBlock对象
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="item">PLCDBReadBlock对象</param>
        public void Add(string key, PLCDBBlock item)
        {
            if (key == "")
            {
                throw new Exception("关键字参数不能空白");
            }
            if (item == null)
            {
                throw new Exception("PLCDBReadBlock对象不能是null");
            }

            if (_items.ContainsKey(key))
            {
                throw new Exception($"集合中已经存在关键字[{key}]的PLCDBReadBlock对象");
            }

            _items.Add(key, item);
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>可用于循环访问集合的IEnumerator对象</returns>
        public IEnumerator GetEnumerator()
        {
            foreach (PLCDBBlock item in _items.Values)
            {
                yield return item;
            }
        }

        /// <summary>
        /// 根据索引号获取关键字
        /// </summary>
        /// <param name="index">索引号</param>
        public string GetKey(int index)
        {
            return _items.Keys.ElementAt(index);
        }
    }
}