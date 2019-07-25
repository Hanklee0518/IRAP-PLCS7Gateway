using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace IRAP.BL.S7Gateway.Utils
{
    /// <summary>
    /// JSON转换类
    /// </summary>
    public static class JSONHelper
    {
        /// <summary>
        /// JSON 序列化
        /// </summary>
        /// <param name="obj">源对象</param>
        /// <returns>以 JSON 序列化源对象后的内容</returns>
        public static string ToJSON(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 将字符串数组转换成 JSON 数据格式：["Value1", "Value2", ...]
        /// </summary>
        /// <param name="strs">字符串数组</param>
        /// <returns>JSON 数据格式的字符串</returns>
        public static string ToJSON(this string[] strs)
        {
            return ToJSON((object)strs);
        }

        /// <summary>
        /// 将 DataTable 数据源转换为 JSON 数据格式：
        /// [{"ColumnName":"ColumnValue",...},{"ColumnName":"ColumnValue",...},...]
        /// </summary>
        /// <param name="dt">DataTable 数据源</param>
        /// <returns>JSON 数据格式的字符串</returns>
        public static string ToJSON(this DataTable dt)
        {
            return JsonConvert.SerializeObject(dt, new DataTableConverter());
        }

        /// <summary>
        /// 将 JSON 格式的日期时间（"\/Date(673286400000)\/" 转换成 "YYYY-MM-dd HH:mm:ss" 格式
        /// </summary>
        /// <param name="jsonDateTimeString">JSON 格式的日期时间</param>
        /// <returns></returns>
        public static string ConvertToDateTimeString(this string jsonDateTimeString)
        {
            string result = "";
            string p = @"\\/Date\((d+\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJSONDateToDateString);
            Regex reg = new Regex(p);
            result = reg.Replace(jsonDateTimeString, matchEvaluator);
            return result;
        }

        /// <summary>
        /// 将JSON格式的日期转换成日期字符串
        /// </summary>
        /// <param name="match">正则表达式</param>
        /// <returns>日期字符串</returns>
        public static string ConvertJSONDateToDateString(Match match)
        {
            string result = "";
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }

        /// <summary>
        /// 根据简单的 JSON 格式字符串返回动态类型
        /// </summary>
        /// <param name="jsonString">JSON 格式字符串</param>
        /// <returns>动态类型的结果</returns>
        public static dynamic GetSimpleObjectFromJson(this string jsonString)
        {
            dynamic d = new ExpandoObject();
            // 将JSON字符串反序列化
            JavaScriptSerializer s = new JavaScriptSerializer();
            object resobj = s.DeserializeObject(jsonString);

            // 拷贝数据
            IDictionary<string, object> dic = (IDictionary<string, object>)resobj;
            IDictionary<string, object> dicdyn = (IDictionary<string, object>)d;

            foreach (var item in dic)
            {
                dicdyn.Add(item.Key, item.Value);
            }
            return d;
        }

        /// <summary>
        /// 把 JSON 格式字符串反序列化为 List&lt;T&gt; 对象
        /// </summary>
        /// <typeparam name="T">反序列化对象类型</typeparam>
        /// <param name="jsonString">JSON 格式的字符串</param>
        /// <returns>List&lt;T&gt;</returns>
        public static List<T> GetListFromJSON<T>(this string jsonString)
        {

            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483647;
            IList<T> list = s.Deserialize<IList<T>>(jsonString);
            return list.ToList();
        }

        /// <summary>
        /// 把 JSON 格式字符串反序列化为 T 对象
        /// </summary>
        /// <typeparam name="T">反序列化对象类型</typeparam>
        /// <param name="jsonString">JSON 格式的字符串</param>
        /// <returns>T 对象</returns>
        public static T GetObjectFromJSON<T>(this string jsonString)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            T obj = s.Deserialize<T>(jsonString);
            return obj;
        }

        /// <summary>
        /// 把Get参数用字典的形式返回
        /// </summary>
        /// <remarks>参数格式：Key1=Value1&amp;Key2=Value&amp;Key3=Value3</remarks>
        /// <param name="sourceString">参数字符串</param>
        /// <returns>参数字典</returns>
        public static Dictionary<string, string> ToDict(this string sourceString)
        {
            Dictionary<string, string> keys = new Dictionary<string, string>();
            try
            {
                string[] inparam = sourceString.Split('&');
                foreach (string item in inparam)
                {
                    string[] keyvalue = item.Split('=');
                    keys.Add(keyvalue[0], keyvalue[1]);
                };
                return keys;
            }
            catch (Exception)
            {
                return keys;
            }
        }

        /// <summary>
        /// 把 JSON Array 字符串转换为对象
        /// </summary>
        /// <param name="jsonString">JSON字符串</param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> ToRows(this string jsonString)
        {

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            JavaScriptSerializer s = new JavaScriptSerializer();
            object[] resobj = (object[])s.DeserializeObject(jsonString);

            // 拷贝数据
            foreach (Dictionary<string, object> item in resobj)
            {
                rows.Add(item);
            }
            return rows;
        }
    }
}
