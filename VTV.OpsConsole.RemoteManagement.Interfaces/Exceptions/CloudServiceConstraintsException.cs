using System;

namespace VTV.OpsConsole.RemoteManagement.Exceptions
{
    public class CloudServiceConstraintsException : BaseException
    {
        public string CloudProviderName { get; private set; }
        public string Constraint { get; private set; }
        public string ServiceName { get; private set; }

        public CloudServiceConstraintsException() : base("A cloud service constraint caused an exception.")
        {
            CloudProviderName = "Unknown";
            Constraint = "Unknown";
            ServiceName = "unknown";
        }

        public CloudServiceConstraintsException(string cloudProviderName, string serviceName, string constraint) 
            : base($"The service '{cloudProviderName}:{serviceName}' caused an exception because of constraint '{constraint}'.")
        {
            CloudProviderName = cloudProviderName;
            Constraint = constraint;
            ServiceName = serviceName;
        }

        public CloudServiceConstraintsException(string cloudProviderName, string serviceName, string constraint, Exception innerException) 
            : base($"The service '{cloudProviderName}:{serviceName}' caused an exception because of constraint '{constraint}'.", innerException)
        {
            CloudProviderName = cloudProviderName;
            Constraint = constraint;
            ServiceName = serviceName;
        }
    }
}
