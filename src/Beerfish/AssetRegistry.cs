using Microsoft.AspNet.Http;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Beerfish
{
    public class AssetRegistry : IAssetRegistry
    {
        private readonly Dictionary<PathString, Asset> _assetLookup = new Dictionary<PathString, Asset>();
        protected readonly SHA1 _sha = SHA1.Create();

        protected AssetOptions _options;

        public AssetRegistry(IOptions<AssetOptions> options)
        {
            _options = options.Options;
        }

        private string GetHashString(string contents)
        {
            var textBytes = Encoding.UTF8.GetBytes(contents);
            var hashBytes = _sha.ComputeHash(textBytes);
            var sb = new StringBuilder();
            foreach (var b in hashBytes)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        public void RegisterAsset(string name, string contents, AssetTypes type)
        {
            var extension = name.Split('.').ToList().Last();
            var baseName = name.Replace($".{extension}", string.Empty);
            
            if (_options.Fingerprint)
            {
                var hash = GetHashString(contents);
                name = $"{baseName}-{hash}.{type.ToString()}".ToLower();
            }
            else
            {
                name = $"{baseName}.{type.ToString()}".ToLower();
            }
            
            var asset = new Asset
            {
                Type = type,
                SimpleName = $"{baseName}.{type.ToString()}".ToLower(),
                Path = $"{_options.ServePath}/{name}",
                Contents = contents
            };

            var pathString = new PathString(asset.Path);

            if (_assetLookup.ContainsKey(pathString))
            {
                _assetLookup[pathString] = asset;
            }
            else
            {
                _assetLookup.Add(pathString, asset);
            }
        }

        public Asset GetAsset(PathString path)
        {
            Asset asset;
            _assetLookup.TryGetValue(path, out asset);
            return asset;
        }

        public Asset GetAsset(string name)
        {
            return _assetLookup.Values.SingleOrDefault(a => string.Equals(a.SimpleName, name, StringComparison.OrdinalIgnoreCase));
        }
        
        public void Clear()
        {
            _assetLookup.Clear();
        }
    }
}
