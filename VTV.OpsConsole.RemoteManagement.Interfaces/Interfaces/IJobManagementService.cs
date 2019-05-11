using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VTV.OpsConsole.RemoteManagement.Interfaces
{
    public interface IJobManagementService
    {
        Task<JobDetailsResult> GetJobDetailsAsync(string jobId);
    }

    public class JobDetailsResult
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
        public string DocumentSource { get; set; }
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public string ErrorDescription { get; set; }
    }
}
