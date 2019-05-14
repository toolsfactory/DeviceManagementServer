﻿using Newtonsoft.Json.Linq;

namespace VTV.OpsConsole.RemoteManagement.Models
{
    public class CommandSendModel
    {
        public string JobId { get; set; }
        public string JobUrl { get; set; }
        public JObject Document { get; set; }
    }

}
