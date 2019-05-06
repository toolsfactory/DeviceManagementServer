using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagementServer.Services
{
    public interface ICommandExecutionService
    {
        Task<CommandExecutionResult> CreateCommandAsync(string deviceId, Command cmd, string idprefix = "");
        Task<CommandsForDeviceResult> ListPendingJobsForDeviceAsync(string deviceId);
        Task<CommandsForDeviceResult> ListJobsForDeviceAsync(string deviceId);
    }
}
