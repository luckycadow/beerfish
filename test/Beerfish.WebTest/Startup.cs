using System.Collections.Generic;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Runtime;
using Beerfish.Extensions;

namespace Beerfish.WebTest
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            // Setup configuration sources.

            var builder = new Configuration()
                .AddJsonFile("config.json");
            
            builder.AddEnvironmentVariables();
            Configuration = builder;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddAssetManagement(o => {
                o.Assets = new List<string> { "stuff/sass", "stuff/js" };
                o.Fingerprint = true;
                o.ServePath = "/asset";
                o.WatchFiles = false;
                o.Minify = true;
            });
            services.AddMvc();
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.MinimumLevel = LogLevel.Information;
            loggerFactory.AddConsole();
            
            app.UseErrorPage(ErrorPageOptions.ShowAll);

            app.UseAssetHandler();

            // Add static files to the request pipeline.
            app.UseStaticFiles();

            app.UseMvc(route =>
            {
                route.MapRoute("Default", "", new { controller = "Home", action = "Index" });
            });

            app.UseWelcomePage();
        }
    }
}
