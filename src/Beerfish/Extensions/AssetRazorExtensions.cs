using System;
using Microsoft.AspNet.Mvc.Rendering;

namespace Beerfish.Extensions
{
    public static class AssetRazorExtensions
    {
        private static IAssetRegistry _assetRegistry;

        public static void SetAssetRegistry(IAssetRegistry registry)
        {
            _assetRegistry = registry;
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
                return new HtmlString(string.Empty);
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
