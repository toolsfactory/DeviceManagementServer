using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagementServer.Models
{

    public class DeviceModel
    {
        public string Id { get; set; }
        public string Manufacturer { get; set; }
        public string ModelName { get; set; }
        public string ProductClass { get; set; }
        public Dictionary<string, string> Versions { get; set; }
        public Dictionary<string, string> AdditionalAttributes { get; set; }
        public List<string> Tags { get; set; }
    }
}
