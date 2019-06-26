using IRAP.BL.S7Gateway.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace IRAP.BL.S7Gateway.Entities
{
    /// <summary>
    /// 西门子PLC对象创建者
    /// </summary>
    public class SiemensPLCCreator : CustomPLCCreator
    {
        /// <summary>
        /// 创建一个西门子 PLC 对象
        /// </summary>
        public override CustomPLC CreateProduct(XmlNode node)
        {
            if (node.Name.ToUpper() != "SIEMENSPLC")
            {
                return null;
            }
            else
            {
                return new SiemensPLC(node);
            }
        }
    }

    /// <summary>
    /// 受控西门子PLC的设备创建者类
    /// </summary>
    public class SiemensDeviceCreator : CustomDeviceCreator
    {
        /// <summary>
        /// 创建一个SiemensDevice对象
        /// </summary>
        public override CustomDevice CreateProduct(CustomPLC parent, XmlNode node)
        {
            return new SiemensDevice(parent, node);
        }
    }

    /// <summary>
    /// 西门子PLC TagGroup对象创建者类
    /// </summary>
    public class SiemensTagGroupCreator : CustomTagGroupCreator
    {
        /// <summary>
        /// 创建一个西门子TagGroup对象
        /// </summary>
        /// <param name="parent">TagGroup对象所依赖的Device对象</param>
        /// <param name="node">TagGroup对象属性的Xml节点</param>
        /// <param name="registerHandler">Tag对象注册用事件</param>
        public override CustomTagGroup CreateProuct(
            CustomDevice parent, 
            XmlNode node, 
            TagRegisterHandler registerHandler)
        {
            if (node.Name.ToUpper() != "TAGGROUP")
            {
                return null;
            }

            return new SiemensTagGroup(parent, node, registerHandler);
        }
    }

    /// <summary>
    /// 创建一个西门子Tag对象
    /// </summary>
    public class SiemensTagCreator : CustomTagCreator
    {
        /// <summary>
        /// 创建一个西门子Tag对象
        /// </summary>
        /// <param name="parent">Tag所属Group对象</param>
        /// <param name="node">Tag属性的Xml节点</param>
        public override CustomTag CreateProduct(CustomGroup parent, XmlNode node)
        {
            if (node.Name.ToUpper() != "TAG")
            {
                throw new Exception("node节点名不是Tag");
            }

            if (node.Attributes["Datatype"] == null)
            {
                throw new Exception("node节点中没有定义[Datatype]属性，请注意大小写");
            }

            string dataType = node.Attributes["Datatype"].Value;
            switch (dataType)
            {
                case "Bool":
                    return new SiemensBoolOfTag(parent, node);
                case "Byte":
                    return new SiemensByteOfTag(parent, node);
                case "Word":
                    return new SiemensWordOfTag(parent, node);
                case "Int":
                    return new SiemensIntOfTag(parent, node);
                case "DWord":
                    return new SiemensDWordOfTag(parent, node);
                case "Real":
                    return new SiemensRealOfTag(parent, node);
                case "ArrayChar":
                    return new SiemensArrayCharOfTag(parent, node);
                default:
                    throw new Exception($"当前版本不支持Datatype=[{dataType}]类型的Tag");
            }
        }
    }
}