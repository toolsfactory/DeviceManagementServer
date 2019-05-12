using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VTV.OpsConsole.RemoteManagement.Models;
using VTV.OpsConsole.RemoteManagement.APIServer.Models;
using VTV.OpsConsole.RemoteManagement.Interfaces;

namespace VTV.OpsConsole.RemoteManagement.APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IDevicesService devicesService;
        private readonly IDeviceDetailsService deviceService;
        private readonly ICommandSendService commandSendService;
        private readonly ICommandsManagementService commandManagementService;

        public DevicesController(IDevicesService devicesService, IDeviceDetailsService deviceService, ICommandSendService commandSendService, ICommandsManagementService commandManagementService)
        {
            this.devicesService = devicesService;
            this.deviceService = deviceService;
            this.commandSendService = commandSendService;
            this.commandManagementService = commandManagementService;
        }

        // GET api/devices
        /// <summary>
        /// Returns a list of all devices registered in the ThingGroup for this mock
        /// </summary>
        /// <returns>List of devices</returns>
        [HttpGet]
        public async Task<ActionResult<DevicesListModel>> GetAsync()
        {
            var devices = await devicesService.GetDeviceListAsync();
            return Ok(devices);
        }

        // GET api/devices/stb-123abc
        /// <summary>
        /// Loads details for a specific device
        /// </summary>
        /// <param name="id">Unique Id of the device</param>
        /// <returns>Details of the device</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceDetailsModel>> GetDeviceAsync(string id)
        {
            return Ok(await deviceService.GetDeviceDetailsAsync(id));
        }

        // GET api/devices/stb-123abc/shadow
        /// <summary>
        /// Loads the device shadow from AWS IoT Core for the specific device
        /// </summary>
        /// <param name="id">Unique Id of the device</param>
        /// <returns>DEvice shadow json</returns>
        [HttpGet("{id}/shadow")]
        public async Task<ActionResult<JObject>> GetDeviceShadowAsync(string id)
        {
            return Ok(await deviceService.GetDeviceShadowAsync(id));
        }

        // GET api/devices/stb-123abc/commands
        /// <summary>
        /// List of commands currently in the queue
        /// </summary>
        /// <param name="id">Unique Id of the device</param>
        /// <returns>queued commands</returns>
        [HttpGet("{id}/commands")]
        public async Task<ActionResult<IReadOnlyList<DeviceJobsModel>>> GetDeviceCommands(string id)
        {
            return Ok(await deviceService.GetJobsForDeviceAsync(id));
        }

        // POST api/devices/stb-123abc/commands/reboot
        /// <summary>
        /// Sends a RemoteManagement command to the device
        /// </summary>
        /// <param name="id">Unique Id of the device</param>
        /// <param name="command">name of the command to be sent</param>
        /// <param name="parameters">optional parameters send in the body of the request.</param>
        /// 
        /// <returns>job details</returns>
        /// <remarks>Remark: if no parameters are available or set, request body must at least be "{}".</remarks>
        [HttpPost("{id}/commands/{command}")]
        public async Task<ActionResult<JobSendResult>> PostCommandAsync(string id, string command, [FromBody] JObject parameters)
        {
            if (!commandManagementService.CommandExists(command))
                return NotFound();

            var cmd = commandManagementService.TryParseCommandAndParameters(command, "");
            var result = await commandSendService.SendCommandToDeviceAsync(id, cmd.Command);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return StatusCode((int)result.StatusCode, result.ErrorDescription);
            
            return result;
        }

        // POST api/devices/stb-123abc/customjob
        /// <summary>
        /// Sends a custom job to the device
        /// </summary>
        /// <param name="id">Unique Id of the device</param>
        /// <param name="parameters">optional parameters send in the body of the request.</param>
        /// 
        /// <returns>job details</returns>
        /// <remarks>Remark: if no parameters are available or set, request body must at least be "{}".</remarks>
        [HttpPost("{id}/customjob")]
        public async Task<ActionResult<JobSendResult>> PostCustomJobAsync(string id, [FromBody] JObject parameters)
        {
            var result = await commandSendService.SendCustomJobToDeviceAsync(id, parameters);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return StatusCode((int)result.StatusCode, result.ErrorDescription);

            return result;
        }
    }
}
