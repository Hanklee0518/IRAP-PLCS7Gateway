using IRAP.BL.S7Gateway.Utils;
using IRAP.BL.S7Gateway.WebAPIClient.Contents;
using IRAP.BL.S7Gateway.WebAPIClient.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAP.BL.S7Gateway.WebAPIClient.Exchange.DCS
{
    /// <summary>
    /// 设备故障告警请求报文
    /// </summary>
    public class EquipFailAndonCallRequest : CustomPLCRequest
    {
        /// <summary>
        /// 交易代码
        /// </summary>
        public override string ExCode => "IRAP_DCS_EquipFailAndonCall";
        /// <summary>
        /// 设备故障告警请求报文体
        /// </summary>
        public EquipFailAndonCallParamXML ParamXML { get; set; } =
            new EquipFailAndonCallParamXML();
    }

    /// <summary>
    /// 设备故障告警请求报文体
    /// </summary>
    public class EquipFailAndonCallParamXML
    {
        /// <summary>
        /// 设备故障第1组
        /// </summary>
        public uint Equipment_Failures_Group_1 { get; set; } = 0;
        /// <summary>
        /// 设备故障第2组
        /// </summary>
        public uint Equipment_Failures_Group_2 { get; set; } = 0;
        /// <summary>
        /// 设备故障第3组
        /// </summary>
        public uint Equipment_Failures_Group_3 { get; set; } = 0;
        /// <summary>
        /// 设备故障第4组
        /// </summary>
        public uint Equipment_Failures_Group_4 { get; set; } = 0;
        /// <summary>
        /// 设备故障第5组
        /// </summary>
        public uint Equipment_Failures_Group_5 { get; set; } = 0;
        /// <summary>
        /// 设备故障第6组
        /// </summary>
        public uint Equipment_Failures_Group_6 { get; set; } = 0;
        /// <summary>
        /// 设备故障第7组
        /// </summary>
        public uint Equipment_Failures_Group_7 { get; set; } = 0;
        /// <summary>
        /// 设备故障第8组
        /// </summary>
        public uint Equipment_Failures_Group_8 { get; set; } = 0;
        /// <summary>
        /// 设备故障代码
        /// </summary>
        public string Failure_Code { get; set; } = "";
    }

    /// <summary>
    /// 设备故障告警响应报文
    /// </summary>
    public class EquipFailAndonCallResponse : CustomResponse
    {

    }

    /// <summary>
    /// 设备故障告警
    /// </summary>
    public class EquipFailAndonCall : CustomWebAPICall
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="webAPIUrl">WebAPI地址</param>
        /// <param name="contentType">报文类型</param>
        /// <param name="clientID">渠道标识</param>
        /// <param name="logEntity">交易日志实体对象</param>
        public EquipFailAndonCall(
            string webAPIUrl,
            ContentType contentType,
            string clientID,
            DCSGatewayLogEntity logEntity) : 
            base(webAPIUrl, contentType, clientID, logEntity)
        {
            moduleType = ModuleType.Exchange;
            ExCode = "IRAP_DCS_EquipFailAndonCall";
        }

        /// <summary>
        /// 请求报文
        /// </summary>
        public EquipFailAndonCallRequest Request { get; set; } = null;
        /// <summary>
        /// 响应报文
        /// </summary>
        public EquipFailAndonCallResponse Response { get; private set; } = null;

        /// <summary>
        /// 发送请求报文并接受响应报文
        /// </summary>
        /// <param name="errorMsg">交易执行结果对象</param>
        protected override void Communicate(out ErrorMessage errorMsg)
        {
            if (Request == null)
            {
                Exception error =
                    new Exception(
                        "请求报文[EquipFailAndonCallRequest]未实例化");
                errorMsg =
                    new ErrorMessage()
                    {
                        ErrCode = 999999,
                        ErrText = error.Message,
                    };
                return;
            }

            Response =
                Call<EquipFailAndonCallResponse>(Request, out errorMsg);
        }
    }
}
