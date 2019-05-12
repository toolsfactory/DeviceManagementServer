using System;
using System.Threading.Tasks;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using VTV.OpsConsole.RemoteManagement.Models;
using Microsoft.Extensions.Configuration;

namespace VTV.OpsConsole.RemoteManagement.Services
{
    public class DevicesService : IDevicesService
    {
        private readonly IConfiguration config;
        private readonly IAWSClientsService awsClientsService;

        public DevicesService(IAWSClientsService awsClientsService, IConfiguration configuration)
        {
            this.awsClientsService = awsClientsService;
            this.config = configuration;
        }

        public async Task<DevicesListModel> GetDeviceListAsync()
        {
            try
            {
                var data = await awsClientsService.IoTClient.ListThingsInThingGroupAsync(new Amazon.IoT.Model.ListThingsInThingGroupRequest() { MaxResults = 25, ThingGroupName = "STB_GC_MOCK" });
                var result = new DevicesListModel();
                foreach (var item in data.Things)
                {
                    result.Devices.Add(new DevicesListEntryModel() { DeviceId = item, DeviceName = item, DeviceUrl = config["BaseSystem:ServerUrl"] + "/api/devices/"+item });
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
