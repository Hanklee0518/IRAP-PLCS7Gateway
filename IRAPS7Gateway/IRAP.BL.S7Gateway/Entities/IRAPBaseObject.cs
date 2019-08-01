using Logrila.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRAP.BL.S7Gateway
{
    /// <summary>
    /// IRAP基础对象父类
    /// </summary>
    public class IRAPBaseObject
    {
        /// <summary>
        /// 日志记录对象
        /// </summary>
        protected ILog _log = null;

        ///// <summary>
        ///// 保存到 MongoDB 的日志记录对象
        ///// </summary>
        //protected LogUtil logUtil = null;

        /// <summary>
        /// 对象的唯一内部标识码
        /// </summary>
        protected Guid id = Guid.NewGuid();

        /// <summary>
        /// 构造方法
        /// </summary>
        public IRAPBaseObject()
        {
            //logUtil = LogUtil.GetInstance("DeviceLog");
        }
    }
}