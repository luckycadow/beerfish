using System.Collections.Generic;
using System.IO;

namespace Beerfish.Compilation
{
    public interface IAssetCompiler
    {
        AssetTypes Type { get; }
        Dictionary<string, string> CompileAssets(IEnumerable<DirectoryInfo> directories);
    }
}
