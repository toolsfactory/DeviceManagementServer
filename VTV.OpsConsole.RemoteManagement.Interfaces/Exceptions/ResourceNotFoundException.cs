using System;

namespace VTV.OpsConsole.RemoteManagement.Exceptions
{
    public class ResourceNotFoundException : BaseException
    {
        public string ResourceName { get; private set; }
        public string ResourceType { get; private set; }

        public ResourceNotFoundException() : base("A resource was not found.")
        {
            ResourceName = "Unknown";
            ResourceType = "Unknown";
        }

        public ResourceNotFoundException(string resourceType, string resourceName) : base($"The resource '{resourceName}' of type '{resourceType}' was not found")
        {
            ResourceName = resourceName;
            ResourceType = resourceType;
        }

        public ResourceNotFoundException(string resourceType, string resourceName, Exception innerException) : base($"The resource '{resourceName}' of type '{resourceType}' was not found", innerException)
        {
            ResourceName = resourceName;
            ResourceType = resourceType;
        }
    }
}
