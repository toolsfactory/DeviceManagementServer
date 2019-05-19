using System;
using System.Collections.Generic;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using VTV.OpsConsole.RemoteManagement.Models;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;

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

        public void LoadCommandTemplates()
        {
            /*
            var assembly = Assembly.GetCallingAssembly();
            var resourceName = "VTV.OpsConsole.RemoteManagement.Services.commands.json";
            string json = "";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }
            */
            JObject o = JObject.Parse(Commands.Text);

            _AvailableCommands = o["commands"].ToObject<List<CommandTemplate>>();
        }

        public bool CommandExists(string command)
        {
            return GetCommandTemplate(command) != null;
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

            var checkval = CheckForCorrectValues(tpl, request);
            if (!checkval.Success)
                return checkval;

            return new CreateCommandDocumentResponse() { Success = true, Document = BuildDocument(request) };
        }

        JObject BuildDocument(CreateCommandDocumentRequest request)
        {
            var message = new JObject();
            message.Add("command", request.Command);
            if (request.Body.Count > 0)
                message.Add("parameters", request.Body);
            var document = new JObject();
            document.Add("operation", "remoteManagement");
            document.Add("ttl", new DateTimeOffset(DateTime.UtcNow.AddSeconds(request.TTL)).ToUnixTimeSeconds());
            document.Add("message", message);
            return document;
        }

        private CreateCommandDocumentResponse CheckForMandatoryParameters(CommandTemplate tpl, CreateCommandDocumentRequest request)
        {
            if (request.CheckRequired)
            {
                foreach (var item in tpl.Parameters)
                {
                    if (item.Required)
                    {
                        var value = request.Body[item.Name];
                        if (value == null)
                            return new CreateCommandDocumentResponse() { Success = false, ErrorText = $"Required attribute '{item.Name}' in 'parameters' object missing." };
                    }
                }
            }
            return new CreateCommandDocumentResponse() { Success = true };
        }

        private CreateCommandDocumentResponse CheckForCorrectTypes(CommandTemplate tpl, CreateCommandDocumentRequest request)
        {
            foreach(var item in tpl.Parameters)
            {
                var value = request.Body[item.Name];
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
            return new CreateCommandDocumentResponse() { Success = true };
        }

        private CreateCommandDocumentResponse CheckForCorrectValues(CommandTemplate tpl, CreateCommandDocumentRequest request)
        {
            foreach (var item in tpl.Parameters)
            {
                if (String.IsNullOrEmpty(item.AcceptedValues))
                    continue;
                var accepted = item.AcceptedValues.Split(',');
                var value = request.Body[item.Name];
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
                    if (Array.IndexOf(accepted, value.ToString()) == -1)
                    {
                        return new CreateCommandDocumentResponse()
                        {
                            Success = false,
                            ErrorText = $"Invalid value for attribute '{item.Name}' in 'parameters' object. '{value.ToString()}' instead of '{item.AcceptedValues}'"
                        };
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