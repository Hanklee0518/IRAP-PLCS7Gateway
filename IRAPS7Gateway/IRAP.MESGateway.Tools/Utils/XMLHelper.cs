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
    }
}
