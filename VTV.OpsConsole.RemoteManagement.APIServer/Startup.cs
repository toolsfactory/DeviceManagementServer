using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VTV.OpsConsole.RemoteManagement.Interfaces;
using VTV.OpsConsole.RemoteManagement.Services;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;

namespace VTV.OpsConsole.RemoteManagement.APIServer
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
            services.AddSwaggerGen(c =>
            {
               c.SwaggerDoc("v1", new Info
               {
                   Title = "RemoteManagement Mockup",
                   Version = "v1",
                   Description = "Simple API allowing STB developers to send standard commands to the STB's",
                   TermsOfService = "Mockup only!",
                   Contact = new Contact
                   {
                       Name = "Tiago Vaz",
                       Email = "tiago.vaz@vodafone.com"
                   }
               });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddSingleton<IAWSClientsService, AWSClientsService>();
            services.AddSingleton<IDevicesService, DevicesService>();
            services.AddSingleton<IDeviceDetailsService, DeviceDetailsService>();
            services.AddSingleton<ICommandSendService, CommandSendService>();
            services.AddSingleton<ICommandsManagementService, CommandManagementService>();
            services.AddSingleton<IJobManagementService, JobManagementService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
           {
               c.SwaggerEndpoint("/swagger/v1/swagger.json", "RemoteManagement MockUp API v1");
               c.RoutePrefix = string.Empty;
           });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseDeveloperExceptionPage();
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
