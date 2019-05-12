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
        /// <summary>
        /// Status of the job
        /// </summary>
        /// <param name="id">Uniqued id of the job</param>
        /// <returns>Status details</returns>
        [HttpGet("{id}", Name = "GetJob")]
        public async Task<ActionResult<JobDetailsResult>> GetJobAsync(string id)
        {
            var result = await _jobManagementService.GetJobDetailsAsync(id);
            return result;
        }

        // DELETE: /Jobs/5
        /// <summary>
        /// Delete a specific job
        /// </summary>
        /// <param name="id">Unique id of the job</param>
        /// <returns>Code</returns>
        [HttpDelete("{id}", Name = "DeleteJob")]
        public async Task<ActionResult> DeleteJobAsync(string id)
        {
            await _jobManagementService.DeleteJobAsync(id);
            return Ok();
        }

    }
}