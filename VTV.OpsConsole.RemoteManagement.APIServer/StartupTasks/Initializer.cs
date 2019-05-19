using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VTV.OpsConsole.RemoteManagement.Interfaces;

namespace VTV.OpsConsole.RemoteManagement.APIServer.StartupTasks
{
    public class Initializer : IStartupTask
    {
        private readonly ICommandsManagementService _cmdManagementSvc;

        public Initializer(ICommandsManagementService cmdManagementSvc)
        {
            _cmdManagementSvc = cmdManagementSvc;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _cmdManagementSvc.LoadCommandTemplates();
            return Task.CompletedTask;
        }
    }
}
