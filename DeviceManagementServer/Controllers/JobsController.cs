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

namespace DeviceManagementServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobManagementService _jobManagementService;

        public JobsController(IJobManagementService jobManagementService)
        {
            this._jobManagementService = jobManagementService;
        }

        // GET: /Jobs/5
        [HttpGet("{id}", Name = "GetJobs")]
        public async Task<ActionResult<JobDetailsResult>> GetAsync(string id)
        {
            var result = await _jobManagementService.GetJobDetailsAsync(id);
            return result;
        }

    }
}
