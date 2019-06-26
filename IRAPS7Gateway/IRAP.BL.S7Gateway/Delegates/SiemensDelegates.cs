using IRAP.BL.S7Gateway.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRAP.BL.S7Gateway
{
    /// <summary>
    /// 西门子PLC数据块内容变化事件
    /// </summary>
    /// <param name="buffer">数据缓冲区</param>
    public delegate void SiemensDBDataChangedHandler(byte[] buffer);

    /// <summary>
    /// 西门子PLC数据块回写事件
    /// </summary>
    /// <param name="device">受西门子PLC控制的设备对象</param>
    /// <param name="tag">待回写的Tag对象</param>
    public delegate void SiemensWriteBackHandler(SiemensDevice device, SiemensTag tag);
}