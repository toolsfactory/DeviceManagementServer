using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VTV.OpsConsole.RemoteManagement.Interfaces;

namespace VTV.OpsConsole.RemoteManagement.APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ICommandsManagementService _cmdManagementSvc;

        public StatusController(IConfiguration config, ICommandsManagementService cmdManagementSvc)
        {
            _config = config;
           _cmdManagementSvc = cmdManagementSvc;
        }
        /// <summary>
        /// Gives an overview of the health and relevant parameters of the mockup service.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<StatusResult> Get()
        {
            var result = new StatusResult()
            {
                Environment = _config["Environment:Name"],
                APIAccessKeyInfo = _config["AWS:ApiAccessKey"].Substring(0, 5),
                ServerBaseUrl = _config["BaseSystem:ServerUrl"],
                CommandsTemplateSource = _cmdManagementSvc.CommandTemplatesSource,
                CommandTemplatesLoaded = _cmdManagementSvc.CommandTemplatesLoaded,
                CommandTemplatesParsed = _cmdManagementSvc.CommandTemplatesParsed,
                CommandsTemplateAuthor = _cmdManagementSvc.CommandTemplateAuthor,
                CommandsTemplateVersion = _cmdManagementSvc.CommandTemplateVersion,
                CommandTemplatesParseAndLoadError = _cmdManagementSvc.CommandTemplatesLoadAndParseErrorText
            };
            return Ok(result);
        }
        /// <summary>
        /// Reloads command templates so that uploading a new json file and calling this method is enough to modify the available commands.
        /// </summary>
        /// <returns></returns>
        [HttpPost("reload")]
        public ActionResult PostReload()
        {
            _cmdManagementSvc.LoadCommandTemplates();
            if (!_cmdManagementSvc.CommandTemplatesParsed)
                return this.BadRequest();
            else
                return Ok();
        }

    }
    public class StatusResult
    {
        public string Environment { get; set; }
        public string APIAccessKeyInfo { get; set; }
        public string ServerBaseUrl { get; set; }
        public string CommandsTemplateSource { get; set; }
        public string CommandsTemplateAuthor { get; set; }
        public string CommandsTemplateVersion { get; set; }
        public bool CommandTemplatesParsed { get; set; }
        public bool CommandTemplatesLoaded { get; set; }
        public string CommandTemplatesParseAndLoadError { get; set; }
    }
}