using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System.Threading.Tasks;
using Microsoft.Framework.OptionsModel;
using System.IO;
using System.Text;

namespace Beerfish
{
    internal class AssetMiddleware
    {
        private readonly IAssetRegistry _registry;
        private readonly RequestDelegate _next;
        private readonly AssetOptions _options;
        private readonly PathString _assetPath;

        public AssetMiddleware(RequestDelegate next, IOptions<AssetOptions> options, IAssetRegistry registry)
        {
            _next = next;
            _options = options.Options;
            _assetPath = new PathString(options.Options.ServePath);
            _registry = registry;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments(_assetPath))
            {
                Asset asset = _registry.GetAsset(context.Request.Path);
                if (asset != null)
                {
                    context.Response.ContentType = asset.ContentType;
                    context.Response.ContentLength = Encoding.ASCII.GetByteCount(asset.Contents);
                    await context.Response.WriteAsync(asset.Contents);
                    return;
                }
            }

            await _next(context);
        }
        
    }
}
