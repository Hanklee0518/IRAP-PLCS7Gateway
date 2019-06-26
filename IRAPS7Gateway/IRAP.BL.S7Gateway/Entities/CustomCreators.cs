using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace IRAP.BL.S7Gateway.Entities
{
    /// <summary>
    /// 所有Creator类的父类
    /// </summary>
    public class CustomCreator : IRAPBaseObject
    {
    }

    /// <summary>
    /// PLC 对象创建者
    /// </summary>
    public abstract class CustomPLCCreator : CustomCreator
    {
        /// <summary>
        /// 创建一个 PLC 对象
        /// </summary>
        /// <param name="node">PLC 对象属性的 Xml 节点</param>
        public abstract CustomPLC CreateProduct(XmlNode node);
    }

    /// <summary>
    /// Device对象创建者类
    /// </summary>
    public abstract class CustomDeviceCreator : CustomCreator
    {
        /// <summary>
        /// 创建一个CustomDevice对象
        /// </summary>
        /// <param name="parent">设备所属PLC对象</param>
        /// <param name="node">设备属性Xml节点</param>
        public abstract CustomDevice CreateProduct(CustomPLC parent, XmlNode node);
    }

    /// <summary>
    /// TagGroup对象创建者类
    /// </summary>
    public abstract class CustomTagGroupCreator : CustomCreator
    {
        /// <summary>
        /// 创建一个TagGroup对象
        /// </summary>
        /// <param name="parent">TagGroup对象所依赖的Device对象</param>
        /// <param name="node">TagGroup对象属性的Xml节点</param>
        /// <param name="registerHandler">Tag对象注册用事件</param>
        public abstract CustomTagGroup CreateProuct(CustomDevice parent, XmlNode node, TagRegisterHandler registerHandler);
    }

    /// <summary>
    /// Tag对象创建者类
    /// </summary>
    public abstract class CustomTagCreator : CustomCreator
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="parent">Tag所属CustomGroup对象</param>
        /// <param name="node">Tag属性的Xml节点</param>
        public abstract CustomTag CreateProduct(CustomGroup parent, XmlNode node);
    }
}