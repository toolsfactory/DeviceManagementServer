using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon.IoT;
using Amazon.IoT.Model;

namespace DeviceManagementServer.Services
{
    public class CommandExecutionService : ICommandExecutionService
    {
        private readonly IAWSClientsService awsClientsService;
        private readonly IConfiguration config;

        public CommandExecutionService(IAWSClientsService awsClientsService, IConfiguration config)
        {
            this.awsClientsService = awsClientsService;
            this.config = config;
        }

        public async Task<CommandExecutionResult> CreateCommandAsync(string deviceId, Command cmd, string idprefix = "")
        {
            CreateJobResponse clientresponse;
            CommandExecutionResult response = new CommandExecutionResult();
            var request = new Amazon.IoT.Model.CreateJobRequest()
            {
                Document = CreateJobDocument(cmd),
                JobId = idprefix + Guid.NewGuid().ToString("N"),
                Targets = new List<string>(1) { "arn:aws:iot:" + config["AWS:Region"] +":" + config["AWS:AccountId"] +":thing/" + deviceId }, 
            };
            try
            {
                clientresponse = await awsClientsService.IoTClient.CreateJobAsync(request);
                response.StatusCode = clientresponse.HttpStatusCode;
                response.CommandId = clientresponse.JobId;
            }
            catch (ResourceNotFoundException ex)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.ErrorDescription = "Device not found";
            }
            catch (AmazonIoTException ex)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.ErrorDescription = $"Internal Code: '{ex.StatusCode}' - {ex.Message}";
            }
            return response;
        }

        public async Task<CommandsForDeviceResult> ListJobsForDeviceAsync(string deviceId)
        {
            return await InternalListJobsForDeviceAsync(deviceId);
        }

        public async Task<CommandsForDeviceResult> ListPendingJobsForDeviceAsync(string deviceId)
        {
            return await InternalListJobsForDeviceAsync(deviceId, JobExecutionStatus.QUEUED);
        }

        private string CreateJobDocument(Command cmd)
        {
            var ttlc = DateTime.UtcNow.AddSeconds(cmd.TTL).ToString("o");
            return $"{{\"operation\":\"RemoteManagement\", \"command\":\"{cmd.Name}\", \"ttlc\":\"{ttlc}\"}}";
        }

        private async Task<CommandsForDeviceResult> InternalListJobsForDeviceAsync(string deviceId, JobExecutionStatus status = null)
        {
            ListJobExecutionsForThingResponse clientresponse;
            CommandsForDeviceResult response = new CommandsForDeviceResult();

            var request = new ListJobExecutionsForThingRequest()
            {
                ThingName = deviceId,
                MaxResults = 32,
                Status = status
            };
            try
            {
                clientresponse = await awsClientsService.IoTClient.ListJobExecutionsForThingAsync(request);
                var data = new List<CommandStatus>(clientresponse.ExecutionSummaries.Count);
                foreach (var item in clientresponse.ExecutionSummaries)
                {
                    data.Add(new CommandStatus()
                    {
                        Id = item.JobId,
                        Status = "Queued",
                        StartedAt = item.JobExecutionSummary.StartedAt,
                        QueuedAt = item.JobExecutionSummary.QueuedAt,
                        LastUpdatedAt = item.JobExecutionSummary.LastUpdatedAt
                    });
                }
                response.Commands = data;
                response.StatusCode = System.Net.HttpStatusCode.OK;
            }
            catch (ResourceNotFoundException ex)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.ErrorDescription = "Device not found";
            }
            catch (AmazonIoTException ex)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.ErrorDescription = $"Internal Code: '{ex.StatusCode}' - {ex.Message}";
            }
            return response;
        }
    }
}
