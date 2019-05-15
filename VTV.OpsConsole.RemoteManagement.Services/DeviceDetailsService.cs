using System;
using System.IO;
using System.Threading.Tasks;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using VTV.OpsConsole.RemoteManagement.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Amazon.IoT;
using System.Text.RegularExpressions;
using Amazon.IoT.Model;
namespace VTV.OpsConsole.RemoteManagement.Services
{
    public class DeviceDetailsService : IDeviceDetailsService
    {
        private readonly IConfiguration _config;
        private readonly IAWSClientsService _awsClientsService;

        public DeviceDetailsService(IAWSClientsService awsClientsService, IConfiguration configuration)
        {
            _awsClientsService = awsClientsService;
            _config = configuration;
        }

        public async Task<DeviceDetailsModel> GetDeviceDetailsAsync(string id)
        {
            var data = await _awsClientsService.IoTClient.DescribeThingAsync(new Amazon.IoT.Model.DescribeThingRequest() { ThingName = id });
            var result = new DeviceDetailsModel()
            {
                Name = data.ThingName,
                Id = data.ThingId,
                Version = data.Version,
                TypeName = data.ThingTypeName,
                CommandsUrl = _config["BaseSystem:ServerUrl"] + "/api/devices/" + id + "/commands"
            };
            foreach(var attr in data.Attributes)
            { result.Attributes.Add(attr.Key, attr.Value); }
            return result;
        }

        public async Task<JObject> GetDeviceShadowAsync(string id)
        {
            var data = await _awsClientsService.IoTDataClient.GetThingShadowAsync(new Amazon.IotData.Model.GetThingShadowRequest() { ThingName = id });
            return JObject.Parse(GetStringFromStream(data.Payload));
        }

        public async Task<DeviceJobsModel> GetJobsForDeviceAsync(string id)
        {
            var jobs = new DeviceJobsModel();
            var data = await _awsClientsService.IoTClient.ListJobExecutionsForThingAsync(new Amazon.IoT.Model.ListJobExecutionsForThingRequest { MaxResults = 25, ThingName = id, Status = JobExecutionStatus.QUEUED });  
            foreach(var item in data.ExecutionSummaries)
            {
                var docresp = await _awsClientsService.IoTClient.GetJobDocumentAsync(new GetJobDocumentRequest { JobId = item.JobId });
                var entry = new DeviceJobsEntryModel
                {
                    JobId = item.JobId,
                    JobUrl = _config["BaseSystem:ServerUrl"] + "/api/jobs/" + item.JobId,
                    Status = item.JobExecutionSummary.Status.Value,
                    QueuedAt = item.JobExecutionSummary.QueuedAt,
                    Document = JObject.Parse(docresp.Document)
                };
                jobs.Jobs.Add(entry);
            }
            return jobs;
        }

        private string GetStringFromStream(System.IO.Stream stream)
        {
            // Create a stream reader.
            using (StreamReader reader = new StreamReader(stream))
            {
                // Just read to the end.
                return reader.ReadToEnd();
            }
        }

    }
}
