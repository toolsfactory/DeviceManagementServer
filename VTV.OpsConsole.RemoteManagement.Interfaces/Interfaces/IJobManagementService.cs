using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using VTV.OpsConsole.RemoteManagement.Models;

namespace VTV.OpsConsole.RemoteManagement.Interfaces
{
    public interface IJobManagementService
    {
        Task<JobDetailsModel> GetJobDetailsAsync(string jobId);
        Task<JObject> GetJobDocumentAsync(string jobId);
        Task DeleteJobAsync(string jobId);
    }
}
