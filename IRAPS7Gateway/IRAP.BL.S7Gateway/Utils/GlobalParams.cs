using IRAP.BL.S7Gateway.WebAPIClient.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAP.BL.S7Gateway.Utils
{
    /// <summary>
    /// 全局参数
    /// </summary>
    public class GlobalParams
    {
        private static GlobalParams _instance = null;

        private GlobalParams()
        {
            if (ConfigurationManager.AppSettings["CommunityID"] != null)
            {
                int rlt = 0;
                if (int.TryParse(
                    ConfigurationManager.AppSettings["CommunityID"],
                    out rlt))
                {
                    CommunityID = rlt;
                }
            }
            if (ConfigurationManager.AppSettings["MongoDBConnectionString"] != null)
            {
                MongoDBConnectionString =
                    ConfigurationManager.AppSettings["MongoDBConnectionString"];
            }
            else
            {
                MongoDBConnectionString = "http://127.0.0.1:27017";
            }
        }

        /// <summary>
        /// 全局参数唯一引用对象
        /// </summary>
        public static GlobalParams Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GlobalParams();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 社区标识
        /// </summary>
        public int CommunityID { get; private set; } = 0;
        /// <summary>
        /// WebAPI配置参数
        /// </summary>
        public WebAPIClientParams WebAPI { get; private set; } =
            new WebAPIClientParams();
        /// <summary>
        /// MongoDB的连接字符串
        /// </summary>
        public string MongoDBConnectionString { get; private set; } = "";
    }

    /// <summary>
    /// 调用WebAPI的配置参数
    /// </summary>
    public class WebAPIClientParams
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public WebAPIClientParams()
        {
            if (ConfigurationManager.AppSettings["WebAPIUrl"] != null)
            {
                URL = ConfigurationManager.AppSettings["WebAPIUrl"];
            }
            if (ConfigurationManager.AppSettings["ClientID"] != null)
            {
                ClientID = ConfigurationManager.AppSettings["ClientID"];
            }
            if (ConfigurationManager.AppSettings["ContentType"] != null)
            {
                try
                {
                    ContentType =
                        (ContentType)Enum.Parse(
                            typeof(ContentType),
                            ConfigurationManager.AppSettings["ContentType"]);
                }
                catch { }
            }
        }

        /// <summary>
        /// WebAPI 调用地址
        /// </summary>
        public string URL { get; set; } = "http://127.0.0.1:55559";
        /// <summary>
        /// 渠道标识
        /// </summary>
        public string ClientID { get; set; } = "sms";
        /// <summary>
        /// 报文格式
        /// </summary>
        public ContentType ContentType { get; set; } = ContentType.json;
    }
}
