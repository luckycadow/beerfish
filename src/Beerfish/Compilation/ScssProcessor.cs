using System.Collections.Generic;
using System.Linq;
using System.IO;
using LibSassNet;
using Microsoft.Framework.OptionsModel;
using System.Text.RegularExpressions;

namespace Beerfish.Compilation
{
    internal class ScssProcessor : IAssetProcessor
    {
        private readonly ISassCompiler _compiler = new SassCompiler();
        private readonly AssetOptions _options;

        public ScssProcessor(IOptions<AssetOptions> options)
        {
            _options = options.Options;
        }

        private IEnumerable<FileInfo> FilterFiles(DirectoryInfo directory)
        {
           return directory.EnumerateFiles("*.scss", SearchOption.AllDirectories)
                .Where(f => !f.Name.StartsWith("_"));
        }

        public Dictionary<string, string> ProcessAssets(IEnumerable<DirectoryInfo> directories)
        {
            var outputStyle = _options.Minify ? OutputStyle.Compressed : OutputStyle.Expanded;

            var assets = new Dictionary<string, string>();

            foreach(var directory in directories)
            {
                var files = FilterFiles(directory);
                foreach(var file in files)
                {
                    var result = _compiler.CompileFile(file.FullName, outputStyle: outputStyle, includeSourceComments: false);
                    var name = Regex.Replace(file.Name, $"{file.Extension}$", ".css");
                    assets.Add(name, result.CSS);
                }
            }
            return assets;
        }
    }
}
