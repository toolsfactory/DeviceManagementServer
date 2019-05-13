using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using VTV.OpsConsole.RemoteManagement.Models;

namespace VTV.OpsConsole.RemoteManagement.Interfaces
{
    public interface ICommandSendService
    {
        Task<CommandSendModel> SendCommandToDeviceAsync(string deviceid, Command command, JObject parameters);
        Task<CommandSendModel> SendCustomCommandToDeviceAsync(string deviceId, JObject data);
    }
}
