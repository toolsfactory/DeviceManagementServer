using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VTV.OpsConsole.RemoteManagement.Models;
using VTV.OpsConsole.RemoteManagement.APIServer.Models;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using Microsoft.Extensions.Configuration;

namespace VTV.OpsConsole.RemoteManagement.APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandSendService commandSendService;
        private readonly ICommandsManagementService commandManagementService;
        private readonly IConfiguration config;


        public CommandsController(ICommandSendService commandSendService, ICommandsManagementService commandManagementService, IConfiguration configuration)
        {
            this.commandSendService = commandSendService;
            this.commandManagementService = commandManagementService;
            this.config = configuration;
        }

        // GET api/commands
        /// <summary>
        /// List of commands available
        /// </summary>
        /// <returns>queued commands</returns>
        [HttpGet()]
        public ActionResult<IList<CommandInfoModel>> GetCommands()
        {
            var commands = new List<CommandInfoModel>(commandManagementService.AvailableCommands.Count);
            foreach (var item in commandManagementService.AvailableCommands)
            {
                commands.Add(new CommandInfoModel
                {
                    Name = item.Name,
                    Url = config["BaseSystem:ServerUrl"] + "/api/commands/" + item.Name
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
            if (!commandManagementService.CommandExists(command))
                return NotFound();

            return Ok(commandManagementService.GetCommandTemplate(command));
        }

    }
}
