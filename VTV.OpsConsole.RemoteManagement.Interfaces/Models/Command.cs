using System.Collections.Generic;

namespace VTV.OpsConsole.RemoteManagement.Models
{
    public enum CommandParameterType
    {
        String,
        Integer
    }

    public class CommandParameter
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
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
}
