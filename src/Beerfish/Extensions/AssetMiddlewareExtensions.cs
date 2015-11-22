using System.Linq;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;
using System.IO;
using Microsoft.Framework.OptionsModel;
using Beerfish.Compilation;
using System;
using System.Collections.Generic;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.Runtime;

namespace Beerfish.Extensions
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public static class AssetMiddlewareExtensions
    {

        private static ProcessorHandler _handler;

        public static IApplicationBuilder UseAssetHandler(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IApplicationEnvironment>();
            var options = app.ApplicationServices.GetRequiredService<IOptions<AssetOptions>>().Options;
            var registry = app.ApplicationServices.GetRequiredService<IAssetRegistry>();
            var compilers = app.ApplicationServices.GetRequiredServices<IAssetProcessor>();

            AssetRazorExtensions.SetupExtensions(registry, options.ServePath);
            
            var directories = options.Assets
                .Select(f => new DirectoryInfo(Path.Combine(env.ApplicationBasePath, f)));

            if (_handler == null)
            {
                _handler = new ProcessorHandler(compilers, registry);
            }

            _handler.ExecuteCompilers(directories);

            if (options.WatchFiles)
                _handler.RegisterFileWatcher(directories);
            
            return app.UseMiddleware<AssetMiddleware>();
        }

        public static IServiceCollection AddAssetManagement(this IServiceCollection services)
        {
            return services.AddAssetManagement(o =>
            {
                o.Assets = new List<string> { "/Content" };
                o.Fingerprint = true;
                o.ServePath = "/assets";
                o.WatchFiles = false;
            });
        }

        public static IServiceCollection AddAssetManagement(this IServiceCollection services, Action<AssetOptions> setupAction)
        {
            services
                .Configure(setupAction)
                .AddSingleton(typeof(IAssetRegistry), typeof(AssetRegistry))
                .AddTransient(typeof(IAssetProcessor), typeof(SimpleJavascriptProcessor));

            // Scss compilation will not work on mono because it's using the c++ library
            if (Type.GetType("Mono.Runtime") == null)
                services.AddTransient(typeof(IAssetProcessor), typeof(ScssProcessor));

            return services;
        }

    }
}
