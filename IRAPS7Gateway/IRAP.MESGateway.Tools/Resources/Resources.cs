using IRAP.MESGateway.Tools.Entities;
using IRAP.MESGateway.Tools.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAP.MESGateway.Tools
{
    public class DeviceTreeDataSourceChangedEventArgs : EventArgs
    {
        public DeviceTreeDataSourceChangedEventArgs(object objectID)
        {
            if (objectID is Guid)
            {
                EntityID = (Guid)objectID;
            }
        }

        internal Guid EntityID { get; private set; }
    }

    public delegate void DeviceTreeDataSourceChangedEventHandler(
        object sender, 
        ref DeviceTreeDataSourceChangedEventArgs e);

    public class ServiceTreeDataSourceChangedEventArgs : EventArgs
    {
        public ServiceTreeDataSourceChangedEventArgs(ServiceEntity service)
        {
            Service = service;
        }

        internal ServiceEntity Service { get; private set; }
    }

    public delegate void ServiceTreeDataSourceChangedEventHandler(
        object sender, 
        ServiceTreeDataSourceChangedEventArgs e);
}
