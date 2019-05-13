using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon.IoT;
using AWSM = Amazon.IoT.Model;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using VTV.OpsConsole.RemoteManagement.Models;
using VTV.OpsConsole.RemoteManagement.Exceptions;
using Newtonsoft.Json.Linq;

namespace VTV.OpsConsole.RemoteManagement.Services
{
    public class CommandSendService : ICommandSendService
    {
        private readonly IAWSClientsService _awsClientsService;
        private readonly IConfiguration _config;

        public CommandSendService(IAWSClientsService awsClientsService, IConfiguration config)
        {
            _awsClientsService = awsClientsService;
            _config = config;
        }

        public async Task<CommandSendModel> SendCommandToDeviceAsync(string deviceId, Command command, JObject parameters)
        {
            var ttlc = new DateTimeOffset(DateTime.UtcNow.AddSeconds(command.TTL)).ToUnixTimeSeconds();
            parameters.Add("operation", "RemoteManagement");
            parameters.Add("command", command.Name);
            var response = await SendCommandAsync(deviceId, _config["BaseSystem:CommandPrefix"], parameters.ToString());
            return response;
        }

        public async Task<CommandSendModel> SendCustomCommandToDeviceAsync(string deviceId, JObject data)
        {
            data.Add("operation", "RemoteManagement");
            var response = await SendCommandAsync(deviceId, _config["BaseSystem:CustomJobPrefix"], data.ToString());
            return response;
        }

        private async Task<CommandSendModel> SendCommandAsync(string deviceId, string prefix, string document)
        {
            CommandSendModel response = new CommandSendModel();
            var request = new Amazon.IoT.Model.CreateJobRequest()
            {
                Document = document,
                JobId = prefix + Guid.NewGuid().ToString("N"),
                Targets = new List<string>(1) { "arn:aws:iot:" + _config["AWS:Region"] + ":" + _config["AWS:AccountId"] + ":thing/" + deviceId }
            };
            var clientresponse = await _awsClientsService.IoTClient.CreateJobAsync(request);
            response.JobId = clientresponse.JobId;
            response.JobUrl = _config["BaseSystem:ServerUrl"] + "/api/jobs/" + clientresponse.JobId;
            return response;
        }
    }
    /*
    public static class ServiceHelper
    {
        public async static Task<T> ExecuteAwsCallAsync<T>(Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (AWSM.ResourceNotFoundException ex) { throw new ResourceNotFoundException("IoT:Thing", request.Targets[0], ex); }
            catch (AWSM.ResourceAlreadyExistsException ex) { throw new ResourceAlreadyExistsException("IoT:Job", request.JobId, ex); }
            catch (AWSM.ThrottlingException ex) { throw new CloudServiceConstraintsException("AWS", "IoT:CreateJobAsync", "Throttling", ex); }
            catch (AWSM.LimitExceededException ex) { throw new CloudServiceConstraintsException("AWS", "IoT:CreateJobAsync", "Limit", ex); }
            catch (AmazonIoTException ex) { throw new CloudServiceGenericException("AWS", "IoT:CreateJobAsync", ex); }
        }
    }
    */
}
