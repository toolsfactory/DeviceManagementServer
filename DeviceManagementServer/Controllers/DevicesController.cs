using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Amazon;
using Amazon.IoT;
using Amazon.IotData;
using Amazon.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DeviceManagementServer.Services;
using Microsoft.Extensions.Configuration;

namespace DeviceManagementServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly ICommandManagementService _commandManagementService;
        private readonly ICommandExecutionService _commandExecutionService;
        private readonly IConfiguration _config;

        public DevicesController(ICommandManagementService commandManagementService, ICommandExecutionService commandExecutionService, IConfiguration configuration) {
            this._commandManagementService = commandManagementService;
            this._commandExecutionService = commandExecutionService;
            this._config = configuration;
        }

        // GET: /Devices
        [HttpGet]
        public string Get()
        {
            return "No Device List";
        }

        // GET: /Devices/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<Models.DeviceModel>> GetAsync(string id)
        {
            var model = new Models.DeviceModel();
            var awsCredentials = new BasicAWSCredentials(_config["AWS:ApiAccessKey"], _config["AWS:ApiSecretKey"]);
            var client = new AmazonIoTClient(awsCredentials, RegionEndpoint.EUCentral1);
            var response = await client.ListJobsAsync(new Amazon.IoT.Model.ListJobsRequest() { ThingGroupName = "stb-456def" });
            return Ok();
        }

        // GET: /Devices/5/jobs
        [HttpGet("{id}/jobs", Name = "GetDeviceJobs")]
        public async Task<ActionResult> GetJobs(string id)
        {
            var result = await _commandExecutionService.ListJobsForDeviceAsync(id);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return StatusCode((int)result.StatusCode, result.ErrorDescription);

            return Ok(result);
        }

        // POST: api/Devices/{id}/commands/{command}
        [HttpGet("{id}/commands/{command}", Name="GetCommand")]
        public async Task<ActionResult<CommandExecutionResult>> GetCommandAsync(string id, string command)
        {
            if (!_commandManagementService.CommandExists(command))
                return NotFound();

            var cmd = _commandManagementService.TryParseCommandAndParameters(command, "");
            var result = await _commandExecutionService.CreateCommandAsync(id, cmd.Command, "stb-");

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return StatusCode((int)result.StatusCode, result.ErrorDescription);

            return result;
        }

        [HttpPost("{id}/commands/{command}", Name = "PostCommand")]
        public async Task<ActionResult<CommandExecutionResult>> PostCommandAsync(string id, string command, [FromBody] string body)
        {
            if (!_commandManagementService.CommandExists(command))
                return NotFound();

            var cmd = _commandManagementService.TryParseCommandAndParameters(command, "");
            var result = await _commandExecutionService.CreateCommandAsync(id, cmd.Command, "stb-");

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return StatusCode((int)result.StatusCode, result.ErrorDescription);

            return result;
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
