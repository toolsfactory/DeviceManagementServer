using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace VTV.OpsConsole.RemoteManagement.Models
{
    public class CommandParameterTemplate
    {
        public string Name { get; set; }
        public bool Required { get; set; } = false;
        public string AcceptedValues { get; set; } = "";
        public string Description { get; set; }
        public JTokenType ParameterType { get; set; } = JTokenType.String;
    }

    public class CommandTemplate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<CommandParameterTemplate> Parameters { get; set; }
        public CommandTemplate(string name, string description = "", IList<CommandParameterTemplate> parameters = null)
        {
            Name = name;
            Description = description;
            Parameters = parameters;
        }
    }
}
