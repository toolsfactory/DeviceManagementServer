using System;

namespace VTV.OpsConsole.RemoteManagement.Exceptions
{
    public class CloudServiceGenericException : BaseException
    {
        public string CloudProviderName { get; private set; }
        public string ServiceName { get; private set; }

        public CloudServiceGenericException() : base("A cloud service is not working ea expected.")
        {
            CloudProviderName = "Unknown";
            ServiceName = "unknown";
        }

        public CloudServiceGenericException(string cloudProviderName, string serviceName, string constraint)
            : base($"The service '{cloudProviderName}:{serviceName}' caused an exception.")
        {
            CloudProviderName = cloudProviderName;
            ServiceName = serviceName;
        }

        public CloudServiceGenericException(string cloudProviderName, string serviceName, Exception innerException)
            : base($"The service '{cloudProviderName}:{serviceName}' caused an exception.", innerException)
        {
            CloudProviderName = cloudProviderName;
            ServiceName = serviceName;
        }
    }
}
