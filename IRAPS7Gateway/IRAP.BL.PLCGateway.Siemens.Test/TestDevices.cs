using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace IRAP.BL.PLCGateway.Siemens.Test
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

        protected int GetIntValue(byte[] buffer, int pos)
        {
            int value =
                (int)((buffer[pos] & 0xff) << 8 | ((buffer[pos + 1] & 0xff)));
            return value;
        }

        protected Int32 GetDWordValue(byte[] buffer, int pos)
        {
            byte[] value = new byte[4];
            Array.Copy(buffer, pos, value, 0, 4);

            return BitConverter.ToInt32(value, 0);
        }

        protected float GetRealValue(byte[] buffer, int pos)
        {
            byte[] value = new byte[4];
            Array.Copy(buffer, pos, value, 0, 4);

            return BitConverter.ToSingle(value, 0);
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

        protected void DealIntOfTag(byte[] buffer, IntOfTag tag, EventHandler postProcessing)
        {
            if (tag.Used)
            {
                int value = GetIntValue(buffer, (int)tag.DB_Offset);
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

        protected void DealRealOfTag(byte[] buffer, RealOfTag tag, EventHandler postProcessing)
        {
            if (tag.Used)
            {
                float value = GetRealValue(buffer, (int)tag.DB_Offset);
                if (tag.Value != value)
                {
                    tag.Value = value;
                    Console.WriteLine(
                        $"{DateTime.Now.ToString("HH:mm:ss.fff")}:" +
                        $"DB{DBNumber}.DBD{tag.DB_Offset}={tag.Value}");

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
        protected void WriteToPLC(TCustomTKDevice device, CustomTagTest tag)
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

            #region 设置 WIPStation Tag 组
            wipStation = new WIPStation()
            {
                Prefix = "01",
            };
            wipStation.WIP_Station_LeafID.DB_Offset = 2;
            wipStation.WIP_Code.DB_Offset = 6;
            wipStation.WIP_ID_Type_Code.DB_Offset = 26;
            wipStation.WIP_ID_Code.DB_Offset = 28;

            wipStation.WIP_Move_In.SetPosition(108, 0);
            wipStation.Request_For_Poka_Yoke.SetPosition(108, 1);
            wipStation.Is_NG_WIP_Onto_Line_Station.SetPosition(108, 2);
            wipStation.Error_proofing_detection.SetPosition(108, 3);

            wipStation.Poka_Yoke_Result.DB_Offset = 109;
            wipStation.Operation_Conclusion.DB_Offset = 110;
            #endregion
        }

        //public override uint DBNumber => 1901;
        public override uint DBNumber => 30;

        public override int BufferSize => 111;

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

        //public override uint DBNumber => 5900;
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

    /// <summary>
    /// 华域麦格纳组装线 M0100-安装水冷管设备
    /// </summary>
    public class M0100 : TCustomTKDevice
    {
        private TS7GroupWIPOntoLine wipOntoLine = new TS7GroupWIPOntoLine();
        private TS7GroupFEEDING feeding = new TS7GroupFEEDING();
        private TS7GroupUNFEEDING unfeeding = new TS7GroupUNFEEDING();
        private TS7GroupIDBinding idbinding = new TS7GroupIDBinding();
        private TS7GroupPropertyM0100 property = new TS7GroupPropertyM0100();

        public M0100()
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
            wipStation.WIP_Station_LeafID.DB_Offset = 100;
            wipStation.Product_Number.DB_Offset = 104;
            wipStation.WIP_Code.DB_Offset = 144;
            wipStation.WIP_ID_Type_Code.DB_Offset = 164;
            wipStation.WIP_ID_Code.DB_Offset = 166;

            wipStation.WIP_Move_In.SetPosition(246, 0);
            wipStation.Production_Start.SetPosition(246, 1);
            wipStation.Production_End.SetPosition(246, 2);
            wipStation.WIP_Move_Out.SetPosition(246, 3);
            wipStation.Change_Over_Start.SetPosition(246, 4);
            wipStation.Change_Over_End.SetPosition(246, 5);
            wipStation.Production_Pause.SetPosition(246, 6);
            wipStation.Production_Restart.SetPosition(246, 7);
            wipStation.Request_For_Poka_Yoke.SetPosition(247, 0);
            wipStation.Move_To_MRB_Operation.SetPosition(247, 1);
            wipStation.Request_For_Routing.SetPosition(247, 2);
            wipStation.Request_For_ID_Binding.SetPosition(247, 3);
            wipStation.Trigger_Equipment_Fail_Andon.SetPosition(247, 4);
            wipStation.Trigger_Quality_Problem_Andon.SetPosition(247, 5);
            wipStation.Stagnation_Warnning.SetPosition(247, 6);
            wipStation.Serial_Number_Request.SetPosition(247, 7);
            wipStation.Label_ELements_Request.SetPosition(248, 0);
            wipStation.Label_Print_Request.SetPosition(248, 1);
            wipStation.Label_Reprint_Request.SetPosition(248, 2);
            wipStation.Packing_Succeeded.SetPosition(248, 3);
            wipStation.Is_OK_WIP_Onto_Line_Station.SetPosition(248, 4);
            wipStation.Is_NG_WIP_Onto_Line_Station.SetPosition(248, 5);
            wipStation.Scheduled_Production_Down.SetPosition(248, 6);
            wipStation.Error_proofing_detection.SetPosition(248, 7);

            wipStation.Poka_Yoke_Result.DB_Offset = 250;
            wipStation.Operation_Conclusion.DB_Offset = 251;
            #endregion

            #region WIPOntoLine
            wipOntoLine.WIP_Src_Code.DB_Offset = 300;
            wipOntoLine.WIP_ID_Code.DB_Offset = 320;
            wipOntoLine.Container_Number_pallet_code.DB_Offset = 400;
            wipOntoLine.Number_of_Sub_WIPs.DB_Offset = 408;
            wipOntoLine.WIP_Code_01.DB_Offset = 410;
            wipOntoLine.WIP_ID_Type_Code_01.DB_Offset = 430;
            wipOntoLine.WIP_ID_Code_01.DB_Offset = 432;
            wipOntoLine.PWO_Number_01.DB_Offset = 512;
            wipOntoLine.Product_number_01.DB_Offset = 530;
            wipOntoLine.Sub_Container_Number_01.DB_Offset = 570;
            wipOntoLine.WIP_Quantity_01.DB_Offset = 578;
            #endregion

            #region FEEDING
            feeding.Material_Track_ID.DB_Offset = 600;
            feeding.Slot_Number.DB_Offset = 680;
            feeding.Request_For_Poka_Yoke.DB_Offset = 686;
            feeding.Poka_Yoke_Result.DB_Offset = 688;
            #endregion

            #region UNFEEDING
            unfeeding.Material_Track_ID.DB_Offset = 700;
            unfeeding.Slot_Number.DB_Offset = 780;
            unfeeding.Unfeeding_Quantity.DB_Offset = 786;
            unfeeding.Unfeeding_End.DB_Offset = 790;
            #endregion

            #region IDBinding
            idbinding.Primary_WIP_Code.DB_Offset = 800;
            idbinding.Product_Number.DB_Offset = 820;
            idbinding.ID_Part_SN_Scanner_Code_01.DB_Offset = 860;
            idbinding.Part_Number_Feedback_01.DB_Offset = 940;
            idbinding.Sequence_Number_01.DB_Offset = 942;
            #endregion

            #region PROPERTY
            property.Tighten1_Result.DB_Offset = 1000;
            property.Tighten1_PGNO.DB_Offset = 1002;
            property.Tighten1_MinTorque.DB_Offset = 1004;
            property.Tighten1_Torque.DB_Offset = 1008;
            property.Tighten1_MaxTorque.DB_Offset = 1012;
            property.Tighten1_MinAngle.DB_Offset = 1016;
            property.Tighten1_Angle.DB_Offset = 1020;
            property.Tighten1_MaxAngle.DB_Offset = 1024;
            property.Tighten2_Result.DB_Offset = 1028;
            property.Tighten2_PGNO.DB_Offset = 1030;
            property.Tighten2_MinTorque.DB_Offset = 1032;
            property.Tighten2_Torque.DB_Offset = 1036;
            property.Tighten2_MaxTorque.DB_Offset = 1040;
            property.Tighten2_MinAngle.DB_Offset = 1044;
            property.Tighten2_Angle.DB_Offset = 1048;
            property.Tighten2_MaxAngle.DB_Offset = 1052;
            property.Tighten3_Result.DB_Offset = 1056;
            property.Tighten3_PGNO.DB_Offset = 1058;
            property.Tighten3_MinTorque.DB_Offset = 1060;
            property.Tighten3_Torque.DB_Offset = 1064;
            property.Tighten3_MaxTorque.DB_Offset = 1068;
            property.Tighten3_MinAngle.DB_Offset = 1072;
            property.Tighten3_Angle.DB_Offset = 1076;
            property.Tighten3_MaxAngle.DB_Offset = 1080;
            property.StationRecord_MachineCycle.DB_Offset = 1084;
            property.StationRecord_WorkerID.DB_Offset = 1098;
            property.StationRecord_TrayNum.DB_Offset = 1108;
            property.StationRecord_Time1.DB_Offset = 1118;
            property.StationRecord_Time2.DB_Offset = 1130;
            property.StationRecord_Time3.DB_Offset = 1142;
            property.StationRecord_Time4.DB_Offset = 1154;
            property.StationRecord_Time5.DB_Offset = 1166;
            property.StationRecord_Time6.DB_Offset = 1178;
            property.StationRecord_Time7.DB_Offset = 1190;
            property.StationRecord_Time8.DB_Offset = 1202;
            property.StationRecord_Time9.DB_Offset = 1214;
            property.StationRecord_Time10.DB_Offset = 1226;
            property.StationRecord_Time11.DB_Offset = 1238;
            property.StationRecord_Time12.DB_Offset = 1250;
            property.StationRecord_Time13.DB_Offset = 1262;
            property.StationRecord_Time14.DB_Offset = 1274;
            property.StationRecord_Time15.DB_Offset = 1286;
            property.StationRecord_Check1.DB_Offset = 1298;
            property.StationRecord_Check2.DB_Offset = 1300;
            property.StationRecord_Check3.DB_Offset = 1302;
            property.StationRecord_Check4.DB_Offset = 1304;
            property.StationRecord_Check5.DB_Offset = 1306;
            property.StationRecord_Check6.DB_Offset = 1308;
            property.StationRecord_Check7.DB_Offset = 1310;
            property.StationRecord_Check8.DB_Offset = 1312;
            property.StationRecord_Check9.DB_Offset = 1314;
            property.StationRecord_Check10.DB_Offset = 1316;
            #endregion
        }

        //public override uint DBNumber => 1012;
        public override uint DBNumber => 31;

        public override int BufferSize => 1318;

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

            #region WIPOntoLine
            DealArrayCharOfTag(buffer, wipOntoLine.WIP_Src_Code, null);
            DealArrayCharOfTag(buffer, wipOntoLine.WIP_ID_Code, null);
            DealArrayCharOfTag(buffer, wipOntoLine.Container_Number_pallet_code, null);
            DealByteOfTag(buffer, wipOntoLine.Number_of_Sub_WIPs, null);
            DealArrayCharOfTag(buffer, wipOntoLine.WIP_Code_01, null);
            DealArrayCharOfTag(buffer, wipOntoLine.WIP_ID_Type_Code_01, null);
            DealArrayCharOfTag(buffer, wipOntoLine.WIP_ID_Code_01, null);
            DealArrayCharOfTag(buffer, wipOntoLine.PWO_Number_01, null);
            DealArrayCharOfTag(buffer, wipOntoLine.Product_number_01, null);
            DealArrayCharOfTag(buffer, wipOntoLine.Sub_Container_Number_01, null);
            DealDWordOfTag(buffer, wipOntoLine.WIP_Quantity_01, null);
            #endregion

            #region FEEDING
            DealArrayCharOfTag(buffer, feeding.Material_Track_ID, null);
            DealArrayCharOfTag(buffer, feeding.Slot_Number, null);
            DealByteOfTag(buffer, feeding.Request_For_Poka_Yoke, null);
            DealDWordOfTag(buffer, feeding.Poka_Yoke_Result, null);
            #endregion

            #region UNFEEDING
            DealArrayCharOfTag(buffer, unfeeding.Material_Track_ID, null);
            DealArrayCharOfTag(buffer, unfeeding.Slot_Number, null);
            DealDWordOfTag(buffer, unfeeding.Unfeeding_Quantity, null);
            DealByteOfTag(buffer, unfeeding.Unfeeding_End, null);
            #endregion

            #region IDBinding
            DealArrayCharOfTag(buffer, idbinding.Primary_WIP_Code, null);
            DealArrayCharOfTag(buffer, idbinding.Product_Number, null);
            DealArrayCharOfTag(buffer, idbinding.ID_Part_SN_Scanner_Code_01, null);
            DealIntOfTag(buffer, idbinding.Part_Number_Feedback_01, null);
            DealIntOfTag(buffer, idbinding.Sequence_Number_01, null);
            #endregion

            #region PROPERTY
            DealIntOfTag(buffer, property.Tighten1_Result, null);
            DealIntOfTag(buffer, property.Tighten1_PGNO, null);
            DealRealOfTag(buffer, property.Tighten1_MinTorque, null);
            DealRealOfTag(buffer, property.Tighten1_Torque, null);
            DealRealOfTag(buffer, property.Tighten1_MaxTorque, null);
            DealRealOfTag(buffer, property.Tighten1_MinAngle, null);
            DealRealOfTag(buffer, property.Tighten1_MinAngle, null);
            DealRealOfTag(buffer, property.Tighten1_Angle, null);
            DealRealOfTag(buffer, property.Tighten1_MaxAngle, null);
            DealIntOfTag(buffer, property.Tighten2_Result, null);
            DealIntOfTag(buffer, property.Tighten2_PGNO, null);
            DealRealOfTag(buffer, property.Tighten2_MinTorque, null);
            DealRealOfTag(buffer, property.Tighten2_Torque, null);
            DealRealOfTag(buffer, property.Tighten2_MaxTorque, null);
            DealRealOfTag(buffer, property.Tighten2_MinAngle, null);
            DealRealOfTag(buffer, property.Tighten2_MinAngle, null);
            DealRealOfTag(buffer, property.Tighten2_Angle, null);
            DealRealOfTag(buffer, property.Tighten2_MaxAngle, null);
            DealIntOfTag(buffer, property.Tighten3_Result, null);
            DealIntOfTag(buffer, property.Tighten3_PGNO, null);
            DealRealOfTag(buffer, property.Tighten3_MinTorque, null);
            DealRealOfTag(buffer, property.Tighten3_Torque, null);
            DealRealOfTag(buffer, property.Tighten3_MaxTorque, null);
            DealRealOfTag(buffer, property.Tighten3_MinAngle, null);
            DealRealOfTag(buffer, property.Tighten3_MinAngle, null);
            DealRealOfTag(buffer, property.Tighten3_Angle, null);
            DealRealOfTag(buffer, property.Tighten3_MaxAngle, null);
            DealIntOfTag(buffer, property.StationRecord_MachineCycle, null);
            DealArrayCharOfTag(buffer, property.StationRecord_WorkerID, null);
            DealArrayCharOfTag(buffer, property.StationRecord_TrayNum, null);
            DealArrayCharOfTag(buffer, property.StationRecord_Time1, null);
            DealArrayCharOfTag(buffer, property.StationRecord_Time2, null);
            DealArrayCharOfTag(buffer, property.StationRecord_Time3, null);
            DealArrayCharOfTag(buffer, property.StationRecord_Time4, null);
            DealArrayCharOfTag(buffer, property.StationRecord_Time5, null);
            DealArrayCharOfTag(buffer, property.StationRecord_Time6, null);
            DealArrayCharOfTag(buffer, property.StationRecord_Time7, null);
            DealArrayCharOfTag(buffer, property.StationRecord_Time8, null);
            DealArrayCharOfTag(buffer, property.StationRecord_Time9, null);
            DealArrayCharOfTag(buffer, property.StationRecord_Time10, null);
            DealArrayCharOfTag(buffer, property.StationRecord_Time11, null);
            DealArrayCharOfTag(buffer, property.StationRecord_Time12, null);
            DealArrayCharOfTag(buffer, property.StationRecord_Time13, null);
            DealArrayCharOfTag(buffer, property.StationRecord_Time14, null);
            DealArrayCharOfTag(buffer, property.StationRecord_Time15, null);
            DealIntOfTag(buffer, property.StationRecord_Check1, null);
            DealIntOfTag(buffer, property.StationRecord_Check2, null);
            DealIntOfTag(buffer, property.StationRecord_Check3, null);
            DealIntOfTag(buffer, property.StationRecord_Check4, null);
            DealIntOfTag(buffer, property.StationRecord_Check5, null);
            DealIntOfTag(buffer, property.StationRecord_Check6, null);
            DealIntOfTag(buffer, property.StationRecord_Check7, null);
            DealIntOfTag(buffer, property.StationRecord_Check8, null);
            DealIntOfTag(buffer, property.StationRecord_Check9, null);
            DealIntOfTag(buffer, property.StationRecord_Check10, null);
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