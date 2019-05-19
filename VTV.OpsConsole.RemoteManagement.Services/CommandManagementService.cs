using System;
using System.Collections.Generic;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using VTV.OpsConsole.RemoteManagement.Models;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace VTV.OpsConsole.RemoteManagement.Services
{
    public class CommandManagementService : ICommandsManagementService
    {
        private List<CommandTemplate> _AvailableCommands;
        private readonly IConfiguration _config;

        public IReadOnlyList<CommandTemplate> AvailableCommands => _AvailableCommands;

        public bool CommandTemplatesLoaded { get; private set; }

        public bool CommandTemplatesParsed { get; private set; }

        public string CommandTemplatesLoadAndParseErrorText { get; private set; }

        public string CommandTemplatesSource { get; private set; }

        public string CommandTemplateVersion { get; private set; }

        public string CommandTemplateAuthor { get; private set; }

        public CommandManagementService(IConfiguration config)
        {
            _AvailableCommands = new List<CommandTemplate>(3);
            _config = config;
            LoadCommandTemplates();
        }

        private void CleanUpStatus()
        {
            CommandTemplatesLoaded = false;
            CommandTemplatesParsed = false;
            CommandTemplatesSource = "";
            CommandTemplatesLoadAndParseErrorText = "";
            if (_AvailableCommands != null)
                _AvailableCommands.Clear();
            _AvailableCommands = null;
            CommandTemplateVersion = "";
            CommandTemplateAuthor = "";
        }

        public void LoadCommandTemplates()
        {
            CleanUpStatus();
            string cmds = "";
            try
            {
                CommandTemplatesSource = _config["AWS:S3CommandsURL"];
                cmds = LoadCommandsFromS3();
                CommandTemplatesLoaded = true;
            }
            catch (Exception ex)
            {
                CommandTemplatesLoadAndParseErrorText = "Commands json could not be loaded. " + ex.Message;
                return;
            }
            try
            {
                JObject o = JObject.Parse(cmds);
                _AvailableCommands = o["commands"].ToObject<List<CommandTemplate>>();
                CommandTemplateVersion = o["version"].ToString();
                CommandTemplateAuthor = o["author"].ToString();
                CommandTemplatesParsed = true;
            }
            catch (Exception ex)
            {
                CommandTemplatesLoadAndParseErrorText = "Commands json could not be parsed. " + ex.Message;
            }

        }

        private string LoadCommandsFromS3()
        {
            WebClient client = new WebClient();
            Stream data = client.OpenRead(CommandTemplatesSource);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            data.Close();
            reader.Close();
            return s;
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