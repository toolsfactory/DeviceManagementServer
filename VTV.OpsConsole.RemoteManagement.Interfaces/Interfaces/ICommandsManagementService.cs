using System.Collections.Generic;
using VTV.OpsConsole.RemoteManagement.Models;

namespace VTV.OpsConsole.RemoteManagement.Interfaces
{
    public interface ICommandsManagementService
    {
        bool CommandTemplatesLoaded { get; }
        bool CommandTemplatesParsed { get; }
        string CommandTemplatesLoadAndParseErrorText { get; }
        string CommandTemplatesSource { get; }
        string CommandTemplateVersion { get; }
        string CommandTemplateAuthor { get; }
        IReadOnlyList<CommandTemplate> AvailableCommands { get; }
        void LoadCommandTemplates();
        bool CommandExists(string command);
        CreateCommandDocumentResponse CreateCommandDocument(CreateCommandDocumentRequest request);
        CommandTemplate GetCommandTemplate(string cmd);
    }
}