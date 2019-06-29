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
using Amazon.S3;
using Amazon.S3.Transfer;
using System.Text;
using System.Threading.Tasks;

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
        public IAWSClientsService AwsClients { get; }

        public CommandManagementService(IConfiguration config, IAWSClientsService awsClients)
        {
            _AvailableCommands = new List<CommandTemplate>(3);
            _config = config;
            AwsClients = awsClients;
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

        public async Task<UploadCommandsTemplateResult> UploadCommandsTemplateAsync(JObject template)
        {
            var cmdcnt = VerifyNewTemplate(template, out var errortext);
            if ( cmdcnt <= 0)
            {
                return new UploadCommandsTemplateResult { Success = false, ErrorCode = 1, ErrorText = errortext };
            }
            return await SaveCommandsTemplateToS3Async(template, cmdcnt);
        }

        private int VerifyNewTemplate(JObject template, out string errortext)
        {
            errortext = "";
            try
            {
                var cmdcount = template["commands"].ToObject<List<CommandTemplate>>().Count;
                if (cmdcount <= 0)
                {
                    errortext = "no commands in template";
                    return -1;
                }
                template["version"] = DateTime.Now.ToString("d");
                if (String.IsNullOrEmpty(template["author"].ToString()))
                {
                    template["author"] = "Unknown";
                }
                return cmdcount;
            }
            catch (Exception ex)
            {
                errortext = "Error parsing JSON. " + ex.Message;
                return -1;
            }
        }
        
        private async Task<UploadCommandsTemplateResult> SaveCommandsTemplateToS3Async(JObject template, int cmdcnt)
        {
            try
            {
                byte[] byteArray = Encoding.ASCII.GetBytes(template.ToString());
                MemoryStream stream = new MemoryStream(byteArray);
                var req = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    BucketName = _config["AWS:S3Bucket"],
                    Key = _config["AWS:S3Filename"],
                    CannedACL = S3CannedACL.PublicRead
                };
                var x = new TransferUtility(AwsClients.S3Client);
                await x.UploadAsync(req);
                return new UploadCommandsTemplateResult
                            {
                                Success = true,
                                Author = template["author"].ToString(),
                                Version = template["version"].ToString(),
                                CommandsCount = cmdcnt
                            };
            }
            catch (Exception ex)
            {
                return new UploadCommandsTemplateResult() { Success = false, ErrorCode = 2, ErrorText = "Commands json could not be uploaded. " + ex.Message };
            }
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
            if (tpl.Parameters == null)
            {
                return new CreateCommandDocumentResponse() { Success = true };
            }
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
            if(tpl.Parameters == null)
            {
                return new CreateCommandDocumentResponse() { Success = true };
            }
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
            if (tpl.Parameters == null)
            {
                return new CreateCommandDocumentResponse() { Success = true };
            }
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