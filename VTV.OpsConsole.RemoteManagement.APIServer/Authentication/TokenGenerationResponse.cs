using System;

namespace VTV.OpsConsole.RemoteManagement.APIServer.Authentication
{
    public class TokenGenerationResponse
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
