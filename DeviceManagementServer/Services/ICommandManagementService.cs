using System.Collections.Generic;

namespace DeviceManagementServer.Services
{
    public interface ICommandManagementService
    {
        IReadOnlyList<CommandTemplate> AvailableCommands { get; }
        void LoadCommandTemplates();
        bool CommandExists(string command);
        CommandParsingResult TryParseCommandAndParameters(string command, string body = "");
    }
}