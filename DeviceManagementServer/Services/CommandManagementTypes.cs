using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagementServer.Services
{
    public enum CommandParameterType
    {
        String,
        Integer
    }
    public class CommandTemplate
    {
        public string Name { get; private set; }
        public string RequiredClaim { get; private set; }
        public uint MaxTTLInSeconds { get; private set; }
        public uint DefaultTTLInSeconds { get; private set; }
        public IList<CommandParameterTemplate> Parameters { get; private set; }
        public CommandTemplate(string name, string reqClaim, uint maxTTLSec = 60*60*48, uint defaultTTLSec = 30, IList<CommandParameterTemplate> parameters = null)
        {
            Name = name;
            RequiredClaim = reqClaim;
            MaxTTLInSeconds = maxTTLSec;
            DefaultTTLInSeconds = defaultTTLSec;
            Parameters = parameters;
        }
    }

    public class CommandParameterTemplate
    {
        public string Name { get; private set; }
        public bool Required { get; private set; }
        public string ValidationRegex { get; private set; }
        public string Description { get; private set; }
        public CommandParameterType ParameterType { get; private set; }
    }

    public class Command
    {
        public string Name { get; private set; }
        public uint TTL { get; private set; }
        public IList<CommandParameter> Parameters { get; private set; }
        public Command(string name, uint ttl, IList<CommandParameter> parameters = null)
        {
            Name = name;
            TTL = ttl;
            Parameters = parameters;
        }
    }

    public class CommandParameter
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public CommandParameterType ParameterType { get; private set; }
    }

    public class CommandParsingResult
    {
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public Command Command { get; set; }
    }

    public class CommandExecutionResult
    {
        public System.Net.HttpStatusCode StatusCode { get;  set; }
        public string ErrorDescription { get; set; }
        public string CommandId { get;  set; }
    }

    public class CommandStatus
    {
        public string Id { get; set;}
        public string Status { get; set; }
        public DateTime QueuedAt { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
    public class CommandsForDeviceResult
    {
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public string ErrorDescription { get; set; }
        public IList<CommandStatus> Commands { get; set; }
    }
}