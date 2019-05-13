using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace VTV.OpsConsole.RemoteManagement.Exceptions
{
    public abstract class BaseException : Exception
    {
        public BaseException()
        {
        }

        public BaseException(string message) : base(message)
        {
        }

        public BaseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
