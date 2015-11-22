using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yahoo.Yui.Compressor;

namespace Beerfish.Compilation
{
    public class SimpleJavascriptProcessor : IAssetProcessor
    {
        private IJavaScriptCompressor _yuiCompressor = new JavaScriptCompressor();
        private AssetOptions _options;

        public SimpleJavascriptProcessor(IOptions<AssetOptions> options)
        {
            _options = options.Options;
        }

        public Dictionary<string, string> ProcessAssets(IEnumerable<DirectoryInfo> directories)
        {
            var assets = new Dictionary<string, string>();
            foreach (var directory in directories)
            {
                var sources = directory.EnumerateFiles("*.js", SearchOption.AllDirectories)
                    .Select(f => File.ReadAllText(f.FullName));

                if (!sources.Any())
                    continue;

                var contents = string.Join(Environment.NewLine, sources);

                if (_options.Minify)
                    contents = _yuiCompressor.Compress(contents);

                assets.Add($"{directory.Name}.js", contents);
            }
            return assets;
        }
        
    }
}
