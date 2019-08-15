using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IRAP.MESGateway.Tools.Utils
{
    internal class XMLHelper
    {
        public static XmlAttribute CreateAttribute(XmlDocument xml, string name, string value)
        {
            XmlAttribute attr = xml.CreateAttribute(name);
            attr.Value = value;
            return attr;
        }

        public static string GetAttributeStringValue(XmlNode node, string name, string defaultValue)
        {
            if (node.Attributes[name] == null)
            {
                return defaultValue;
            }
            else
            {
                return node.Attributes[name].Value;
            }
        }

        public static int GetAttributeIntValue(XmlNode node, string name, int defaultValue)
        {
            if (node.Attributes[name] == null)
            {
                return defaultValue;
            }
            else
            {
                int rlt = defaultValue;
                int.TryParse(node.Attributes[name].Value, out rlt);
                return rlt;
            }
        }
    }
}
