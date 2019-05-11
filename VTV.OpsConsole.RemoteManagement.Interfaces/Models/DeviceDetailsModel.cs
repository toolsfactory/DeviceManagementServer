using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace VTV.OpsConsole.RemoteManagement.Models
{
    public class DeviceDetailsModel
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public Dictionary<string,string> Attributes { get; }
        public long Version { get; set; }
        public string TypeName { get; set; }
        public string CommandsUrl { get; set; }

        public DeviceDetailsModel()
        {
            Attributes = new Dictionary<string, string>();
        }
    }
}
