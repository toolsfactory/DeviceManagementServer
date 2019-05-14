using System.Collections.Generic;
using VTV.OpsConsole.RemoteManagement.Models;

namespace VTV.OpsConsole.RemoteManagement.Interfaces
{
    public interface ICommandsManagementService
    {
        IReadOnlyList<CommandTemplate> AvailableCommands { get; }
        void LoadCommandTemplates();
        bool CommandExists(string command);
        CreateCommandDocumentResponse CreateCommandDocument(CreateCommandDocumentRequest request);
        CommandTemplate GetCommandTemplate(string cmd);
    }
}