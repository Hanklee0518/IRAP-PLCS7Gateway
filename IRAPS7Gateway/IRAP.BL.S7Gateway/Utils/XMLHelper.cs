using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace IRAP.BL.S7Gateway.Utils
{
    /// <summary>
    /// XML转换类
    /// </summary>
    public static class XMLHelper
    {
        /// <summary>
        /// 将对象属性序列化为 XML 报文
        /// </summary>
        /// <param name="obj">待序列化的对象</param>
        /// <returns>XML 报文</returns>
        public static string ToXML(this object obj)
        {
            XmlRootAttribute rootAttr = new XmlRootAttribute("Result");
            XmlSerializer xs = new XmlSerializer(obj.GetType(), rootAttr);

            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;
            xmlSettings.IndentChars = " ";
            xmlSettings.NewLineChars = "\r\n";
            xmlSettings.Encoding = Encoding.UTF8;
            xmlSettings.OmitXmlDeclaration = true;      // 是否生成 XML 声明头 

            StringBuilder sb = new StringBuilder();
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");
            using (XmlWriter xmlWriter = XmlWriter.Create(sb, xmlSettings))
            {
                xs.Serialize(xmlWriter, obj, namespaces);
                xmlWriter.Close();
            }

            return sb.ToString();
        }

        /// <summary>
        /// 将 XML 报文反序列化为指定类型的对象
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="obj">XML 报文</param>
        /// <returns>指定类型的对象</returns>
        public static T ToXmlObj<T>(this string obj)
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(obj));

            // 执行反序列化
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                T rlt = (T)xs.Deserialize(reader);
                return rlt;
            }
        }

        /// <summary>
        /// 把 XML 转换成字典类型
        /// </summary>
        /// <param name="resXml"></param>
        /// <param name="resTable"></param>
        /// <param name="errCode"></param>
        /// <param name="errText"></param>
        /// <returns></returns>
        public static Dictionary<string, string> XmlToDict(
            string resXml,
            out DataTable resTable,
            out int errCode,
            out string errText)
        {

            Dictionary<string, string> outParamList =
                new Dictionary<string, string>();
            errCode = 0;
            errText = "转换完成！";
            resTable = new DataTable("ResTable");

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(resXml);

                XmlNodeList nodeList = xml.SelectSingleNode("Result").ChildNodes;
                if (nodeList != null)
                {
                    foreach (XmlNode childNode in nodeList)
                    {
                        XmlElement childElement = (XmlElement)childNode;
                        if (childElement.Name == "Param")
                        {

                            foreach (XmlAttribute item in childElement.Attributes)
                            {
                                outParamList.Add(item.Name, item.Value);

                            }
                            errCode = int.Parse(childElement.Attributes["ErrCode"].Value.ToString());
                            errText = childElement.Attributes["ErrText"].Value.ToString();
                        }

                        if (childElement.Name == "ParamXML")
                        {
                            XmlNodeList datalist = childElement.ChildNodes;

                            XmlNode node = childElement.FirstChild;
                            //创建列
                            if (node != null)
                            {
                                foreach (XmlAttribute item in node.Attributes)
                                {
                                    DataColumn rowXmlColumn = new DataColumn();
                                    rowXmlColumn.DataType = System.Type.GetType("System.String");
                                    rowXmlColumn.ColumnName = item.Name;
                                    resTable.Columns.Add(rowXmlColumn);
                                }
                            }

                            foreach (XmlNode dataNode in datalist)
                            {
                                XmlElement data = (XmlElement)dataNode;
                                DataRow newRow;
                                newRow = resTable.NewRow();
                                foreach (XmlAttribute item in dataNode.Attributes)
                                {
                                    newRow[item.Name] = item.Value;
                                }
                                resTable.Rows.Add(newRow);
                            }
                        }
                    }

                }
                else
                {
                    errCode = 999999;
                    errText = "输入的 XML 不合法没有 Result 根元素！";
                }
            }
            catch (Exception ex)
            {
                errCode = 999999;
                errText = "输入的 XML 格式错误：" + ex.ToString();
            }

            return outParamList;
        }

        /// <summary>
        /// 将 DataTable 转换成 List&lt;T&gt;
        /// </summary>
        /// <typeparam name="T">转换后的对象类型</typeparam>
        /// <param name="dt">数据表</param>
        /// <returns>List&lt;T&gt;</returns>
        public static List<T> ToList<T>(DataTable dt)
        {
            var list = new List<T>();
            Type t = typeof(T);
            try
            {
                var plist = new List<PropertyInfo>(typeof(T).GetProperties());

                foreach (DataRow item in dt.Rows)
                {
                    T s = System.Activator.CreateInstance<T>();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        PropertyInfo info =
                            plist.Find(p => p.Name.ToLower() == dt.Columns[i].ColumnName.ToLower());
                        Type tType = info.PropertyType;

                        if (info != null)
                        {
                            if (!Convert.IsDBNull(item[i]))
                            {
                                object obj = Convert.ChangeType(item[i], tType);
                                info.SetValue(s, obj, null);
                            }
                        }
                    }
                    list.Add(s);
                }
            }
            catch (Exception error)
            {
                error.Data["ErrCode"] = 999999;
                error.Data["ErrText"] =
                    string.Format(
                        "[{0}]转换出错：[{1}]",
                        t.FullName.ToString(),
                        error.Message);
                throw error;
            }

            return list;
        }
    }
}
