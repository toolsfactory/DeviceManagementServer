using System.Collections.Generic;

namespace VTV.OpsConsole.RemoteManagement.Models
{
    public class CommandParameterTemplate
    {
        public string Name { get; private set; }
        public bool Required { get; private set; }
        public string ValidationRegex { get; private set; }
        public string Description { get; private set; }
        public CommandParameterType ParameterType { get; private set; }
    }

    public class CommandTemplate
    {
        public string Name { get; private set; }
        public string RequiredClaim { get; private set; }
        public uint MaxTTLInSeconds { get; private set; }
        public uint DefaultTTLInSeconds { get; private set; }
        public IList<CommandParameterTemplate> Parameters { get; private set; }
        public CommandTemplate(string name, string reqClaim, uint maxTTLSec = 60 * 60 * 48, uint defaultTTLSec = 30, IList<CommandParameterTemplate> parameters = null)
        {
            Name = name;
            RequiredClaim = reqClaim;
            MaxTTLInSeconds = maxTTLSec;
            DefaultTTLInSeconds = defaultTTLSec;
            Parameters = parameters;
        }
    }
}
