using IRAP.BL.S7Gateway.Utils;
using IRAP.BL.S7Gateway.WebAPIClient.Enums;
using Logrila.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRAP.BL.S7Gateway.WebAPIClient
{
    /// <summary>
    /// IRAP的WebAPI调用父类
    /// </summary>
    public abstract class CustomWebAPICall
    {
        /// <summary>
        /// WebAPI 服务地址，由于是在配置文件中定义，因此无需向派生类开放
        /// </summary>
        private string webAPIUrl = "";
        /// <summary>
        /// 模块类别，需要在派生类中显式指定，因此需要向派生类开放。
        /// 默认值：Exchange
        /// </summary>
        protected ModuleType moduleType = ModuleType.Exchange;
        /// <summary>
        /// 报文格式，由于是在配置文件中定义，因此无需向派生类开放。
        /// 默认值：JSON
        /// </summary>
        private ContentType contentType = ContentType.json;
        /// <summary>
        /// 客户端标识，由于是在配置文件中定义，因此无需向派生类开放
        /// </summary>
        private string clientID = "Demo";
        /// <summary>
        /// 交易代码，需要在派生类中显示指定，因此需要向派生类开放
        /// </summary>
        protected string exCode = "";
        /// <summary>
        /// 交易执行后的结果消息对象（属性：ErrCode==0时，交易成功）
        /// </summary>
        private ErrorMessage errorMessage = new ErrorMessage();
        private ILog _log = null;

        private CustomWebAPICall()
        {
            _log = Logger.Get(GetType());

            webAPIUrl = GetValueFromAppSettings("WebAPIUrl", "http://127.0.0.1:55552/");

            string temp = GetValueFromAppSettings("ContentType", "json");
            try { contentType = (ContentType)Enum.Parse(typeof(ContentType), temp); }
            catch { contentType = ContentType.json; }

            clientID = GetValueFromAppSettings("ClientID", "MESDeveloper");
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="webAPIUrl">WebAPI地址</param>
        /// <param name="contentType">报文类型</param>
        /// <param name="clientID">渠道标识</param>
        public CustomWebAPICall(
            string webAPIUrl, 
            ContentType contentType, 
            string clientID): this()
        {
            this.webAPIUrl = webAPIUrl;
            this.contentType = contentType;
            this.clientID = clientID;
        }

        /// <summary>
        /// 交易执行后的结果消息对象（属性：ErrCode==0时，交易成功）
        /// </summary>
        public ErrorMessage Error { get { return errorMessage; } }

        /// <summary>
        /// 根据指定的关键字，从应用程序配置文件中读取配置的值，如果没有则返回 defaultValue 中指定的值
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="defaultValue">指定的缺省值</param>
        /// <returns>配置文件中配置的</returns>
        private string GetValueFromAppSettings(string key, string defaultValue = "")
        {
            if (ConfigurationManager.AppSettings[key] != null)
                return ConfigurationManager.AppSettings["key"];
            else
                return defaultValue;
        }

        /// <summary>
        /// 调用 WebAPI 服务，发送请求报文，接收响应报文后，转换成指定类型的对象
        /// </summary>
        /// <typeparam name="T">返回的对象类型</typeparam>
        /// <param name="requestObject">请求报文对象</param>
        /// <param name="result">交易结果对象</param>
        /// <returns>返回的对象</returns>
        protected T Call<T>(
            object requestObject,
            out ErrorMessage result)
        {
            string url = "";
            result = new ErrorMessage();

            string moduleTypeString =
                Enum.GetName(typeof(ModuleType), moduleType);
            string contentTypeString =
                Enum.GetName(typeof(ContentType), contentType);

            switch (moduleType)
            {
                case ModuleType.Exchange:
                    url =
                        string.Format(
                            "{0}{1}/{2}/{3}/{4}",
                            webAPIUrl,
                            moduleTypeString,
                            clientID,
                            contentTypeString,
                            exCode);
                    break;
                default:
                    string errText = $"目前不支持模块 [{moduleTypeString}]";
                    Exception error = new Exception(errText);
                    error.Data["ErrCode"] = 999999;
                    error.Data["ErrText"] = errText;
                    throw error;
            }

            switch (contentType)
            {
                case ContentType.json:
                    break;
                default:
                    string errText = $"目前不支持 [{contentTypeString}] 报文格式";
                    Exception error = new Exception(errText);
                    error.Data["ErrCode"] = 999999;
                    error.Data["ErrText"] = errText;
                    throw error;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/stream;";
            request.KeepAlive = false;
            request.AllowAutoRedirect = true;
            request.CookieContainer = new CookieContainer();
            request.Timeout = 30000;        // 单位：毫秒

            try
            {
                Stream stream = request.GetRequestStream();

                #region 根据传入的指定报文格式，生成交易请求对象的相应格式的报文文本
                string content = "";
                switch (contentType)
                {
                    case ContentType.json:
                        content = requestObject.ToJSON();
                        break;
                    case ContentType.xml:
                        content = requestObject.ToXML();
                        break;
                }
                #endregion

                _log.Info($"请求报文：[{content}]");

                byte[] requestContext = Encoding.UTF8.GetBytes(content);
                stream.Write(requestContext, 0, requestContext.Length);
                stream.Flush();
                stream.Close();
                Application.DoEvents();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string resJson = "";
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    resJson = sr.ReadToEnd();
                }

                _log.Info($"响应报文：[{resJson}]");

                T rtnObject = default(T);
                switch (contentType)
                {
                    case ContentType.json:
                        result = JsonConvert.DeserializeObject<ErrorMessage>(resJson);
                        rtnObject = JsonConvert.DeserializeObject<T>(resJson);
                        break;
                }

                result.SourceREQContent = content;
                result.SourceRESPContent = resJson;

                return rtnObject;
            }
            catch (Exception error)
            {
                error.Data["ErrCode"] = 999999;
                error.Data["ErrText"] = error.Message;
                throw error;
            }
        }

        /// <summary>
        /// 执行具体的报文发送和接收交易
        /// </summary>
        /// <param name="errorMsg">交易执行结果对象</param>
        protected abstract void Communicate(out ErrorMessage errorMsg);

        /// <summary>
        /// 执行交易
        /// </summary>
        /// <returns>true-交易成功；false-交易失败</returns>
        public virtual bool Do()
        {
            try
            {
                Communicate(out errorMessage);
                if (errorMessage.ErrCode == 0)
                {
                    _log.Debug($"[({errorMessage.ErrCode}){errorMessage.ErrText}");
                }
                else
                {
                    _log.Error($"[({errorMessage.ErrCode}){errorMessage.ErrText}");
                }
                return errorMessage.ErrCode == 0;
            }
            catch (Exception error)
            {
                _log.Error(error.Message, error);
                return false;
            }
        }
    }
}
