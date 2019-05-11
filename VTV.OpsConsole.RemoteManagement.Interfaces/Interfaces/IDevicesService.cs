using System;
using System.Threading.Tasks;

namespace VTV.OpsConsole.RemoteManagement.Interfaces
{
    public interface IDevicesService
    {
        Task<Models.DevicesListModel> GetDeviceListAsync();
    }
}
