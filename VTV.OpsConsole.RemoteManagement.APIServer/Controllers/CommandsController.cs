using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VTV.OpsConsole.RemoteManagement.Models;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using VTV.OpsConsole.RemoteManagement.APIServer.Authentication;
using System.Threading.Tasks;

namespace VTV.OpsConsole.RemoteManagement.APIServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandSendService _commandSendService;
        private readonly ICommandsManagementService _commandManagementService;
        private readonly IConfiguration _config;
        private readonly IAuthorizationService _authorizationService;

        public CommandsController(ICommandSendService commandSendService, ICommandsManagementService commandManagementService, IConfiguration configuration, IAuthorizationService authorizationService)
        {
            _commandSendService = commandSendService;
            _commandManagementService = commandManagementService;
            _config = configuration;
            _authorizationService = authorizationService;
        }

        // GET api/commands
        /// <summary>
        /// List of commands available
        /// </summary>
        /// <returns>queued commands</returns>
        [HttpGet()]
        public ActionResult<IList<CommandInfoModel>> GetCommands()
        {
            var commands = new List<CommandInfoModel>(_commandManagementService.AvailableCommands.Count);
            foreach (var item in _commandManagementService.AvailableCommands)
            {
                commands.Add(new CommandInfoModel
                {
                    Name = item.Name,
                    Url = _config["BaseSystem:ServerUrl"] + "/api/commands/" + item.Name
                });
            }
            return Ok(commands);
        }

        // GET api/commands/reboot
        /// <summary>
        /// information about a specific command
        /// </summary>
        /// <param name="command">command name</param>
        /// <returns>information</returns>
        /// <remarks>
        /// Possible values for command.parameters.type:
        /// Object = 1
        /// Array = 2
        /// Integer = 6
        /// Float = 7
        /// String = 8
        /// Boolean = 9
        /// </remarks>
        [HttpGet("{command}")]
        public async Task<ActionResult<CommandTemplate>> GetCommandDetails(string command)
        {
            var res = new AuthorizationDetails("de", command);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, res, "OpcoPolicy");
            if (!authorizationResult.Succeeded)
                return Forbid();
            if (!_commandManagementService.CommandExists(command))
                return NotFound();

            return Ok(_commandManagementService.GetCommandTemplate(command));
        }

    }
}
