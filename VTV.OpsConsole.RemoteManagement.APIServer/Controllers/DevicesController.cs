﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VTV.OpsConsole.RemoteManagement.Models;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace VTV.OpsConsole.RemoteManagement.APIServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IDevicesService _devicesService;
        private readonly IDeviceDetailsService _deviceDetailsService;
        private readonly ICommandSendService _commandSendService;
        private readonly ICommandsManagementService _commandManagementService;

        public DevicesController(IDevicesService devicesService, IDeviceDetailsService deviceService, ICommandSendService commandSendService, ICommandsManagementService commandManagementService)
        {
            _devicesService = devicesService;
            _deviceDetailsService = deviceService;
            _commandSendService = commandSendService;
            _commandManagementService = commandManagementService;
        }

        // GET api/devices
        /// <summary>
        /// Returns a list of all devices registered in the ThingGroup for this mock
        /// </summary>
        /// <returns>List of devices</returns>
        [HttpGet]
        public async Task<ActionResult<DevicesListModel>> GetAsync()
        {
            var devices = await _devicesService.GetDeviceListAsync();
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
            return Ok(await _deviceDetailsService.GetDeviceDetailsAsync(id));
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
            return Ok(await _deviceDetailsService.GetDeviceShadowAsync(id));
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
            return Ok(await _deviceDetailsService.GetJobsForDeviceAsync(id));
        }

        // POST api/devices/stb-123abc/commands/reboot
        /// <summary>
        /// Sends a RemoteManagement command to the device
        /// </summary>
        /// <param name="id">Unique Id of the device</param>
        /// <param name="command">name of the command to be sent</param>
        /// <param name="parameters">optional parameters send in the body of the request.</param>
        /// <param name="ttl">optional value for the ttl overwriting the default 90 seconds</param>
        /// <param name="checktypes">defines if the system check for correct types of know parameters</param>
        /// <param name="checkmandatory">defines if the system checks that all mandatory fields are there</param>
        /// <param name="checkvalues">defines if the system checks that fields with predefined values are correct</param>
        /// <returns>job details</returns>
        /// <remarks>Remark: if no parameters are available or set, request body must at least be "{}".</remarks>
        [HttpPost("{id}/commands/{command}")]
        public async Task<ActionResult<CommandSendModel>> PostCommandAsync(string id, string command, [FromBody] JObject parameters, [FromQuery] int ttl = 90, [FromQuery] bool checktypes = false, [FromQuery] bool checkmandatory = false, [FromQuery] bool checkvalues = false)
        {
            if (!_commandManagementService.CommandExists(command))
                return NotFound($"Command '{command}' doesn't exist.");
            var response = _commandManagementService.CreateCommandDocument(new CreateCommandDocumentRequest() { Command = command, Body = parameters, TTL = ttl, CheckTypes = checktypes, CheckRequired = checkmandatory });
            if (!response.Success)
                return BadRequest("Invalid command request. "+ response.ErrorText);
            var result = await _commandSendService.SendCommandToDeviceAsync(id, response.Document);
            return result;
        }

        // POST api/devices/stb-123abc/customjob
        /// <summary>
        /// Sends a custom job to the device
        /// </summary>
        /// <param name="id">Unique Id of the device</param>
        /// <param name="parameters">optional parameters send in the body of the request.</param>
        /// <returns>job details</returns>
        /// <remarks>Remark: if no parameters are available or set, request body must at least be "{}".</remarks>
        [HttpPost("{id}/customjob")]
        public async Task<ActionResult<CommandSendModel>> PostCustomJobAsync(string id, [FromBody] JObject parameters)
        {
            var result = await _commandSendService.SendCustomCommandToDeviceAsync(id, parameters);
            return result;
        }
    }
}
