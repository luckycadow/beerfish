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

        private static CompilationHandler _handler;

        public static IApplicationBuilder UseAssetHandler(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IApplicationEnvironment>();
            var options = app.ApplicationServices.GetRequiredService<IOptions<AssetOptions>>().Options;
            var registry = app.ApplicationServices.GetRequiredService<IAssetRegistry>();
            var compilers = app.ApplicationServices.GetRequiredServices<IAssetCompiler>();

            AssetRazorExtensions.SetupExtensions(registry, options.ServePath);
            
            var directories = options.Assets
                .Select(f => new DirectoryInfo(Path.Combine(env.ApplicationBasePath, f)));

            if (_handler == null)
            {
                _handler = new CompilationHandler(compilers, registry);
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
                .AddTransient(typeof(IAssetCompiler), typeof(SimpleJavascriptCompiler));

            // Scss compilation will not work on mono because it's using the c++ library
            #if !__MonoCS__
                services.AddTransient(typeof(IAssetCompiler), typeof(ScssCompiler));
            #endif

            return services;
        }

    }
}
