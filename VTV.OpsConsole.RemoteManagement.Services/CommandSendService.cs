using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon.IoT;
using Amazon.IoT.Model;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using VTV.OpsConsole.RemoteManagement.Models;
using Newtonsoft.Json.Linq;

namespace VTV.OpsConsole.RemoteManagement.Services
{
    public class CommandSendService : ICommandSendService
    {
        private readonly IAWSClientsService awsClientsService;
        private readonly IConfiguration config;

        public CommandSendService(IAWSClientsService awsClientsService, IConfiguration config)
        {
            this.awsClientsService = awsClientsService;
            this.config = config;
        }

        public async Task<JobSendResult> SendCommandToDeviceAsync(string deviceId, Command command, JObject parameters = null)
        {
            var response = await SendJobAsync(deviceId, this.config["BaseSystem:CommandPrefix"], CreateJobDocument(command));
            return response;
        }

        public async Task<JobSendResult> SendCustomJobToDeviceAsync(string deviceId, JObject data)
        {
            data.Add("operation", "RemoteManagement");
            var response = await SendJobAsync(deviceId, this.config["BaseSystem:CustomJobPrefix"], data.ToString());
            return response;
        }

        private async Task<JobSendResult> SendJobAsync(string deviceId, string prefix, string document)
        {
            JobSendResult response = new JobSendResult();
            var request = new Amazon.IoT.Model.CreateJobRequest()
            {
                Document = document,
                JobId = prefix + Guid.NewGuid().ToString("N"),
                Targets = new List<string>(1) { "arn:aws:iot:" + config["AWS:Region"] + ":" + config["AWS:AccountId"] + ":thing/" + deviceId }
            };
            try
            {
                var clientresponse = await awsClientsService.IoTClient.CreateJobAsync(request);
                response.StatusCode = clientresponse.HttpStatusCode;
                response.JobId = clientresponse.JobId;
                response.JobUrl = config["BaseSystem:ServerUrl"] + "/api/jobs/" + clientresponse.JobId;

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

        private string CreateJobDocument(Command cmd)
        {
            var ttlc = new DateTimeOffset(DateTime.UtcNow.AddSeconds(cmd.TTL)).ToUnixTimeSeconds();
            return $"{{\"operation\":\"RemoteManagement\", \"command\":\"{cmd.Name}\", \"ttlc\":\"{ttlc}\"}}";
        }
    }
}
