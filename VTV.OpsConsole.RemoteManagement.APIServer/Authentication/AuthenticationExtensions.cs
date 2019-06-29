using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace VTV.OpsConsole.RemoteManagement.APIServer.Authentication
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var key = Encoding.ASCII.GetBytes(configuration["Usermanagement:Secret"]);
            services.AddDbContext<DataContext>(x => x.UseInMemoryDatabase("TestDb"), ServiceLifetime.Singleton);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var userId = int.Parse(context.Principal.Identity.Name);
                        var user = userService.GetById(userId);
                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddAuthorization(x =>
            {
                x.AddPolicy("OpcoPolicy", policy => policy.Requirements.Add(new SameAuthorRequirement()));
            });
            services.AddScoped<IUserService, UserService>(); // instanciated once per request
            services.AddSingleton<IAuthorizationHandler, DocumentAuthorizationHandler>();
            return services;
        }
    }

    public class AuthorizationDetails
    {
        public string OpCoID { get; }
        public string Command { get; }

        public AuthorizationDetails(string opcoid, string command)
        {
            OpCoID = opcoid;
            Command = command;
        }
    }
    public class DocumentAuthorizationHandler : AuthorizationHandler<SameAuthorRequirement, AuthorizationDetails>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       SameAuthorRequirement requirement,
                                                       AuthorizationDetails resource)
        {
            var claim = context.User.Claims.Where(c => c.Type == "opco_" + resource.OpCoID && c.Value == "rm.device.all").FirstOrDefault();
            if (claim != null)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class SameAuthorRequirement : IAuthorizationRequirement { }

}
