using Microsoft.AspNet.Http;
using System.IO;

namespace Beerfish
{
    public interface IAssetRegistry
    {
        Asset GetAsset(string name);
        Asset GetAsset(PathString path);
        void RegisterAsset(string name, string contents, AssetTypes type);
        void Clear();
    }
}
