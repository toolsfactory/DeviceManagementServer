using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DeviceManagementServer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace DeviceManagementServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            /*
            services.AddAuthentication("Bearer")
            .AddJwtBearer(options =>
            {
                options.Audience = Configuration["AWS:UserPoolClientId"];
                options.Authority = Configuration["Authentication:Cognito:BaseAddress"];
            });
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddOpenIdConnect(options =>
                {
                    options.ResponseType = "code";
                    options.MetadataAddress = Configuration["Authentication:Cognito:MetadataAddress"];
                    options.ClientId = Configuration["AWS:UserPoolClientId"];
                    options.ClientSecret = Configuration["AWS:UserPoolClientSecret"];
                });
            //*/
            //            services.AddCognitoIdentity();

            services.AddSingleton<IAWSClientsService, AWSClientsService>();
            services.AddSingleton<ICommandExecutionService, CommandExecutionService>();
            services.AddSingleton<ICommandManagementService, CommandManagementService>();
            services.AddSingleton<IJobManagementService, JobManagementService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
