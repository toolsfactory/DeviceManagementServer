using System;

namespace VTV.OpsConsole.RemoteManagement.Exceptions
{
    public class ResourceAlreadyExistsException : BaseException
    {
        public string ResourceName { get; private set; }
        public string ResourceType { get; private set; }

        public ResourceAlreadyExistsException() : base("A resource with same name already exists.")
        {
            ResourceName = "Unknown";
            ResourceType = "Unknown";
        }

        public ResourceAlreadyExistsException(string resourceType, string resourceName) : base($"The resource '{resourceName}' of type '{resourceType}' cannot be created as another one with same name already exists.")
        {
            ResourceName = resourceName;
            ResourceType = resourceType;
        }

        public ResourceAlreadyExistsException(string resourceType, string resourceName, Exception innerException) : base($"The resource '{resourceName}' of type '{resourceType}' cannot be created as another one with same name already exists.", innerException)
        {
            ResourceName = resourceName;
            ResourceType = resourceType;
        }
    }
}
