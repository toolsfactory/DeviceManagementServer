using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTV.OpsConsole.RemoteManagement.Models
{
    public class DevicesListModel
    {
        public DevicesListModel()
        {
            Devices = new List<DevicesListEntryModel>(10);
        }
        public List<DevicesListEntryModel> Devices { get; }
    }

    public class DevicesListEntryModel
    {
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public string DeviceUrl { get; set; }
    }
}
