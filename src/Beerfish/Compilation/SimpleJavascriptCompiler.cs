using DouglasCrockford.JsMin;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Beerfish.Compilation
{
    public class SimpleJavascriptCompiler : IAssetCompiler
    {
        private JsMinifier _minifier = new JsMinifier();
        private AssetOptions _options;

        public SimpleJavascriptCompiler(IOptions<AssetOptions> options)
        {
            _options = options.Options;
        }

        public AssetTypes Type
        {
            get { return AssetTypes.Js; }
        }

        public Dictionary<string, string> CompileAssets(IEnumerable<DirectoryInfo> directories)
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
                {
                    contents = _minifier.Minify(contents);
                }

                assets.Add(directory.Name, contents);
            }
            return assets;
        }
        
    }
}
