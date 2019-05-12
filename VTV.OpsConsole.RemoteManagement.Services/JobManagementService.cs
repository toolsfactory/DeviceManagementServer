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
        private readonly IAWSClientsService awsClientsService;
        private readonly IConfiguration config;

        public JobManagementService(IAWSClientsService awsClientsService, IConfiguration config)
        {
            this.awsClientsService = awsClientsService;
            this.config = config;
        }

        public async Task DeleteJobAsync(string jobId)
        {
            var request = new DeleteJobRequest { JobId = jobId, Force = true };
            try
            {
                var response = await awsClientsService.IoTClient.DeleteJobAsync(request);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<JobDetailsResult> GetJobDetailsAsync(string jobId)
        {
            JobDetailsResult result = new JobDetailsResult();
            try
            {
                var describeresp = await awsClientsService.IoTClient.DescribeJobAsync(new DescribeJobRequest { JobId = jobId });
                result.JobId = jobId;
                result.JobArn = describeresp.Job.JobArn;
                result.Comment = describeresp.Job.Comment;
                result.CompletedAt = describeresp.Job.CompletedAt;
                result.CreatedAt = describeresp.Job.CreatedAt;
                result.Description = describeresp.Job.Description;
                result.LastUpdatedAt = describeresp.Job.LastUpdatedAt;
                result.Status = describeresp.Job.Status.Value;
                result.StatusCode = System.Net.HttpStatusCode.OK;
                result.Targets = describeresp.Job.Targets;
            }
            catch (AmazonIoTException ex)
            {
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.ErrorDescription = $"Internal Code: '{ex.StatusCode}' - {ex.Message}";
            }
            return result;
        }

        public async Task<JObject> GetJobDocumentAsync(string jobId)
        {
            try
            {
                var docresp = await awsClientsService.IoTClient.GetJobDocumentAsync(new GetJobDocumentRequest { JobId = jobId });
                return JObject.Parse(docresp.Document);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
