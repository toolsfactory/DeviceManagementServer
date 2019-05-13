using System;
using System.Collections.Generic;

namespace VTV.OpsConsole.RemoteManagement.Models
{
    public class JobDetailsModel
    {
        public List<string> Targets { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string JobId { get; set; }
        public string JobArn { get; set; }
        public string Description { get; set; }
       public DateTime CreatedAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public string Comment { get; set; }
    }
}
