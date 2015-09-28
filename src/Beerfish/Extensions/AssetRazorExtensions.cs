using System;
using Microsoft.AspNet.Mvc.Rendering;

namespace Beerfish.Extensions
{
    public static class AssetRazorExtensions
    {
        private static IAssetRegistry _assetRegistry;
        private static string _servePath;


        public static void SetupExtensions(IAssetRegistry registry, string servePath)
        {
            _assetRegistry = registry;
            _servePath = servePath.TrimEnd("/".ToCharArray());
        }

        /// <summary>
        /// Returns a script or link tag as appropriate referencing the asset specified by name.
        /// If the asset does not exist no tag will be written.
        /// </summary>
        /// <param name="helper">HtmlHelper</param>
        /// <param name="name">Asset name</param>
        /// <returns></returns>
        public static HtmlString Asset(this IHtmlHelper helper, string name)
        {
            if (_assetRegistry == null)
            {
                throw new InvalidOperationException("Asset helpers cannot be used before an AssetRegistry has been set.");
            }

            var asset = _assetRegistry.GetAsset(name);

            if (asset == null)
            {
                // If we're asked to write a tag for an asset that doesn't exist, just use the name
                // prefixed by the serve path so the request has the option to fall through to a
                // file handler.
                if (name.EndsWith(".css"))
                {
                    return new HtmlString($"<link rel=\"stylesheet\" href=\"{_servePath}/{name}\">");
                }
                else
                {
                    return new HtmlString($"<script src=\"{_servePath}/{name}\"></script>");
                }
            }
            else if (asset.Type == AssetTypes.Js)
            {
                return new HtmlString($"<script src=\"{asset.Path}\"></script>");
            }
            else if (asset.Type == AssetTypes.Css)
            {
                return new HtmlString($"<link rel=\"stylesheet\" href=\"{asset.Path}\">");
            }

            return new HtmlString(string.Empty);
        }
    }
}
