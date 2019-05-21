using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace VTV.OpsConsole.RemoteManagement.APIServer.Authentication
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
        private readonly DataContext _context;

        public UserService(DataContext context, IConfiguration configuration)
        {
            _config = configuration;
            _context = context;
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!Helper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public TokenGenerationResponse GenerateToken(User user)
        {
            int expiresSeconds = 60 * 60 * 24 * 7;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Usermanagement:Secret"]);
            var parsed = int.TryParse(_config["Usermanagement:TokenExpiresSeconds"], out var expiresSecondsConfig);

            var expires = DateTime.UtcNow.AddSeconds((expiresSecondsConfig>0) ? expiresSecondsConfig : expiresSeconds);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return new TokenGenerationResponse
            {
                Token = tokenString,
                Expires = expires
            };
        }
    }
}
