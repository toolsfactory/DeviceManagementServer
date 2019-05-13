﻿using System.Collections.Generic;
using VTV.OpsConsole.RemoteManagement.Models;

namespace VTV.OpsConsole.RemoteManagement.Interfaces
{
    public interface ICommandsManagementService
    {
        IReadOnlyList<CommandTemplate> AvailableCommands { get; }
        void LoadCommandTemplates();
        bool CommandExists(string command);
        (Command cmd, bool success) TryParseCommandAndParameters(string command, string body = "");
        CommandTemplate GetCommandTemplate(string cmd);
    }
}