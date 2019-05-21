using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VTV.OpsConsole.RemoteManagement.APIServer.StartupTasks
{
    public interface IStartupTask
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
