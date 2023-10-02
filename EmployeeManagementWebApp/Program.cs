using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                    logging.AddNLog();
                })
                .UseStartup<Startup>();


        //public static IHostBuilder CreateWebHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //.ConfigureWebHostDefaults(webBuilder =>
        //{
        //    webBuilder.UseStartup<Startup>();
        //});


        //below is the code to use any configuration file instead appsettings.json by mentioning in configureAppconfiguration method
        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureAppConfiguration((hostingContext, config) =>
        //        {
        //            config.SetBasePath(Directory.GetCurrentDirectory());
        //            config.AddJsonFile("myconfig.json", optional: true, reloadOnChange: true);
        //        })
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });
    }
}
