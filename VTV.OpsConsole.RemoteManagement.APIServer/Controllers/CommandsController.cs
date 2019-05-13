using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VTV.OpsConsole.RemoteManagement.Models;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using Microsoft.Extensions.Configuration;

namespace VTV.OpsConsole.RemoteManagement.APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandSendService _commandSendService;
        private readonly ICommandsManagementService _commandManagementService;
        private readonly IConfiguration _config;


        public CommandsController(ICommandSendService commandSendService, ICommandsManagementService commandManagementService, IConfiguration configuration)
        {
            _commandSendService = commandSendService;
            _commandManagementService = commandManagementService;
            _config = configuration;
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
        [HttpGet("{command}")]
        public ActionResult<CommandTemplate> GetCommandDetails(string command)
        {
            if (!_commandManagementService.CommandExists(command))
                return NotFound();

            return Ok(_commandManagementService.GetCommandTemplate(command));
        }

    }
}
