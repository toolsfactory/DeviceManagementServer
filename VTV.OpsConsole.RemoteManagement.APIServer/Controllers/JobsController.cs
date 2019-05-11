using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VTV.OpsConsole.RemoteManagement.Interfaces;

namespace VTV.OpsConsole.RemoteManagement.APIServer.Controllers
{
    [Route("api/[controller]")]
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