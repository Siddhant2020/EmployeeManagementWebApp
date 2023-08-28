using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementWebApp
{
    public class Startup
    {
        public IConfiguration _configuration { get; }

        //Constructor injection to inject IConfiguration service
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            MvcOptions options = new MvcOptions();
            options.EnableEndpointRouting = false;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            //if (env.IsEnvironment("UAT")) // check for the custom created Environment
            if (env.IsDevelopment())
            {
                //DeveloperExceptionPageOptions developerExceptionPageOptions = new DeveloperExceptionPageOptions
                //{
                //    SourceCodeLineCount = 1,
                //};
                //app.UseDeveloperExceptionPage(developerExceptionPageOptions);
                app.UseDeveloperExceptionPage();
            }
            //else if (env.IsStaging() || env.IsProduction() || env.IsEnvironment("UAT"))
            //{
            //    app.UseExceptionHandler("/Error");
            //}


            //DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            //defaultFilesOptions.DefaultFileNames.Clear();
            //defaultFilesOptions.DefaultFileNames.Add("foo.html"); //set foo.html as default page
            //app.UseDefaultFiles(defaultFilesOptions); //UseDefaultFiles Middleware is used to set any default web page apart from the conventional approach(i.e. default.html, default.htm) by using the DefaultFilesOptions object
            //app.UseStaticFiles(); //To serve static files UseStaticFiles middleware is required, files which is placed inside wwwroot folder

            //FileServer Middleware is the replacement of both UseDefaultFiles, UseStaticFiles Middlewares
            //FileServerOptions fileServerOptions = new FileServerOptions();
            //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Clear();
            //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Add("foo.html");
            //app.UseFileServer(fileServerOptions);

            //app.UseMvc();

            //app.Run(async (context) =>
            //{
            //    //throw new Exception("Some error occured processing the request");
            //    //await context.Response.WriteAsync("Hello World!");
            //    await context.Response.WriteAsync("Hosting Environment : " + env.EnvironmentName);
            //});


            //app.Use(async (context, next) => {
            //    logger.LogInformation("MW1 : Incoming Request");
            //    //await context.Response.WriteAsync("Hello World from 1st Middleware \n");
            //    await next();
            //    logger.LogInformation("MW1 : Outgoing Response");
            //});

            //app.Use(async (context, next) =>
            //{
            //    logger.LogInformation("MW2 : Incoming Request");
            //    await context.Response.WriteAsync("Hello World from 1st Middleware \n");
            //    await next();
            //    logger.LogInformation("MW2 : Outgoing Response");
            //});

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World from 3rd Middleware");
            //    //logger.LogInformation("MW3: Request handled and response produced");
            //    //await context.Response.WriteAsync(System.Diagnostics.Process.GetCurrentProcess().ProcessName);    //to get the server process name the application is running upon
            //    //await context.Response.WriteAsync(_configuration["MyKey"]);   //to get the value from configuration file
            //});
        }
    }
}
