using System;
using System.IO;
using System.Threading.Tasks;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using VTV.OpsConsole.RemoteManagement.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace VTV.OpsConsole.RemoteManagement.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IConfiguration config;
        private readonly IAWSClientsService awsClientsService;

        public DeviceService(IAWSClientsService awsClientsService, IConfiguration configuration)
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

        private string GetStringFromStream(Stream stream)
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
