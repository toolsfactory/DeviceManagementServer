using System.Collections.Generic;

namespace VTV.OpsConsole.RemoteManagement.Models
{
    public class CommandParameterTemplate
    {
        public string Name { get; set; }
        public bool Required { get; set; } = false;
        public string ValidationRegex { get; set; } = "";
        public string Description { get; set; }
        public CommandParameterType ParameterType { get; set; } = CommandParameterType.String;
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
