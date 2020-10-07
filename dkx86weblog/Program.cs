using dkx86weblog.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace dkx86weblog
{
    public class Program
    {

        private static NLog.Logger ConfigLogger()
        {
            var pathToBin = Process.GetCurrentProcess().MainModule.FileName;
            var pathToContentRoot = Path.GetDirectoryName(pathToBin);
            NLog.LogManager.LogFactory.SetCandidateConfigFilePaths(new List<string> { $"{Path.Combine(pathToContentRoot, "nlog.config")}" });
            return NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            //return NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger(); 
        }

        public static void Main(string[] args)
        {
            var logger = ConfigLogger();
            IHost host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                logger.Info("--> init main");
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();
                    logger.Info("--> DB migrated");
                    host.Run();
                }
                catch (Exception ex)
                {
                    // var logger = services.GetRequiredService<ILogger<Program>>();
                    //logger.LogError(ex, "An error occurred while migrating the DB.");
                    logger.Error(ex, "An error occurred while migrating the DB.");
                    throw;
                }
                finally
                {
                    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                    NLog.LogManager.Shutdown();
                }
            }

        }



        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel((context, options) =>
                    {
                        // Handle requests up to 512 MB
                        options.Limits.MaxRequestBodySize = 536_870_912;
                    })
                    .UseStartup<Startup>();
                }).ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                }).UseNLog();  // NLog: Setup NLog for Dependency injection
    }
}
