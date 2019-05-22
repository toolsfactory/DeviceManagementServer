using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using VTV.OpsConsole.RemoteManagement.APIServer.Authentication;



namespace VTV.OpsConsole.RemoteManagement.APIServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ICommandsManagementService _cmdManagementSvc;
        private readonly IUserService _userService;

        public SystemController(IConfiguration config, ICommandsManagementService cmdManagementSvc, IUserService userService)
        {
            _userService = userService;
            _config = config;
           _cmdManagementSvc = cmdManagementSvc;
        }
        /// <summary>
        /// Gives an overview of the health and relevant parameters of the mockup service.
        /// </summary>
        /// <returns></returns>
        [HttpGet("status")]
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

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]Models.UserAuthenticateRequestModel model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password incorrect" });

            var result = _userService.GenerateToken(user);
            return Ok(result);
        }
    }
}