using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon.IoT;
using Amazon.IoT.Model;
using VTV.OpsConsole.RemoteManagement.Interfaces;

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
            var request = new DeleteJobRequest { JobId = jobId };
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
            DescribeJobResponse awsresponse;
            JobDetailsResult result = new JobDetailsResult();
            var request = new DescribeJobRequest { JobId = jobId };
            try
            {
                awsresponse = await awsClientsService.IoTClient.DescribeJobAsync(request);
                result.JobId = jobId;
                result.JobArn = awsresponse.Job.JobArn;
                result.Comment = awsresponse.Job.Comment;
                result.CompletedAt = awsresponse.Job.CompletedAt;
                result.CreatedAt = awsresponse.Job.CreatedAt;
                result.Description = awsresponse.Job.Description;
                result.DocumentSource = awsresponse.DocumentSource;
                result.LastUpdatedAt = awsresponse.Job.LastUpdatedAt;
                result.Status = awsresponse.Job.Status.Value;
                result.StatusCode = System.Net.HttpStatusCode.OK;
                result.Targets = awsresponse.Job.Targets;
            }
            catch (AmazonIoTException ex)
            {
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.ErrorDescription = $"Internal Code: '{ex.StatusCode}' - {ex.Message}";
            }
            return result;
        }
    }
}
