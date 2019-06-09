using IRAP.BL.S7Gateway.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRAP.BL.S7Gateway
{
    public delegate void DBDataChangedHandler(byte[] buffer);

    public delegate void S7WriteBackHandler(TCustomTKDevice device, CustomTag tag);
}