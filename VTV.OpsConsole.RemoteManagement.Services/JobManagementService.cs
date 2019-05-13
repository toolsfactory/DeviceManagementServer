using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon.IoT;
using Amazon.IoT.Model;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using VTV.OpsConsole.RemoteManagement.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace VTV.OpsConsole.RemoteManagement.Services
{
    public class JobManagementService : IJobManagementService
    {
        private readonly IAWSClientsService _awsClientsService;
        private readonly IConfiguration _config;

        public JobManagementService(IAWSClientsService awsClientsService, IConfiguration config)
        {
            _awsClientsService = awsClientsService;
            _config = config;
        }

        public async Task DeleteJobAsync(string jobId)
        {
            var request = new DeleteJobRequest { JobId = jobId, Force = true };
            var response = await _awsClientsService.IoTClient.DeleteJobAsync(request);
        }

        public async Task<JobDetailsModel> GetJobDetailsAsync(string jobId)
        {
            var describeresp = await _awsClientsService.IoTClient.DescribeJobAsync(new DescribeJobRequest { JobId = jobId });
            return new JobDetailsModel()
            {
                JobId = jobId,
                JobArn = describeresp.Job.JobArn,
                Comment = describeresp.Job.Comment,
                CompletedAt = describeresp.Job.CompletedAt,
                CreatedAt = describeresp.Job.CreatedAt,
                Description = describeresp.Job.Description,
                LastUpdatedAt = describeresp.Job.LastUpdatedAt,
                Status = describeresp.Job.Status.Value,
                Targets = describeresp.Job.Targets
            };
        }

        public async Task<JObject> GetJobDocumentAsync(string jobId)
        {
            var docresp = await _awsClientsService.IoTClient.GetJobDocumentAsync(new GetJobDocumentRequest { JobId = jobId });
            return JObject.Parse(docresp.Document);
        }
    }
}
