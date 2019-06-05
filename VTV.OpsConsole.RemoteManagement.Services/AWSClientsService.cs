using Amazon;
using Amazon.IoT;
using Amazon.IotData;
using Amazon.IoTJobsDataPlane;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;

namespace VTV.OpsConsole.RemoteManagement.Services
{
    public class AWSClientsService : IAWSClientsService
    {
        private readonly IConfiguration _config;

        public AmazonIoTClient IoTClient { get; private set; }

        public AmazonIotDataClient IoTDataClient { get; private set; }

        public AmazonIoTJobsDataPlaneClient IoTJobClient { get; private set; }

        public AmazonS3Client S3Client { get; private set; }

        public AWSClientsService(IConfiguration config) {
            _config = config;
            var awsCredentials = new BasicAWSCredentials(_config["AWS:ApiAccessKey"], _config["AWS:ApiSecretKey"]);
            IoTClient = new AmazonIoTClient(awsCredentials, RegionEndpoint.GetBySystemName(_config["AWS:Region"]));
            IoTDataClient = new AmazonIotDataClient("https://" + _config["AWS:IoTEndpoint"], awsCredentials);
            IoTJobClient = new AmazonIoTJobsDataPlaneClient("https://" + _config["AWS:IoTEndpoint"], awsCredentials);
            S3Client = new AmazonS3Client(awsCredentials, RegionEndpoint.GetBySystemName(_config["AWS:Region"]));
        }
    }
}
