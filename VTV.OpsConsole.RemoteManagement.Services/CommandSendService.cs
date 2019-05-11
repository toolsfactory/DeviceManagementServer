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

        public async Task<CommandSendResult> SendCommandToDeviceAsync(string deviceId, Command command, JObject parameters = null)
        {
            CreateJobResponse clientresponse;
            CommandSendResult response = new CommandSendResult();
            var request = new Amazon.IoT.Model.CreateJobRequest()
            {
                Document = CreateJobDocument(command),
                JobId = this.config["BaseSystem:CommandPrefix"] + Guid.NewGuid().ToString("N"),
                Targets = new List<string>(1) { "arn:aws:iot:" + config["AWS:Region"] +":" + config["AWS:AccountId"] +":thing/" + deviceId }
            };
            try
            {
                clientresponse = await awsClientsService.IoTClient.CreateJobAsync(request);
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
            var ttlc = DateTime.UtcNow.AddSeconds(cmd.TTL).ToString("o");
            return $"{{\"operation\":\"RemoteManagement\", \"command\":\"{cmd.Name}\", \"ttlc\":\"{ttlc}\"}}";
        }
    }
}
