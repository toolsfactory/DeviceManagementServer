using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTV.OpsConsole.RemoteManagement.Models
{
    public class DeviceJobsModel
    {
        public DeviceJobsModel()
        {
            Jobs = new List<DeviceJobsEntryModel>(10);
        }
        public List<DeviceJobsEntryModel> Jobs { get; }
    }

    public class DeviceJobsEntryModel
    {
        public string JobId { get; set; }
        public DateTime QueuedAt { get; set; }
        public string Status { get; set; }
        public JObject Document { get; set; }
        public string JobUrl { get; set; }
    }
}
