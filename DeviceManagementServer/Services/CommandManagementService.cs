using System;
using System.Collections.Generic;

namespace DeviceManagementServer.Services
{
    public class CommandManagementService : ICommandManagementService
    {
        private List<CommandTemplate> _AvailableCommands;
        public IReadOnlyList<CommandTemplate> AvailableCommands => _AvailableCommands;

        public CommandManagementService()
        {
            _AvailableCommands = new List<CommandTemplate>(3);
            LoadCommandTemplates();
        }

        public bool CommandExists(string command)
        {
            return GetCommandTemplateFrom(command) != null;
        }

        public void LoadCommandTemplates()
        {
            _AvailableCommands.Add(new CommandTemplate("Reboot", "cmd.reboot", defaultTTLSec: 15));
            _AvailableCommands.Add(new CommandTemplate("UpdateFirmware", "cmd.updateFW", defaultTTLSec: 15));
            _AvailableCommands.Add(new CommandTemplate("RunDesasterRecovery", "cmd.desasterRec", defaultTTLSec: 15));
            _AvailableCommands.Add(new CommandTemplate("FactoryReset", "cmd.facoryRes", defaultTTLSec: 15));
        }

        public CommandParsingResult TryParseCommandAndParameters(string command, string body = "")
        {
            var template = GetCommandTemplateFrom(command);
            if (template == null)
            {
                return new CommandParsingResult() { StatusCode = System.Net.HttpStatusCode.NotFound };
            }
            var cmd = new Command(template.Name, template.DefaultTTLInSeconds);
            return new CommandParsingResult() { StatusCode = System.Net.HttpStatusCode.OK, Command = cmd };
        }

        private CommandTemplate GetCommandTemplateFrom(string command)
        {
            foreach (var cmd in _AvailableCommands)
            {
                if (command.ToLower() == cmd.Name.ToLower())
                    return cmd;
            }
            return null;

        }
    }
}