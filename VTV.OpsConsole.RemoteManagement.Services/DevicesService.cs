using System;
using System.Threading.Tasks;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using VTV.OpsConsole.RemoteManagement.Models;
using Microsoft.Extensions.Configuration;

namespace VTV.OpsConsole.RemoteManagement.Services
{
    public class DevicesService : IDevicesService
    {
        private readonly IConfiguration _config;
        private readonly IAWSClientsService _awsClientsService;

        public DevicesService(IAWSClientsService awsClientsService, IConfiguration configuration)
        {
            _awsClientsService = awsClientsService;
            _config = configuration;
        }

        public async Task<DevicesListModel> GetDeviceListAsync()
        {
            var data = await _awsClientsService.IoTClient.ListThingsInThingGroupAsync(new Amazon.IoT.Model.ListThingsInThingGroupRequest() { MaxResults = 100, ThingGroupName = _config["BaseSystem:ThingGroup"] });
            var result = new DevicesListModel();
            foreach (var item in data.Things)
            {
                result.Devices.Add(new DevicesListEntryModel()
                {
                    DeviceId = item,
                    DeviceName = item,
                    DeviceUrl = _config["BaseSystem:ServerUrl"] + "/api/devices/"+item
                });
            }
            return result;
        }
    }
}
