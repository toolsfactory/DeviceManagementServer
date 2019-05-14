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
        private List<CommandTemplate> _AvailableCommands;
        public IReadOnlyList<CommandTemplate> AvailableCommands => _AvailableCommands;

        public CommandManagementService()
        {
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
                    new CommandParameterTemplate { Name = "firmwareImageFile", Description="Name of the firmware image file", Required=true },
                    new CommandParameterTemplate { Name = "firmwareImageLocation", Description="Url of the server and directory to download the image from. if not provided, default server is used." },
                    new CommandParameterTemplate { Name = "overwriteNewerVersion", Description="is the stb forced to replace newer versions=" }
            }));
            _AvailableCommands.Add(new CommandTemplate("Reboot"));
            _AvailableCommands.Add(new CommandTemplate("GotoPowerState", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "powerState", Description = "Status to send the device to. on, off, standby", Required = true }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetAudioMute", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "muted", Description = "muted or unmuted", Required = true, ParameterType = JTokenType.Boolean }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetAudioVolume", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "volumePercent", Description = "The audio volume. Range 0-100", Required = true, ParameterType = JTokenType.Integer }
            }));
            _AvailableCommands.Add(new CommandTemplate("RunDesasterRecovery"));
            _AvailableCommands.Add(new CommandTemplate("DoFactoryReset"));
            _AvailableCommands.Add(new CommandTemplate("DoPartialReset", parameters: new List<CommandParameterTemplate>()
            {
                    new CommandParameterTemplate { Name = "recordings", Description="Name of the firmware image file", Required=true, ParameterType = JTokenType.Integer },
                    new CommandParameterTemplate { Name = "drm", Description="Url of the server and directory to download the image from. if not provided, default server is used.", Required=true },
                    new CommandParameterTemplate { Name = "network", Description="Name of the firmware image file", Required=true, ParameterType = JTokenType.Integer },
                    new CommandParameterTemplate { Name = "bluetooth", Description="Url of the server and directory to download the image from. if not provided, default server is used.", Required=true },
                    new CommandParameterTemplate { Name = "channellist", Description="Name of the firmware image file", Required=true, ParameterType = JTokenType.Integer },
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
                    new CommandParameterTemplate { Name = "scanMode",   Description="'p' or 'i'" },
                    new CommandParameterTemplate { Name = "frameRate",  Description="framerate", ParameterType = JTokenType.Float }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetHDMIMode", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "mode", Description = "'auto' or 'manual'", Required = true }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetHearingDisabilityMode", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "active", Required = true, ParameterType = JTokenType.Boolean }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetUILanguage", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "language", Required = true, Description = "language as 2 character countrycode" }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetPereferredSubtitleLanguage", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "language", Required = true, Description = "language as 2 character countrycode"}
            }));
            _AvailableCommands.Add(new CommandTemplate("SetSubtitleMode", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "mode", Description = "'on' or 'off'", Required = true }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetActiveStandbyTimer", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "durationMinutes", Description = "duration of the timer in minutes", Required = true, ParameterType = JTokenType.Integer }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetParentalPin", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "pin", Description = "pin", Required = true, ParameterType = JTokenType.Integer }
            }));
            _AvailableCommands.Add(new CommandTemplate("SetParentalMode", parameters: new List<CommandParameterTemplate>()
            {
                new CommandParameterTemplate { Name = "parentalMode", Description = "'enabled' or 'disabled'", Required = true }
            }));
            _AvailableCommands.Add(new CommandTemplate("FlushBatchEvents"));
        }

        public CreateCommandDocumentResponse CreateCommandDocument(CreateCommandDocumentRequest request)
        {
            var tpl = GetCommandTemplate(request.Command);
            var checkreq = CheckForMandatoryParameters(tpl, request);
            if (!checkreq.Success)
                return checkreq;

            var checktyp = CheckForCorrectTypes(tpl, request);
            if (!checktyp.Success)
                return checktyp;

            return new CreateCommandDocumentResponse() { Success = true, Document = BuildDocument(request) };
        }

        JObject BuildDocument(CreateCommandDocumentRequest request)
        {
            var ttlc = new DateTimeOffset(DateTime.UtcNow.AddSeconds(request.TTL)).ToUnixTimeSeconds();
            var parameters = request.Body.SelectToken("parameters");
            var body = new JObject();
            body.Add("operation", "RemoteManagement");
            body.Add("command", request.Command);
            body.Add("cttl", ttlc);
            if (parameters != null)
                body.Add("parameters", parameters);
            return body;
        }

        private CreateCommandDocumentResponse CheckForMandatoryParameters(CommandTemplate tpl, CreateCommandDocumentRequest request)
        {
            if (request.CheckRequired)
            {
                foreach (var item in tpl.Parameters)
                {
                    if (item.Required)
                    {
                        if (request.Body.ContainsKey("parameters"))
                        {
                            var value = request.Body["parameters"][item.Name];
                            if (value == null)
                                return new CreateCommandDocumentResponse() { Success = false, ErrorText = $"Required attribute '{item.Name}' in 'parameters' object missing." };
                        }
                        else
                        {
                            return new CreateCommandDocumentResponse() { Success = false, ErrorText = $"Required object 'parameters'  missing." };
                        }
                    }
                }
            }
            return new CreateCommandDocumentResponse() { Success = true };
        }

        private CreateCommandDocumentResponse CheckForCorrectTypes(CommandTemplate tpl, CreateCommandDocumentRequest request)
        {
            if (request.CheckTypes && request.Body.ContainsKey("parameters"))
            {
                foreach(var item in tpl.Parameters)
                {
                    var value = request.Body["parameters"][item.Name];
                    if (value != null)
                    {
                        if (!AreTypesEqual(value.Type, item.ParameterType))
                        {
                            return new CreateCommandDocumentResponse()
                            {
                                Success = false,
                                ErrorText = $"Invalid type for attribute '{item.Name}' in 'parameters' object. '{value.Type}' instead of '{item.ParameterType}'"
                            };
                        }
                    }
                }
            }
            return new CreateCommandDocumentResponse() { Success = true };
        }

        private bool AreTypesEqual(JTokenType val, JTokenType refval)
        {
            if (refval == JTokenType.Float) 
                if(val == JTokenType.Integer)
                    return true;
            return val == refval;
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