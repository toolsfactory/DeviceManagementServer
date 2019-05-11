using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace VTV.OpsConsole.RemoteManagement.Interfaces
{
    public interface IDeviceService
    {
        Task<Models.DeviceDetailsModel> GetDeviceDetailsAsync(string id);
        Task<JObject> GetDeviceShadowAsync(string id);
    }
}
