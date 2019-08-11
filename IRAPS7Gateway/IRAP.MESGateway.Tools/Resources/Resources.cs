using IRAP.MESGateway.Tools.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAP.MESGateway.Tools
{
    public class DataSourceChangedEventArgs : EventArgs
    {
        public DataSourceChangedEventArgs(object objectID)
        {
            if (objectID is Guid)
            {
                EntityID = (Guid)objectID;
            }
        }

        internal Guid EntityID { get; private set; }
    }

    public delegate void DataSourceChangedEventHandler(object sender, ref DataSourceChangedEventArgs e);
}
