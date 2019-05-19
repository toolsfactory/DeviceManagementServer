using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace VTV.OpsConsole.RemoteManagement.APIServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateWebHostBuilder(args).Build().RunWithTasksAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                /*
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    if (!hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        config.AddSecretsManager(configurator: ops =>
                        {
                            // Replace __ tokens in the configuration key name
                            ops.KeyGenerator = (secret, name) => name.Replace("__", ":");
                        });
                    }
                })
                */
                .UseStartup<Startup>();
    }
}
