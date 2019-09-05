using Logrila.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAP.BL.S7Gateway.Utils
{
    internal class Log4MongoDB
    {
        public static void WriteLog(DCSGatewayLogEntity log)
        {
            //string strConn = "mongodb://admin:admin123@127.0.0.1:27017";
            //string strConn = "mongodb://10.230.69.64:27017";
            string dbName = "DCSLog";
            string collectionName = "DCSGatewayLog";

            try
            {
                MongoClient client = new MongoClient(GlobalParams.Instance.MongoDBConnectionString);
                IMongoDatabase db = client.GetDatabase(dbName);
                IMongoCollection<BsonDocument> collection =
                    db.GetCollection<BsonDocument>(collectionName);

                string json = JsonConvert.SerializeObject(log);
                BsonDocument bson = BsonSerializer.Deserialize<BsonDocument>(json);
                collection.InsertOne(bson);
            }
            catch (Exception error)
            {
                ILog _log = Logger.Get<Log4MongoDB>();
                _log.Error(error.Message, error);
            }
        }
    }

    /// <summary>
    /// DCS网关交易日志实体
    /// </summary>
    public class DCSGatewayLogEntity
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public DCSGatewayLogEntity()
        {
            BeginTime = DateTime.Now;
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 操作名称
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// 操作代码
        /// </summary>
        public string ActionCode { get; set; }
        /// <summary>
        /// 触发信号标签信息
        /// </summary>
        public LogTagEntity TriggerTag { get; set; } = new LogTagEntity();
        /// <summary>
        /// 执行时间(毫秒)
        /// </summary>
        public long TotalExecutionTime { get; set; }
        /// <summary>
        /// 开始DCS接口调用日志
        /// </summary>
        public DCSTradeLogEntity StartDCSInvokingLog { get; set; } =
            new DCSTradeLogEntity();
        /// <summary>
        /// DCS主交易日志
        /// </summary>
        public DCSTradeLogEntity MainTradeLog { get; set; } = new DCSTradeLogEntity();
        /// <summary>
        /// 执行期间记录的错误信息集
        /// </summary>
        public List<Exception> Errors { get; set; } = new List<Exception>();
    }

    /// <summary>
    /// 标记日志实体
    /// </summary>
    public class LogTagEntity
    {
        /// <summary>
        /// 标记名称
        /// </summary>
        public string TagName { get; set; }
        /// <summary>
        /// 偏移量
        /// </summary>
        public string Offset { get; set; }
    }

    /// <summary>
    /// DCS交易日志实体
    /// </summary>
    public class DCSTradeLogEntity
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public string Excode { get; set; }
        /// <summary>
        /// 请求发送时间
        /// </summary>
        public DateTime RequestTime { get; set; }
        /// <summary>
        /// 请求对象
        /// </summary>
        public object RequestObject { get; set; }
        /// <summary>
        /// 请求报文
        /// </summary>
        public string RequestContent { get; set; }
        /// <summary>
        /// 响应收到时间
        /// </summary>
        public DateTime ResponseTime { get; set; }
        /// <summary>
        /// 响应对象
        /// </summary>
        public object ResponseObject { get; set; }
        /// <summary>
        /// 响应报文
        /// </summary>
        public string ResponseContent { get; set; }
    }
}
