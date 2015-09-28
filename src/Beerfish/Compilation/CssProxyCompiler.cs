using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Beerfish.Compilation
{
    public class CssProxyCompiler : IAssetCompiler
    {
        private readonly string _servePath;

        public CssProxyCompiler(IOptions<AssetOptions> options, IApplicationEnvironment env)
        {
            _servePath = Path.Combine(env.ApplicationBasePath, 
                options.Options.ServePath.TrimStart("/".ToCharArray()));
        }

        public AssetTypes Type
        {
            get
            {
                return AssetTypes.Css;
            }
        }

        public Dictionary<string, string> CompileAssets(IEnumerable<DirectoryInfo> directories)
        {
            // This compiler simply looks for already compiled css in the asset directory and registers it.
            // This serves as a temporary workaround for not being able compile scss on mono by allowing another
            // process to do the work and using the result in the handler.
            return new DirectoryInfo(_servePath)
                .EnumerateFiles("*.css", SearchOption.TopDirectoryOnly)
                .ToDictionary(f => f.Name, f => File.ReadAllText(f.FullName));
        }
    }
}
