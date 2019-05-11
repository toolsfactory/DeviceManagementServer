using Amazon.IoT;
using Amazon.IotData;
using Amazon.IoTJobsDataPlane;

namespace VTV.OpsConsole.RemoteManagement.Services
{
    public interface IAWSClientsService
    {
        AmazonIoTClient IoTClient { get; }
        AmazonIotDataClient IoTDataClient { get; }
        AmazonIoTJobsDataPlaneClient IoTJobClient { get; }
    }
}
