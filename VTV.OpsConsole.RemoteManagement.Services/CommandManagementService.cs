using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon.IoT;
using Amazon.IoT.Model;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using VTV.OpsConsole.RemoteManagement.Models;
using Newtonsoft.Json.Linq;

namespace VTV.OpsConsole.RemoteManagement.Services
{
    public class CommandManagementService : ICommandsManagementService
    {
        private readonly IConfiguration _config;
        private List<CommandTemplate> _AvailableCommands;
        public IReadOnlyList<CommandTemplate> AvailableCommands => _AvailableCommands;

        public CommandManagementService(IConfiguration config)
        {
            _config = config;
            _AvailableCommands = new List<CommandTemplate>(3);
            LoadCommandTemplates();
        }

        public bool CommandExists(string command)
        {
            return GetCommandTemplate(command) != null;
        }

        public void LoadCommandTemplates()
        {
            _AvailableCommands.Add(new CommandTemplate("UpdateFirmware"));
            _AvailableCommands.Add(new CommandTemplate("UpdateToSpecificFirmware", parameters: new List<CommandParameterTemplate>()
            {
                    new CommandParameterTemplate { Name = "firmwareImageFile", Description="Name of the firmware image file", Required=true, ParameterType = CommandParameterType.Integer },
                    new CommandParameterTemplate { Name = "firmwareImageLocation", Description="Url of the server and directory to download the image from. if not provided, default server is used.", Required=true },
                    new CommandParameterTemplate { Name = "overwriteNewerVersion", Description="is the stb forced to replace newer versions=" }
            }));
            _AvailableCommands.Add(new CommandTemplate("Reboot"));
            _AvailableCommands.Add(new CommandTemplate("GotoPowerState", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "powerstate", Description = "Status to send the device to. on, off, standby", Required = true }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetAudioMute", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "muted", Description = "muted or unmuted", Required = true, ParameterType = CommandParameterType.boolean }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetAudioVolume", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "volumepercent", Description = "The audio volume. Range 0-100", Required = true, ParameterType = CommandParameterType.boolean }
            }));
            _AvailableCommands.Add(new CommandTemplate("RunDesasterRecovery"));
            _AvailableCommands.Add(new CommandTemplate("DoFactoryReset"));
            _AvailableCommands.Add(new CommandTemplate("DoPartialReset", parameters: new List<CommandParameterTemplate>()
            {
                    new CommandParameterTemplate { Name = "recordings", Description="Name of the firmware image file", Required=true, ParameterType = CommandParameterType.Integer },
                    new CommandParameterTemplate { Name = "drm", Description="Url of the server and directory to download the image from. if not provided, default server is used.", Required=true },
                    new CommandParameterTemplate { Name = "network", Description="Name of the firmware image file", Required=true, ParameterType = CommandParameterType.Integer },
                    new CommandParameterTemplate { Name = "bluetooth", Description="Url of the server and directory to download the image from. if not provided, default server is used.", Required=true },
                    new CommandParameterTemplate { Name = "channellist", Description="Name of the firmware image file", Required=true, ParameterType = CommandParameterType.Integer },
                    new CommandParameterTemplate { Name = "favorites", Description="is the stb forced to replace newer versions=" }
            }));
            _AvailableCommands.Add(new CommandTemplate("RunChannelScan"));
            _AvailableCommands.Add(new CommandTemplate("RunRegionDiscovery"));
            _AvailableCommands.Add(new CommandTemplate("HideChannels"));
            _AvailableCommands.Add(new CommandTemplate("RunSpeedTest"));
            _AvailableCommands.Add(new CommandTemplate("DeleteLocalRecording", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "recID", Description = "Id of the planned recording", Required = true }
            }));
            _AvailableCommands.Add(new CommandTemplate("DeleteLocalPlannedRecording", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "recID", Description = "Id of the planned recording", Required = true }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetStandbyMode", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "mode", Description = "'active standby' or 'passive standby'", Required = true }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetHDMIResolution", parameters: new List<CommandParameterTemplate>()
            {
                    new CommandParameterTemplate { Name = "resolution", Description="Resolution in WxH format. 1920x1080" },
                    new CommandParameterTemplate { Name = "scanmode",   Description="'p' or 'i'" },
                    new CommandParameterTemplate { Name = "framerate",  Description="framerate" }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetHDMIMode", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "mode", Description = "'auto' or 'manual'", Required = true }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetHearingDisabilityMode", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "active", Required = true, ParameterType = CommandParameterType.boolean }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetUILanguage", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "language", Required = true, Description = "language as 2 character countrycode", ParameterType = CommandParameterType.String }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetPereferredSubtitleLanguage", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "language", Required = true, Description = "language as 2 character countrycode", ParameterType = CommandParameterType.String }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetSubtitleMode", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "mode", Description = "'on' or 'off'", Required = true }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetActiveStandbyTimer", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "durationminutes", Description = "duration of the timer in minutes", Required = true, ParameterType = CommandParameterType.Integer }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetParentalPin", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "pin", Description = "pin", Required = true, ParameterType = CommandParameterType.Integer }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetParentalMode", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "parentalMode", Description = "'enabled' or 'disabled'", Required = true }
            }));
            _AvailableCommands.Add(new CommandTemplate("FlushBatchEvents"));
        }

        public (Command cmd, bool success) TryParseCommandAndParameters(string command, string body = "")
        {
            var template = GetCommandTemplate(command);
            if (template == null)
                return (null, false);
            var cmd = new Command(template.Name, uint.Parse(_config["BaseSystem:TTL"]));
            return (cmd, true);
        }

        public CommandTemplate GetCommandTemplate(string command)
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