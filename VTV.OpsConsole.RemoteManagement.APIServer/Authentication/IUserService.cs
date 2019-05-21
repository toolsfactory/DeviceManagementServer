using System;

namespace VTV.OpsConsole.RemoteManagement.APIServer.Authentication
{
    public interface IUserService
    {
        User Authenticate(string unsername, string password);
        User GetById(int id);
        TokenGenerationResponse GenerateToken(User user);
    }
}
