using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace VTV.OpsConsole.RemoteManagement.APIServer.Authentication
{
    public class UserDBInitializer : StartupTasks.IStartupTask
    {
        private DataContext _context;
        private readonly IConfiguration _config;

        public UserDBInitializer(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                for(var i=0; i<50;i++)
                {
                    var username = "user" + i;
                    var password = _config["Usermanagement:"+username];
                    if (string.IsNullOrEmpty(password))
                        continue;
                    if (_context.Users.Any(x => x.Username == username))
                        continue;
                    byte[] passwordHash, passwordSalt;
                    Helper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
                    var user = new User
                    {
                        Id = i,
                        Username = username,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    };
                    _context.Users.Add(user);
                }
                _context.SaveChanges();
            });
        }
    }
}
