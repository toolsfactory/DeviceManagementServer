﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DeviceManagementServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    if (!hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        // Don't add AWS secrets in local environment
                        config.AddSecretsManager(configurator: ops =>
                        {
                            // Replace __ tokens in the configuration key name
                            ops.KeyGenerator = (secret, name) => name.Replace("__", ":");
                        });
                    }
                })
                .UseStartup<Startup>();
    }
}