using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System.Threading.Tasks;
using Microsoft.Framework.OptionsModel;
using System.IO;
using System.Text;
using Microsoft.Framework.Runtime;
using System;

namespace Beerfish
{
    internal class AssetMiddleware
    {
        private readonly IAssetRegistry _registry;
        private readonly RequestDelegate _next;
        private readonly AssetOptions _options;
        private readonly PathString _assetPath;
        private readonly string _baseDirectory;

        public AssetMiddleware(RequestDelegate next, IOptions<AssetOptions> options, IAssetRegistry registry, IApplicationEnvironment env)
        {
            _next = next;
            _options = options.Options;
            _assetPath = new PathString(options.Options.ServePath);
            _registry = registry;
            _baseDirectory = env.ApplicationBasePath;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments(_assetPath))
            {
                Asset asset = _registry.GetAsset(context.Request.Path);
                if (asset != null)
                {
                    SetCacheHeaders(context.Response);
                    context.Response.ContentType = asset.ContentType;
                    context.Response.ContentLength = Encoding.ASCII.GetByteCount(asset.Contents);
                    await context.Response.WriteAsync(asset.Contents);
                    return;
                }

                var physicalPath = Path.Combine(_baseDirectory, context.Request.Path.ToString().TrimStart("/".ToCharArray()));
                if (File.Exists(physicalPath))
                {
                    var contents = File.ReadAllText(physicalPath);
                    SetCacheHeaders(context.Response);
                    context.Response.ContentType = physicalPath.EndsWith(".css") ? "text/css" : "application/javascript";
                    context.Response.ContentLength = Encoding.ASCII.GetByteCount(contents);
                    await context.Response.WriteAsync(contents);
                    return;
                }
            }

            await _next(context);
        }
        
        private void SetCacheHeaders(HttpResponse response)
        {
            if (_options.CacheLength != null)
            {
                var expires = DateTime.Now.AddSeconds(_options.CacheLength.TotalSeconds);
                response.Headers.SetCommaSeparatedValues("Cache-Control", "public", $"max-age={_options.CacheLength.TotalSeconds}");
                response.Headers.Set("Expires", expires.ToUniversalTime().ToString("R"));
            }
        }

    }
}
