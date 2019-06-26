using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRAP.BL.PLCGateway.Siemens.Test
{
    public class CustomDeviceTest
    {
        /// <summary>
        /// 根据关键字建立的 TagGroup 字典
        /// </summary>
        private Dictionary<string, TS7CustomGroup> tagGruops =
                    new Dictionary<string, TS7CustomGroup>();

        /// <param name="parent">所属 PLC</param>
        public CustomDeviceTest(SiemensPLC_Test parent)
        {
            if (parent == null)
            {
                throw new Exception("没有隶属的 PLC ，不能创建设备");
            }
        }

        public TS7GroupCOMM COMM { get; private set; } = new TS7GroupCOMM();

        public TS7GroupWIPStations WIPStations { get; private set; } = new TS7GroupWIPStations();

        public TS7GroupProperty Property { get; private set; } = new TS7GroupProperty();

        public TS7GroupStagnation Stagnation { get; private set; } = new TS7GroupStagnation();

        public TS7GroupEquipmentFail EquipmentFail { get; private set; } = new TS7GroupEquipmentFail();

        public TS7GroupSafetyProblem SafetyProblem { get; private set; } = new TS7GroupSafetyProblem();

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 设备数据块区
        /// </summary>
        public SiemensRegisterType DBType { get; set; } = SiemensRegisterType.DB;

        /// <summary>
        /// 数据块区编号
        /// </summary>
        public int DBNumber { get; set; } = 0;

        /// <summary>
        /// 由该 PLC 进行控制
        /// </summary>
        public SiemensPLC_Test Parent { get; private set; } = null;
    }
}