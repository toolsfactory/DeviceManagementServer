using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using VTV.OpsConsole.RemoteManagement.Models;

namespace VTV.OpsConsole.RemoteManagement.APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobManagementService _jobManagementService;

        public JobsController(IJobManagementService jobManagementService)
        {
            _jobManagementService = jobManagementService;
        }

        // GET: /Jobs/5
        /// <summary>
        /// Status of the job
        /// </summary>
        /// <param name="id">Uniqued id of the job</param>
        /// <returns>Status details</returns>
        [HttpGet("{id}", Name = "GetJob")]
        public async Task<ActionResult<JobDetailsModel>> GetJobAsync(string id)
        {
            var result = await _jobManagementService.GetJobDetailsAsync(id);
            return result;
        }

        // GET: /Jobs/5/document
        /// <summary>
        /// Job document of the job
        /// </summary>
        /// <param name="id">Uniqued id of the job</param>
        /// <returns>document</returns>
        [HttpGet("{id}/document", Name = "GetJobDocument")]
        public async Task<ActionResult<JObject>> GetJobDocumentAsync(string id)
        {
            var result = await _jobManagementService.GetJobDocumentAsync(id);
            return Ok(result);
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