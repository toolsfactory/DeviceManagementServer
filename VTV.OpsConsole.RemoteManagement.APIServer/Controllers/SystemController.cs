using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using VTV.OpsConsole.RemoteManagement.APIServer.Authentication;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

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
        public ActionResult<StatusResponse> Get()
        {
            var result = new StatusResponse()
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
        /// Reloads commands template from S3 bucket. Required after an Upload to activate the new configuration.
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

        /// <summary>
        /// Uploads a new commands template into the central bucket
        /// </summary>
        /// <param name="template">The template in correct JSON format</param>
        /// <returns>Details regarding the uploaded template</returns>
        [HttpPost("upload")]
        public async Task<ActionResult<Models.UploadCommandsTemplateResponse>> PostUploadAsync([FromBody] JObject template)
        {
            var result =await _cmdManagementSvc.UploadCommandsTemplateAsync(template);
            if (!result.Success)
                return this.BadRequest(result.ErrorText);
            else
                return Ok(new Models.UploadCommandsTemplateResponse { Author = result.Author, Version = result.Version, CommandsCount = result.CommandsCount });
        }

        /// <summary>
        /// Use this API to authenticate against the system. In Swagger, copy the returned token from the JSON and insert it in the "Authorize" Dialog as string "Bearer {yourtoken}".
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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