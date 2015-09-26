using System.Collections.Generic;
using System.Linq;
using System.IO;
using LibSassNet;
using Microsoft.Framework.OptionsModel;

namespace Beerfish.Compilation
{
    internal class ScssCompiler : IAssetCompiler
    {
        private readonly ISassCompiler _compiler = new LibSassNet.SassCompiler();
        private readonly AssetOptions _options;

        public ScssCompiler(IOptions<AssetOptions> options)
        {
            _options = options.Options;
        }

        public AssetTypes Type {
            get { return AssetTypes.Css; }
        }

        private IEnumerable<FileInfo> FilterFiles(DirectoryInfo directory)
        {
           return directory.EnumerateFiles("*.scss", SearchOption.AllDirectories)
                .Where(f => !f.Name.StartsWith("_"));
        }

        public Dictionary<string, string> CompileAssets(IEnumerable<DirectoryInfo> directories)
        {
            var outputStyle = _options.Minify ? OutputStyle.Compressed : OutputStyle.Expanded;

            var assets = new Dictionary<string, string>();

            foreach(var directory in directories)
            {
                var files = FilterFiles(directory);
                foreach(var file in files)
                {
                    var result = _compiler.CompileFile(file.FullName, 
                        outputStyle: outputStyle, includeSourceComments: false);
                    assets.Add(file.Name, result.CSS);
                }
            }
            return assets;
        }
    }
}
