using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRAP.BL.S7Gateway.Entities
{
    public class TS7GroupCOMM
    {
        public TS7GroupCOMM()
        {
            Equipment_Running_Mode = new BoolOfTag()
            {
                TagName="Equipment_Running_Mode",
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

    public class TS7GroupWIPStations
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

    public class TS7GroupProperty
    {
        public TS7GroupProperty()
        {
            Equipment_End_Definition = new BoolOfTag();
        }

        public BoolOfTag Equipment_End_Definition { get; private set; }
    }

    public class TS7GroupStagnation
    {
        public DWordOfTag Time_In_Seconds { get; private set; } = new DWordOfTag();

        public WordOfTag Threshold { get; private set; } = new WordOfTag();
    }

    public class TS7GroupEquipmentFail
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

    public class TS7GroupSafetyProblem
    {
        public ByteOfTag Safety_Issue_Type { get; private set; } = new ByteOfTag();
    }
}