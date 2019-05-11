using System;
using System.Collections.Generic;

namespace VTV.OpsConsole.RemoteManagement.Models
{
    public class DeviceAttributesModel
    {
        public Dictionary<string, DeviceAttributesEntryModel> Attributes { get; }

        public DeviceAttributesModel()
        {
            Attributes = new Dictionary<string, DeviceAttributesEntryModel>();
        }
    }

    public class DeviceAttributesEntryModel
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Value { get; set; }
        public DateTime LastChanged { get; set; }
    }
}
