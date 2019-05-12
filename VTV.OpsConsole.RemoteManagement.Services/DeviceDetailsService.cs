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
        private readonly IConfiguration config;
        private readonly IAWSClientsService awsClientsService;

        public DeviceDetailsService(IAWSClientsService awsClientsService, IConfiguration configuration)
        {
            this.awsClientsService = awsClientsService;
            this.config = configuration;
        }

        public async Task<DeviceDetailsModel> GetDeviceDetailsAsync(string id)
        {
            try
            {
                var data = await awsClientsService.IoTClient.DescribeThingAsync(new Amazon.IoT.Model.DescribeThingRequest() { ThingName = id });
                var result = new DeviceDetailsModel()
                {
                    Name = data.ThingName,
                    Id = data.ThingId,
                    Version = data.Version,
                    TypeName = data.ThingTypeName,
                    CommandsUrl = config["BaseSystem:ServerUrl"] + "/api/devices/" + id + "/commands"
                };
                foreach(var attr in data.Attributes)
                { result.Attributes.Add(attr.Key, attr.Value); }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<JObject> GetDeviceShadowAsync(string id)
        {
            try
            {
                var data = await awsClientsService.IoTDataClient.GetThingShadowAsync(new Amazon.IotData.Model.GetThingShadowRequest() { ThingName = id });
                return JObject.Parse(GetStringFromStream(data.Payload));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<DeviceJobsModel> GetJobsForDeviceAsync(string id)
        {
            var jobs = new DeviceJobsModel();
            var r = new Regex("\"command\":\"([a-z_-]*)\"", RegexOptions.IgnoreCase);
            try
            {
                var data = await awsClientsService.IoTClient.ListJobExecutionsForThingAsync(new Amazon.IoT.Model.ListJobExecutionsForThingRequest { MaxResults = 25, ThingName = id, Status = JobExecutionStatus.QUEUED });  
                foreach(var item in data.ExecutionSummaries)
                {
                    var docresp = await awsClientsService.IoTClient.GetJobDocumentAsync(new GetJobDocumentRequest { JobId = item.JobId });
                    var m = r.Match(docresp.Document);
                    var entry = new DeviceJobsEntryModel
                    {
                        JobId = item.JobId,
                        JobUrl = config["BaseSystem:ServerUrl"] + "/api/jobs/" + item.JobId,
                        Status = item.JobExecutionSummary.Status.Value,
                        QueuedAt = item.JobExecutionSummary.QueuedAt,
                        Command = (m.Success) ? m.Groups[1].Value : "Unknown command"
                    };
                    jobs.Jobs.Add(entry);
                }
                return jobs;
            }
            catch (Exception ex)
            {
                throw;
            }
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
