using Amazon;
using Amazon.IoT;
using Amazon.IotData;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;

namespace DeviceManagementServer.Services
{
    public interface IAWSClientsService
    {
        AmazonIoTClient IoTClient { get; }
        AmazonIotDataClient IoTDataClient { get; }
    }

    public class AWSClientsService : IAWSClientsService
    {
        private readonly IConfiguration _config;

        public AmazonIoTClient IoTClient { get; private set; }

        public AmazonIotDataClient IoTDataClient { get; private set; }

        public AWSClientsService(IConfiguration config) {
            this._config = config;
            var awsCredentials = new BasicAWSCredentials(_config["AWS:ApiAccessKey"], _config["AWS:ApiSecretKey"]);
            IoTClient = new AmazonIoTClient(awsCredentials, RegionEndpoint.GetBySystemName(_config["AWS:Region"]));
            IoTDataClient = new AmazonIotDataClient("https://" + _config["AWS:IoTEndpoint"], awsCredentials);
        }
    }
}
