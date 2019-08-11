using IRAP.MESGateway.Tools.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IRAP.MESGateway.Tools.Entities
{
    /// <summary>
    /// PLC
    /// </summary>
    internal class PLCEntity
    {
        [Category("数据"), Description("IP地址"), DisplayName("IP地址")]
        public string IPAddress { get; set; } = "127.0.0.1";
        [Category("数据"), Description("机架号"), DisplayName("机架号")]
        public int Rack { get; set; } = 0;
        [Category("数据"), Description("插槽号"), DisplayName("插槽号")]
        public int Slot { get; set; } = 0;

        public void LoadFromXmlNode(XmlNode node)
        {
            if (node.Attributes["IPAddress"] != null)
            {
                IPAddress = node.Attributes["IPAddress"].Value;
            }
            if (node.Attributes["Rack"] != null)
            {
                if (int.TryParse(node.Attributes["Rack"].Value, out int rlt))
                {
                    Rack = rlt;
                }
            }
            if (node.Attributes["Slot"] != null)
            {
                if (int.TryParse(node.Attributes["Slot"].Value, out int rlt))
                {
                    Slot = rlt;
                }
            }
        }

        public XmlNode GenerateXmlNode()
        {
            XmlDocument xml = new XmlDocument();
            XmlNode root = xml.CreateElement("PLC");
            root.Attributes.Append(XMLHelper.CreateAttribute(xml, "IPAddress", IPAddress));
            root.Attributes.Append(XMLHelper.CreateAttribute(xml, "Rack", Rack.ToString()));
            root.Attributes.Append(XMLHelper.CreateAttribute(xml, "Slot", Slot.ToString()));
            return root;
        }
    }
}
