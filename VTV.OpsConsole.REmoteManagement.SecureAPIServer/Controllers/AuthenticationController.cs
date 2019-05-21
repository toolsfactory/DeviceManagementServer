using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Mvc;

namespace VTV.OpsConsole.REmoteManagement.SecureAPIServer.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private const string _clientId = "129rjscubgcgbhp9geapn3ntt5";
        private readonly RegionEndpoint _region = RegionEndpoint.EUWest1;

        public class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }

        [HttpPost]
        [Route("api/register")]
        public async Task<ActionResult<string>> Register(User user)
        {
            var cognito = new AmazonCognitoIdentityProviderClient(_region);

            var request = new SignUpRequest
            {
                ClientId = _clientId,
                Password = user.Password,
                Username = user.Username
            };

            var emailAttribute = new AttributeType
            {
                Name = "email",
                Value = user.Email
            };
            request.UserAttributes.Add(emailAttribute);

            var response = await cognito.SignUpAsync(request);

            return Ok();
        }

        // https://cognito-idp.us-east-2.amazonaws.com/eu-west-1_c9Om71Z8v/.well-known/openid-configuration
        [HttpPost]
        [Route("api/signin")]
        public async Task<ActionResult<string>> SignIn(User user)
        {
            var cognito = new AmazonCognitoIdentityProviderClient(_region);

            var request = new AdminInitiateAuthRequest
            {
                UserPoolId = "eu-west-1_c9Om71Z8v",
                ClientId = _clientId,
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH
            };

            request.AuthParameters.Add("USERNAME", user.Username);
            request.AuthParameters.Add("PASSWORD", user.Password);

            var response = await cognito.AdminInitiateAuthAsync(request);

            return Ok(response.AuthenticationResult.IdToken);
        }
    }
}
