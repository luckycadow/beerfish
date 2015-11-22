using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System.Threading.Tasks;
using Microsoft.Framework.OptionsModel;
using System.IO;
using System.Text;
using Microsoft.Framework.Runtime;
using System;
using Microsoft.AspNet.StaticFiles;

namespace Beerfish
{
    internal class AssetMiddleware
    {
        private readonly IAssetRegistry _registry;
        private readonly RequestDelegate _next;
        private readonly AssetOptions _options;
        private readonly PathString _assetPath;
        private readonly string _baseDirectory;

        private readonly FileExtensionContentTypeProvider _contentTypeProvider = new FileExtensionContentTypeProvider();

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
                    context.Response.ContentType = GetContentType(asset.Path);
                    context.Response.ContentLength = Encoding.ASCII.GetByteCount(asset.Contents);
                    await context.Response.WriteAsync(asset.Contents);
                    return;
                }

                var physicalPath = Path.Combine(_baseDirectory, context.Request.Path.ToString().TrimStart("/".ToCharArray()));
                if (File.Exists(physicalPath))
                {
                    SetCacheHeaders(context.Response);
                    await context.Response.SendFileAsync(physicalPath);
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

        private string GetContentType(string path)
        {
            string contentType = "application/octet-stream";
            _contentTypeProvider.TryGetContentType(path, out contentType);
            return contentType;
        }

    }
}
