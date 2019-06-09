using IRAP.BL.S7Gateway.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IRAP.BL.S7Gateway.Entities
{
    public abstract class TCustomTKDevice
    {
        protected byte[] hashBytes = null;

        protected TS7GroupCOMM comm = null;

        protected WIPStation wipStation = null;

        /// <summary>
        /// 回写到设备 PLC 中
        /// </summary>
        public event S7WriteBackHandler OnWriteback;

        public abstract uint DBNumber { get; }
        public abstract int BufferSize { get; }

        public TS7GroupCOMM COMM { get => comm; }

        public WIPStation WIPStation { get => wipStation; }

        /// <summary>
        /// 最后一次发送心跳时间
        /// </summary>
        public DateTime LastMESHeartBeatTime { get; set; } = DateTime.Now;

        protected byte[] CalculateHash(byte[] buffer)
        {
            HashAlgorithm hash = HashAlgorithm.Create();
            return hash.ComputeHash(buffer);
        }

        public void DoSomething(byte[] buffer)
        {
            byte[] newHashBytes = CalculateHash(buffer);
            if (Tools.ByteEquals(hashBytes, newHashBytes))
            {
                ;
            }
            else
            {
                hashBytes = newHashBytes;

                //Console.WriteLine($"Buffer Size={buffer.Length}");
                //for (int i = 0; i < buffer.Length; i++)
                //{
                //    Console.Write($"{string.Format("{0:x2}", buffer[i])} ");
                //}
                //Console.WriteLine();

                DBDataChanged(buffer);
            }
        }

        /// <summary>
        /// 数据块内容发生变化的后续处理
        /// </summary>
        public abstract void DBDataChanged(byte[] buffer);

        protected bool GetBitValue(byte data, int pos)
        {
            return (data >> pos & 0x1) == 1;
        }

        protected Int16 GetWordValue(byte[] buffer, int pos)
        {
            byte[] value = new byte[2];
            Array.Copy(buffer, pos, value, 0, 2);

            return BitConverter.ToInt16(value, 0);
        }

        protected Int32 GetDWordValue(byte[] buffer, int pos)
        {
            byte[] value = new byte[4];
            Array.Copy(buffer, pos, value, 0, 4);

            return BitConverter.ToInt32(value, 0);
        }

        protected string GetStringValue(byte[] buffer, int pos, int length)
        {
            byte[] value = new byte[length];
            Array.Copy(buffer, pos, value, 0, length);

            return Encoding.Default.GetString(value);
        }

        protected void DealBoolOfTag(byte[] buffer, BoolOfTag tag, EventHandler postProcessing)
        {
            if (tag.Used)
            {
                bool value = GetBitValue(buffer[tag.DB_Offset], (int)tag.Position);
                if (tag.Value != value)
                {
                    tag.Value = value;
                    if (!(tag.TagName.Contains("MES_Heart_Beat") ||
                        tag.TagName.Contains("PLC_Heart_Beat")))
                    {
                        Console.WriteLine(
                            $"{DateTime.Now.ToString("HH:mm:ss.fff")}:" +
                            $"DB{DBNumber}.DBX{tag.DB_Offset}." +
                            $"{tag.Position}={value}");
                    }

                    postProcessing?.Invoke(tag, null);
                }
            }
        }

        protected void DealByteOfTag(byte[] buffer, ByteOfTag tag, EventHandler postProcessing)
        {
            if (tag.Used)
            {
                if (tag.Value != buffer[tag.DB_Offset])
                {
                    tag.Value = buffer[tag.DB_Offset];
                    Console.WriteLine(
                        $"{DateTime.Now.ToString("HH:mm:ss.fff")}:" +
                        $"DB{DBNumber}.DBB{tag.DB_Offset}={tag.Value}");

                    postProcessing?.Invoke(tag, null);
                }
            }
        }

        protected void DealWordOfTag(byte[] buffer, WordOfTag tag, EventHandler postProcessing)
        {
            if (tag.Used)
            {
                Int16 value = GetWordValue(buffer, (int)tag.DB_Offset);
                if (tag.Value != value)
                {
                    tag.Value = value;
                    Console.WriteLine(
                        $"{DateTime.Now.ToString("HH:mm:ss.fff")}:" +
                        $"DB{DBNumber}.DBW{tag.DB_Offset}={tag.Value}");

                    postProcessing?.Invoke(tag, null);
                }
            }
        }

        protected void DealDWordOfTag(byte[] buffer, DWordOfTag tag, EventHandler postProcessing)
        {
            if (tag.Used)
            {
                Int32 value = GetDWordValue(buffer, (int)tag.DB_Offset);
                if (tag.Value != value)
                {
                    tag.Value = value;
                    Console.WriteLine(
                        $"{DateTime.Now.ToString("HH:mm:ss.fff")}:" +
                        $"DB{DBNumber}.DBDW{tag.DB_Offset}={tag.Value}");

                    postProcessing?.Invoke(tag, null);
                }
            }
        }

        protected void DealArrayCharOfTag(byte[] buffer, ArrayCharOfTag tag, EventHandler postProcessing)
        {
            if (tag.Used)
            {
                string value = GetStringValue(buffer, (int)tag.DB_Offset, (int)tag.Length);
                if (tag.Value != value)
                {
                    tag.Value = value;
                    Console.WriteLine(
                        $"{DateTime.Now.ToString("HH:mm:ss.fff")}:" +
                        $"DB{DBNumber}.DBB{tag.DB_Offset}=[{tag.Value}]");

                    postProcessing?.Invoke(tag, null);
                }
            }
        }

        /// <summary>
        /// 回写指定 Tag 的值
        /// </summary>
        protected void WriteToPLC(TCustomTKDevice device, CustomTag tag)
        {
            if (OnWriteback != null)
            {
                OnWriteback(device, tag);
            }
            else
            {
                Console.WriteLine("未指定回写事件，无法回写到 PLC 中");
            }
        }
    }

    public class HMETurnTable : TCustomTKDevice
    {
        public HMETurnTable()
        {
            #region 设置 COMM Tag 组
            comm = new TS7GroupCOMM();
            comm.Equipment_Running_Mode.DB_Offset = 0;
            comm.Equipment_Running_Mode.Position = 0;

            comm.Domination_Status.DB_Offset = 0;
            comm.Domination_Status.Position = 1;

            comm.PLC_Heart_Beat.DB_Offset = 0;
            comm.PLC_Heart_Beat.Position = 2;

            comm.Equipment_Power_On.DB_Offset = 0;
            comm.Equipment_Power_On.Position = 3;

            comm.Equipment_Fail.DB_Offset = 0;
            comm.Equipment_Fail.Position = 4;

            comm.Tool_Fail.DB_Offset = 0;
            comm.Tool_Fail.Position = 5;

            comm.Cycle_Started.DB_Offset = 0;
            comm.Cycle_Started.Position = 6;

            comm.Equipment_Starvation.DB_Offset = 0;
            comm.Equipment_Starvation.Position = 7;

            comm.MES_Heart_Beat.DB_Offset = 1;
            comm.MES_Heart_Beat.Position = 0;
            #endregion

            #region 设置 WIPStation Tag 组
            wipStation = new WIPStation()
            {
                Prefix = "01",
            };
            wipStation.WIP_Station_LeafID.DB_Offset = 2;
            wipStation.Product_Number.DB_Offset = 6;
            wipStation.WIP_Code.DB_Offset = 46;
            wipStation.WIP_ID_Type_Code.DB_Offset = 66;
            wipStation.WIP_ID_Code.DB_Offset = 68;

            wipStation.WIP_Move_In.DB_Offset = 148;
            wipStation.WIP_Move_In.Position = 0;

            wipStation.Request_For_Poka_Yoke.DB_Offset = 148;
            wipStation.Request_For_Poka_Yoke.Position = 1;

            wipStation.Is_NG_WIP_Onto_Line_Station.DB_Offset = 148;
            wipStation.Is_NG_WIP_Onto_Line_Station.Position = 2;

            wipStation.Error_proofing_detection.DB_Offset = 148;
            wipStation.Error_proofing_detection.Position = 3;

            wipStation.Poka_Yoke_Result.DB_Offset = 149;
            wipStation.Operation_Conclusion.DB_Offset = 150;
            #endregion
        }

        public override uint DBNumber => 30;

        public override int BufferSize => 151;

        public override void DBDataChanged(byte[] buffer)
        {
            DealBoolOfTag(buffer, comm.Equipment_Running_Mode, null);
            DealBoolOfTag(buffer, comm.Domination_Status, null);
            DealBoolOfTag(buffer, comm.PLC_Heart_Beat, null);
            DealBoolOfTag(buffer, comm.Equipment_Power_On, null);
            DealBoolOfTag(buffer, comm.Equipment_Fail, null);
            DealBoolOfTag(buffer, comm.Tool_Fail, null);
            DealBoolOfTag(buffer, comm.Cycle_Started, null);
            DealBoolOfTag(buffer, comm.Equipment_Starvation, null);
            DealBoolOfTag(buffer, comm.MES_Heart_Beat, null);

            DealDWordOfTag(buffer, wipStation.WIP_Station_LeafID, null);
            DealArrayCharOfTag(buffer, wipStation.Product_Number, null);
            DealArrayCharOfTag(buffer, wipStation.WIP_Code, null);
            DealArrayCharOfTag(buffer, wipStation.WIP_ID_Type_Code, null);
            DealArrayCharOfTag(buffer, wipStation.WIP_ID_Code, null);
            DealBoolOfTag(buffer, wipStation.WIP_Move_In, DealWithWIPMoveIn);
            DealBoolOfTag(buffer, wipStation.Request_For_Poka_Yoke, DealWithRequestForPokaYoke);
            DealBoolOfTag(buffer, wipStation.Is_NG_WIP_Onto_Line_Station, null);
            DealBoolOfTag(buffer, wipStation.Error_proofing_detection, null);
            DealByteOfTag(buffer, wipStation.Poka_Yoke_Result, null);
            DealByteOfTag(buffer, wipStation.Operation_Conclusion, null);
        }

        private void DealWithRequestForPokaYoke(object sender, EventArgs e)
        {
            Thread.Sleep(40);

            if (wipStation.Request_For_Poka_Yoke.Value)
            {
                Console.WriteLine("开始进行防错校验");
                wipStation.Poka_Yoke_Result.Value = 1;
                WriteToPLC(this, wipStation.Poka_Yoke_Result);
                WIPStation.Request_For_Poka_Yoke.Value = false;
                WriteToPLC(this, WIPStation.Request_For_Poka_Yoke);
                Console.WriteLine("防错校验完成");
            }
        }

        private void DealWithWIPMoveIn(object sender, EventArgs e)
        {
            Thread.Sleep(40);

            if (wipStation.WIP_Move_In.Value)
            {
                Console.WriteLine("开始执行 WIP Move In 步骤");
                Console.WriteLine("Execute the WIP Move In transaction");
                wipStation.WIP_Move_In.Value = false;
                WriteToPLC(this, wipStation.WIP_Move_In);
                Console.WriteLine("WIP Move In 执行完成");
            }
        }
    }

    public class HMEFlash : TCustomTKDevice
    {
        private TS7GroupProperty property = new TS7GroupProperty();
        private TS7GroupStagnation stagnation = new TS7GroupStagnation();
        private TS7GroupEquipmentFail equipmentFail = new TS7GroupEquipmentFail();
        private TS7GroupSafetyProblem safetyProblem = new TS7GroupSafetyProblem();

        public HMEFlash()
        {
            #region COMM Tag 组
            comm = new TS7GroupCOMM();
            comm.Equipment_Running_Mode.SetPosition(0, 0);
            comm.Domination_Status.SetPosition(0, 1);
            comm.PLC_Heart_Beat.SetPosition(0, 2);
            comm.Equipment_Power_On.SetPosition(0, 3);
            comm.Equipment_Fail.SetPosition(0, 4);
            comm.Tool_Fail.SetPosition(0, 5);
            comm.Cycle_Started.SetPosition(0, 6);
            comm.Equipment_Starvation.SetPosition(0, 7);
            comm.MES_Heart_Beat.SetPosition(1, 0);
            #endregion

            #region WIPStation 组
            wipStation = new WIPStation()
            {
                Prefix = "01",
            };
            wipStation.WIP_Station_LeafID.DB_Offset = 2;
            wipStation.Product_Number.DB_Offset = 6;
            wipStation.WIP_Code.DB_Offset = 46;
            wipStation.WIP_ID_Type_Code.DB_Offset = 66;
            wipStation.WIP_ID_Code.DB_Offset = 68;

            wipStation.WIP_Move_In.SetPosition(148, 0);
            wipStation.Production_Start.SetPosition(148, 1);
            wipStation.Production_End.SetPosition(148, 2);
            wipStation.WIP_Move_Out.SetPosition(148, 3);
            wipStation.Change_Over_Start.SetPosition(148, 4);
            wipStation.Change_Over_End.SetPosition(148, 5);
            wipStation.Production_Pause.SetPosition(148, 6);
            wipStation.Production_Restart.SetPosition(148, 7);
            wipStation.Request_For_Poka_Yoke.SetPosition(149, 0);
            wipStation.Move_To_MRB_Operation.SetPosition(149, 1);
            wipStation.Request_For_Routing.SetPosition(149, 2);
            wipStation.Request_For_ID_Binding.SetPosition(149, 3);
            wipStation.Trigger_Equipment_Fail_Andon.SetPosition(149, 4);
            wipStation.Trigger_Quality_Problem_Andon.SetPosition(149, 5);
            wipStation.Stagnation_Warnning.SetPosition(149, 6);
            wipStation.Serial_Number_Request.SetPosition(149, 7);
            wipStation.Label_ELements_Request.SetPosition(150, 0);
            wipStation.Label_Print_Request.SetPosition(150, 1);
            wipStation.Label_Reprint_Request.SetPosition(150, 2);
            wipStation.Packing_Succeeded.SetPosition(150, 3);
            wipStation.Is_OK_WIP_Onto_Line_Station.SetPosition(150, 4);
            wipStation.Is_NG_WIP_Onto_Line_Station.SetPosition(150, 5);
            wipStation.Scheduled_Production_Down.SetPosition(150, 6);
            wipStation.Error_proofing_detection.SetPosition(150, 7);

            wipStation.Poka_Yoke_Result.DB_Offset = 152;
            wipStation.Operation_Conclusion.DB_Offset = 153;
            #endregion

            #region Property Tag 组
            property = new TS7GroupProperty();
            property.Equipment_End_Definition.SetPosition(154, 0);
            #endregion

            #region Stagnation Tag 组
            stagnation = new TS7GroupStagnation();
            stagnation.Time_In_Seconds.DB_Offset = 156;
            stagnation.Threshold.DB_Offset = 160;
            #endregion

            #region EquipmentFail Tag 组
            equipmentFail = new TS7GroupEquipmentFail();
            equipmentFail.Equipment_Failures_Group_1.DB_Offset = 162;
            equipmentFail.Equipment_Failures_Group_2.DB_Offset = 166;
            equipmentFail.Equipment_Failures_Group_3.DB_Offset = 170;
            equipmentFail.Equipment_Failures_Group_4.DB_Offset = 174;
            equipmentFail.Equipment_Failures_Group_5.DB_Offset = 178;
            equipmentFail.Equipment_Failures_Group_6.DB_Offset = 182;
            equipmentFail.Equipment_Failures_Group_7.DB_Offset = 186;
            equipmentFail.Equipment_Failures_Group_8.DB_Offset = 190;
            equipmentFail.Failure_Code.DB_Offset = 194;
            #endregion

            #region SafetyProblem Tag 组
            safetyProblem = new TS7GroupSafetyProblem();
            safetyProblem.Safety_Issue_Type.DB_Offset = 204;
            #endregion
        }

        public override uint DBNumber => 29;

        public override int BufferSize => 205;

        public override void DBDataChanged(byte[] buffer)
        {
            #region COMM
            DealBoolOfTag(buffer, comm.Equipment_Running_Mode, null);
            DealBoolOfTag(buffer, comm.Domination_Status, null);
            DealBoolOfTag(buffer, comm.Equipment_Power_On, null);
            DealBoolOfTag(buffer, comm.Equipment_Fail, null);
            DealBoolOfTag(buffer, comm.Tool_Fail, null);
            DealBoolOfTag(buffer, comm.Cycle_Started, null);
            DealBoolOfTag(buffer, comm.Equipment_Starvation, null);
            DealBoolOfTag(buffer, comm.MES_Heart_Beat, null);
            #endregion

            #region WIPStation
            DealDWordOfTag(buffer, wipStation.WIP_Station_LeafID, null);
            DealArrayCharOfTag(buffer, wipStation.Product_Number, null);
            DealArrayCharOfTag(buffer, wipStation.WIP_Code, null);
            DealArrayCharOfTag(buffer, wipStation.WIP_ID_Type_Code, null);
            DealArrayCharOfTag(buffer, wipStation.WIP_ID_Code, null);

            DealBoolOfTag(buffer, wipStation.WIP_Move_In, DealWithWIPMoveIn);
            DealBoolOfTag(buffer, wipStation.Production_Start, null);
            DealBoolOfTag(buffer, wipStation.Production_End, null);
            DealBoolOfTag(buffer, wipStation.WIP_Move_Out, null);
            DealBoolOfTag(buffer, wipStation.Change_Over_Start, null);
            DealBoolOfTag(buffer, wipStation.Change_Over_End, null);
            DealBoolOfTag(buffer, wipStation.Production_Pause, null);
            DealBoolOfTag(buffer, wipStation.Production_Restart, null);
            DealBoolOfTag(buffer, wipStation.Request_For_Poka_Yoke, DealWithRequestForPokaYoke);
            DealBoolOfTag(buffer, wipStation.Move_To_MRB_Operation, null);
            DealBoolOfTag(buffer, wipStation.Request_For_Routing, null);
            DealBoolOfTag(buffer, wipStation.Request_For_ID_Binding, null);
            DealBoolOfTag(buffer, wipStation.Trigger_Equipment_Fail_Andon, null);
            DealBoolOfTag(buffer, wipStation.Trigger_Quality_Problem_Andon, null);
            DealBoolOfTag(buffer, wipStation.Stagnation_Warnning, null);
            DealBoolOfTag(buffer, wipStation.Serial_Number_Request, null);
            DealBoolOfTag(buffer, wipStation.Label_ELements_Request, null);
            DealBoolOfTag(buffer, wipStation.Label_Print_Request, null);
            DealBoolOfTag(buffer, wipStation.Label_Reprint_Request, null);
            DealBoolOfTag(buffer, wipStation.Packing_Succeeded, null);
            DealBoolOfTag(buffer, wipStation.Is_OK_WIP_Onto_Line_Station, null);
            DealBoolOfTag(buffer, wipStation.Is_NG_WIP_Onto_Line_Station, null);
            DealBoolOfTag(buffer, wipStation.Scheduled_Production_Down, null);
            DealBoolOfTag(buffer, wipStation.Error_proofing_detection, null);

            DealByteOfTag(buffer, wipStation.Poka_Yoke_Result, null);
            DealByteOfTag(buffer, wipStation.Operation_Conclusion, null);
            #endregion

            #region Property
            DealBoolOfTag(buffer, property.Equipment_End_Definition, null);
            #endregion

            #region Stagnation
            DealDWordOfTag(buffer, stagnation.Time_In_Seconds, null);
            DealWordOfTag(buffer, stagnation.Threshold, null);
            #endregion

            #region EquipmentFail
            DealDWordOfTag(buffer, equipmentFail.Equipment_Failures_Group_1, null);
            DealDWordOfTag(buffer, equipmentFail.Equipment_Failures_Group_2, null);
            DealDWordOfTag(buffer, equipmentFail.Equipment_Failures_Group_3, null);
            DealDWordOfTag(buffer, equipmentFail.Equipment_Failures_Group_4, null);
            DealDWordOfTag(buffer, equipmentFail.Equipment_Failures_Group_5, null);
            DealDWordOfTag(buffer, equipmentFail.Equipment_Failures_Group_6, null);
            DealDWordOfTag(buffer, equipmentFail.Equipment_Failures_Group_7, null);
            DealDWordOfTag(buffer, equipmentFail.Equipment_Failures_Group_8, null);
            DealArrayCharOfTag(buffer, equipmentFail.Failure_Code, null);
            #endregion

            #region SafetyProblem
            DealByteOfTag(buffer, safetyProblem.Safety_Issue_Type, null);
            #endregion
        }

        private void DealWithRequestForPokaYoke(object sender, EventArgs e)
        {
            Thread.Sleep(40);

            if (wipStation.Request_For_Poka_Yoke.Value)
            {
                Console.WriteLine("开始进行防错校验");
                wipStation.Poka_Yoke_Result.Value = 1;
                WriteToPLC(this, wipStation.Poka_Yoke_Result);
                WIPStation.Request_For_Poka_Yoke.Value = false;
                WriteToPLC(this, WIPStation.Request_For_Poka_Yoke);
                Console.WriteLine("防错校验完成");
            }
        }

        private void DealWithWIPMoveIn(object sender, EventArgs e)
        {
            Thread.Sleep(40);

            if (wipStation.WIP_Move_In.Value)
            {
                Console.WriteLine("开始执行 WIP Move In 步骤");
                Console.WriteLine("Execute the WIP Move In transaction");
                wipStation.WIP_Move_In.Value = false;
                WriteToPLC(this, wipStation.WIP_Move_In);
                Console.WriteLine("WIP Move In 执行完成");
            }
        }
    }
}
