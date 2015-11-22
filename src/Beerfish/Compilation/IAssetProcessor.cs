using System.Collections.Generic;
using System.IO;

namespace Beerfish.Compilation
{
    public interface IAssetProcessor
    {
        Dictionary<string, string> ProcessAssets(IEnumerable<DirectoryInfo> directories);
    }
}
