using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRAP.BL.PLCGateway.Siemens.Test
{
    public class TS7CustomGroup
    {
    }

    public class TS7GroupCOMM : TS7CustomGroup
    {
        public TS7GroupCOMM()
        {
            Equipment_Running_Mode = new BoolOfTag()
            {
                TagName = "Equipment_Running_Mode",
            };
            Domination_Status = new BoolOfTag()
            {
                TagName = "Domination_Status",
            };
            PLC_Heart_Beat = new BoolOfTag()
            {
                TagName = "PLC_Heart_Beat",
            };
            Equipment_Power_On = new BoolOfTag()
            {
                TagName = "Equipment_Power_On",
            };
            Equipment_Fail = new BoolOfTag()
            {
                TagName = "Equipment_Fail",
            };
            Tool_Fail = new BoolOfTag()
            {
                TagName = "Tool_Fail",
            };
            Cycle_Started = new BoolOfTag()
            {
                TagName = "Cycle_Started",
            };
            Equipment_Starvation = new BoolOfTag()
            {
                TagName = "Equipment_Starvation",
            };
            MES_Heart_Beat = new BoolOfTag()
            {
                TagName = "MES_Heart_Beat",
            };
        }

        public BoolOfTag Equipment_Running_Mode { get; private set; }

        public BoolOfTag Domination_Status { get; private set; }

        public BoolOfTag PLC_Heart_Beat { get; private set; }

        public BoolOfTag Equipment_Power_On { get; private set; }

        public BoolOfTag Equipment_Fail { get; private set; }

        public BoolOfTag Tool_Fail { get; private set; }

        public BoolOfTag Cycle_Started { get; private set; }

        public BoolOfTag Equipment_Starvation { get; private set; }

        public BoolOfTag MES_Heart_Beat { get; private set; }
    }

    public class TS7GroupWIPStations : TS7CustomGroup
    {

    }

    public class WIPStation
    {
        public WIPStation()
        {
            WIP_Station_LeafID = new DWordOfTag();
            Product_Number = new ArrayCharOfTag(40);
            WIP_Code = new ArrayCharOfTag(20);
            WIP_ID_Type_Code = new ArrayCharOfTag(2);
            WIP_ID_Code = new ArrayCharOfTag(80);
            WIP_Move_In = new BoolOfTag();
            Production_Start = new BoolOfTag();
            Production_End = new BoolOfTag();
            WIP_Move_Out = new BoolOfTag();
            Change_Over_Start = new BoolOfTag();
            Change_Over_End = new BoolOfTag();
            Production_Pause = new BoolOfTag();
            Production_Restart = new BoolOfTag();
            Request_For_Poka_Yoke = new BoolOfTag();
            Move_To_MRB_Operation = new BoolOfTag();
            Request_For_Routing = new BoolOfTag();
            Request_For_ID_Binding = new BoolOfTag();
            Trigger_Equipment_Fail_Andon = new BoolOfTag();
            Trigger_Quality_Problem_Andon = new BoolOfTag();
            Stagnation_Warnning = new BoolOfTag();
            Serial_Number_Request = new BoolOfTag();
            Label_ELements_Request = new BoolOfTag();
            Label_Print_Request = new BoolOfTag();
            Label_Reprint_Request = new BoolOfTag();
            Packing_Succeeded = new BoolOfTag();
            Is_OK_WIP_Onto_Line_Station = new BoolOfTag();
            Is_NG_WIP_Onto_Line_Station = new BoolOfTag();
            Scheduled_Production_Down = new BoolOfTag();
            Error_proofing_detection = new BoolOfTag();
            Reserved_Signal_28 = new BoolOfTag();
            Reserved_Signal_29 = new BoolOfTag();
            Reserved_Signal_30 = new BoolOfTag();
            Reserved_Signal_31 = new BoolOfTag();
            Reserved_Signal_32 = new BoolOfTag();
            Poka_Yoke_Result = new ByteOfTag();
            Operation_Conclusion = new ByteOfTag();
        }

        public DWordOfTag WIP_Station_LeafID { get; private set; }

        public ArrayCharOfTag Product_Number { get; private set; }

        public ArrayCharOfTag WIP_Code { get; private set; }

        public ArrayCharOfTag WIP_ID_Type_Code { get; private set; }

        public ArrayCharOfTag WIP_ID_Code { get; private set; }

        public BoolOfTag WIP_Move_In { get; private set; }

        public BoolOfTag Production_Start { get; private set; }

        public BoolOfTag Production_End { get; private set; }

        public BoolOfTag WIP_Move_Out { get; private set; }

        public BoolOfTag Change_Over_Start { get; private set; }

        public BoolOfTag Change_Over_End { get; private set; }

        public BoolOfTag Production_Pause { get; private set; }

        public BoolOfTag Production_Restart { get; private set; }

        public BoolOfTag Request_For_Poka_Yoke { get; private set; }

        public BoolOfTag Move_To_MRB_Operation { get; private set; }

        public BoolOfTag Request_For_Routing { get; private set; }

        public BoolOfTag Request_For_ID_Binding { get; private set; }

        public BoolOfTag Trigger_Equipment_Fail_Andon { get; private set; }

        public BoolOfTag Trigger_Quality_Problem_Andon { get; private set; }

        public BoolOfTag Stagnation_Warnning { get; private set; }

        public BoolOfTag Serial_Number_Request { get; private set; }

        public BoolOfTag Label_ELements_Request { get; private set; }

        public BoolOfTag Label_Print_Request { get; private set; }

        public BoolOfTag Label_Reprint_Request { get; private set; }

        public BoolOfTag Packing_Succeeded { get; private set; }

        public BoolOfTag Is_OK_WIP_Onto_Line_Station { get; private set; }

        public BoolOfTag Is_NG_WIP_Onto_Line_Station { get; private set; }

        public BoolOfTag Scheduled_Production_Down { get; private set; }

        public BoolOfTag Error_proofing_detection { get; private set; }

        public BoolOfTag Reserved_Signal_28 { get; private set; }

        public BoolOfTag Reserved_Signal_29 { get; private set; }

        public BoolOfTag Reserved_Signal_30 { get; private set; }

        public BoolOfTag Reserved_Signal_31 { get; private set; }

        public BoolOfTag Reserved_Signal_32 { get; private set; }

        public ByteOfTag Poka_Yoke_Result { get; private set; }

        public ByteOfTag Operation_Conclusion { get; private set; }

        /// <summary>
        /// 前缀
        /// </summary>
        public string Prefix { get; set; }
    }

    public class TS7GroupProperty : TS7CustomGroup
    {
        public TS7GroupProperty()
        {
            Equipment_End_Definition = new BoolOfTag();
        }

        public BoolOfTag Equipment_End_Definition { get; private set; }
    }

    public class TS7GroupStagnation : TS7CustomGroup
    {
        public DWordOfTag Time_In_Seconds { get; private set; } = new DWordOfTag();

        public WordOfTag Threshold { get; private set; } = new WordOfTag();
    }

    public class TS7GroupEquipmentFail : TS7CustomGroup
    {
        public DWordOfTag Equipment_Failures_Group_1 { get; private set; } = new DWordOfTag();

        public DWordOfTag Equipment_Failures_Group_2 { get; private set; } = new DWordOfTag();

        public DWordOfTag Equipment_Failures_Group_3 { get; private set; } = new DWordOfTag();

        public DWordOfTag Equipment_Failures_Group_4 { get; private set; } = new DWordOfTag();

        public DWordOfTag Equipment_Failures_Group_5 { get; private set; } = new DWordOfTag();

        public DWordOfTag Equipment_Failures_Group_6 { get; private set; } = new DWordOfTag();

        public DWordOfTag Equipment_Failures_Group_7 { get; private set; } = new DWordOfTag();

        public DWordOfTag Equipment_Failures_Group_8 { get; private set; } = new DWordOfTag();

        public ArrayCharOfTag Failure_Code { get; private set; } = new ArrayCharOfTag(10);
    }

    public class TS7GroupSafetyProblem : TS7CustomGroup
    {
        public ByteOfTag Safety_Issue_Type { get; private set; } = new ByteOfTag();
    }

    public class TS7GroupWIPOntoLine : TS7CustomGroup
    {
        public ArrayCharOfTag WIP_Src_Code { get; private set; } = new ArrayCharOfTag(20);
        public ArrayCharOfTag WIP_ID_Code { get; private set; } = new ArrayCharOfTag(80);
        public ArrayCharOfTag Container_Number_pallet_code { get; private set; } = new ArrayCharOfTag(8);
        public ByteOfTag Number_of_Sub_WIPs { get; private set; } = new ByteOfTag();
        public ArrayCharOfTag WIP_Code_01 { get; private set; } = new ArrayCharOfTag(20);
        public ArrayCharOfTag WIP_ID_Type_Code_01 { get; private set; } = new ArrayCharOfTag(2);
        public ArrayCharOfTag WIP_ID_Code_01 { get; private set; } = new ArrayCharOfTag(80);
        public ArrayCharOfTag PWO_Number_01 { get; private set; } = new ArrayCharOfTag(18);
        public ArrayCharOfTag Product_number_01 { get; private set; } = new ArrayCharOfTag(40);
        public ArrayCharOfTag Sub_Container_Number_01 { get; private set; } = new ArrayCharOfTag(8);
        public DWordOfTag WIP_Quantity_01 { get; private set; } = new DWordOfTag();
    }

    public class TS7GroupFEEDING : TS7CustomGroup
    {
        public ArrayCharOfTag Material_Track_ID { get; private set; } = new ArrayCharOfTag(80);
        public ArrayCharOfTag Slot_Number { get; private set; } = new ArrayCharOfTag(6);
        public ByteOfTag Request_For_Poka_Yoke { get; private set; } = new ByteOfTag();
        public DWordOfTag Poka_Yoke_Result { get; private set; } = new DWordOfTag();
    }

    public class TS7GroupUNFEEDING : TS7CustomGroup
    {
        public ArrayCharOfTag Material_Track_ID { get; private set; } = new ArrayCharOfTag(80);
        public ArrayCharOfTag Slot_Number { get; private set; } = new ArrayCharOfTag(6);
        public DWordOfTag Unfeeding_Quantity { get; private set; } = new DWordOfTag();
        public ByteOfTag Unfeeding_End { get; private set; } = new ByteOfTag();
    }

    public class TS7GroupIDBinding : TS7CustomGroup
    {
        public ArrayCharOfTag Primary_WIP_Code { get; private set; } = new ArrayCharOfTag(20);
        public ArrayCharOfTag Product_Number { get; private set; } = new ArrayCharOfTag(40);
        public ArrayCharOfTag ID_Part_SN_Scanner_Code_01 { get; private set; } = new ArrayCharOfTag(80);
        public IntOfTag Part_Number_Feedback_01 { get; private set; } = new IntOfTag();
        public IntOfTag Sequence_Number_01 { get; private set; } = new IntOfTag();
    }

    public class TS7GroupPropertyM0100 : TS7CustomGroup
    {
        public IntOfTag Tighten1_Result { get; private set; } = new IntOfTag();
        public IntOfTag Tighten1_PGNO { get; private set; } = new IntOfTag();
        public RealOfTag Tighten1_MinTorque { get; private set; } = new RealOfTag();
        public RealOfTag Tighten1_Torque { get; private set; } = new RealOfTag();
        public RealOfTag Tighten1_MaxTorque { get; private set; } = new RealOfTag();
        public RealOfTag Tighten1_MinAngle { get; private set; } = new RealOfTag();
        public RealOfTag Tighten1_Angle { get; private set; } = new RealOfTag();
        public RealOfTag Tighten1_MaxAngle { get; private set; } = new RealOfTag();
        public IntOfTag Tighten2_Result { get; private set; } = new IntOfTag();
        public IntOfTag Tighten2_PGNO { get; private set; } = new IntOfTag();
        public RealOfTag Tighten2_MinTorque { get; private set; } = new RealOfTag();
        public RealOfTag Tighten2_Torque { get; private set; } = new RealOfTag();
        public RealOfTag Tighten2_MaxTorque { get; private set; } = new RealOfTag();
        public RealOfTag Tighten2_MinAngle { get; private set; } = new RealOfTag();
        public RealOfTag Tighten2_Angle { get; private set; } = new RealOfTag();
        public RealOfTag Tighten2_MaxAngle { get; private set; } = new RealOfTag();
        public IntOfTag Tighten3_Result { get; private set; } = new IntOfTag();
        public IntOfTag Tighten3_PGNO { get; private set; } = new IntOfTag();
        public RealOfTag Tighten3_MinTorque { get; private set; } = new RealOfTag();
        public RealOfTag Tighten3_Torque { get; private set; } = new RealOfTag();
        public RealOfTag Tighten3_MaxTorque { get; private set; } = new RealOfTag();
        public RealOfTag Tighten3_MinAngle { get; private set; } = new RealOfTag();
        public RealOfTag Tighten3_Angle { get; private set; } = new RealOfTag();
        public RealOfTag Tighten3_MaxAngle { get; private set; } = new RealOfTag();
        public IntOfTag StationRecord_MachineCycle { get; private set; } = new IntOfTag();
        public ArrayCharOfTag StationRecord_WorkerID { get; private set; } = new ArrayCharOfTag(10);
        public ArrayCharOfTag StationRecord_TrayNum { get; private set; } = new ArrayCharOfTag(10);
        public ArrayCharOfTag StationRecord_Time1 { get; private set; } = new ArrayCharOfTag(12);
        public ArrayCharOfTag StationRecord_Time2 { get; private set; } = new ArrayCharOfTag(12);
        public ArrayCharOfTag StationRecord_Time3 { get; private set; } = new ArrayCharOfTag(12);
        public ArrayCharOfTag StationRecord_Time4 { get; private set; } = new ArrayCharOfTag(12);
        public ArrayCharOfTag StationRecord_Time5 { get; private set; } = new ArrayCharOfTag(12);
        public ArrayCharOfTag StationRecord_Time6 { get; private set; } = new ArrayCharOfTag(12);
        public ArrayCharOfTag StationRecord_Time7 { get; private set; } = new ArrayCharOfTag(12);
        public ArrayCharOfTag StationRecord_Time8 { get; private set; } = new ArrayCharOfTag(12);
        public ArrayCharOfTag StationRecord_Time9 { get; private set; } = new ArrayCharOfTag(12);
        public ArrayCharOfTag StationRecord_Time10 { get; private set; } = new ArrayCharOfTag(12);
        public ArrayCharOfTag StationRecord_Time11 { get; private set; } = new ArrayCharOfTag(12);
        public ArrayCharOfTag StationRecord_Time12 { get; private set; } = new ArrayCharOfTag(12);
        public ArrayCharOfTag StationRecord_Time13 { get; private set; } = new ArrayCharOfTag(12);
        public ArrayCharOfTag StationRecord_Time14 { get; private set; } = new ArrayCharOfTag(12);
        public ArrayCharOfTag StationRecord_Time15 { get; private set; } = new ArrayCharOfTag(12);
        public IntOfTag StationRecord_Check1 { get; private set; } = new IntOfTag();
        public IntOfTag StationRecord_Check2 { get; private set; } = new IntOfTag();
        public IntOfTag StationRecord_Check3 { get; private set; } = new IntOfTag();
        public IntOfTag StationRecord_Check4 { get; private set; } = new IntOfTag();
        public IntOfTag StationRecord_Check5 { get; private set; } = new IntOfTag();
        public IntOfTag StationRecord_Check6 { get; private set; } = new IntOfTag();
        public IntOfTag StationRecord_Check7 { get; private set; } = new IntOfTag();
        public IntOfTag StationRecord_Check8 { get; private set; } = new IntOfTag();
        public IntOfTag StationRecord_Check9 { get; private set; } = new IntOfTag();
        public IntOfTag StationRecord_Check10 { get; private set; } = new IntOfTag();
    }
}