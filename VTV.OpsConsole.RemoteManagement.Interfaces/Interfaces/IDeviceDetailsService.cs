using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using VTV.OpsConsole.RemoteManagement.Models;

namespace VTV.OpsConsole.RemoteManagement.Interfaces
{
    public interface IDeviceDetailsService
    {
        Task<Models.DeviceDetailsModel> GetDeviceDetailsAsync(string id);
        Task<JObject> GetDeviceShadowAsync(string id);
        Task<DeviceJobsModel> GetJobsForDeviceAsync(string id);
    }
}
