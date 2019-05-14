using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace VTV.OpsConsole.RemoteManagement.Models
{
    public enum CommandParameterType
    {
        String,
        Integer,
        Boolean
    }

    public class CommandParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public JTokenType ParameterType { get; set; }
    }

    public class Command
    {
        public string Name { get; set; }
        public IList<CommandParameter> Parameters { get; set; }
        public Command(string name, IList<CommandParameter> parameters = null)
        {
            Name = name;
            Parameters = (parameters == null) ? new List<CommandParameter>(10) : parameters;
        }
    }
}
