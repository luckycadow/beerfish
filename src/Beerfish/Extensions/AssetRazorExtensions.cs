using System;
using Microsoft.AspNet.Mvc.Rendering;
using System.IO;

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
        /// Returns the contents of the asset specified as a string.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Asset(this IHtmlHelper helper, string name)
        {
            if (_assetRegistry == null)
                throw new InvalidOperationException("Asset helpers cannot be used before an AssetRegistry has been set.");

            var asset = _assetRegistry.GetAsset(name);
            
            return asset?.Contents;
        }

        /// <summary>
        /// Return the path of the asset specified.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string AssetPath(this IHtmlHelper helper, string name)
        {
            if (_assetRegistry == null)
                throw new InvalidOperationException("Asset helpers cannot be used before an AssetRegistry has been set.");

            var asset = _assetRegistry.GetAsset(name);

            if (asset == null)
                return $"{_servePath}/{name}";

            return asset.Path;
        }

        /// <summary>
        /// Render a link tag to include the asset specified as a stylesheet.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static HtmlString Css(this IHtmlHelper helper, string name)
        {
            return new HtmlString($"<link rel=\"stylesheet\" href=\"{helper.AssetPath(name)}\">");
        }

        /// <summary>
        /// Render a script tag to include the asset specified as javascript.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static HtmlString Js(this IHtmlHelper helper, string name)
        {
            return new HtmlString($"<script src=\"{helper.AssetPath(name)}\"></script>");
        }
    }
}
