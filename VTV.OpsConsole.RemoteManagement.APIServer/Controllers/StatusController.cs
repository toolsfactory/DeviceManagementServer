using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace VTV.OpsConsole.RemoteManagement.APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private IConfiguration _config;

        public StatusController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public ActionResult Get()
        {
            var result = _config["Environment:Name"] + " - " + (_config["AWS:ApiAccessKey"].StartsWith("AKIA")) + " - " + _config["BaseSystem:ServerUrl"];
            return Ok(result);
        }
    }
}