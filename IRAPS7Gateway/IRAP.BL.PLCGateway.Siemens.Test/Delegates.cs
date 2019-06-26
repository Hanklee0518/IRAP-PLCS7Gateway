using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRAP.BL.PLCGateway.Siemens.Test
{
    public delegate void DBDataChangedHandler(byte[] buffer);

    public delegate void S7WriteBackHandler(TCustomTKDevice device, CustomTagTest tag);
}