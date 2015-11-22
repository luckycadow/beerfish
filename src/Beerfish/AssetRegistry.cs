using Microsoft.AspNet.Http;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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

        public void RegisterAsset(string name, string contents)
        {
            var extension = Path.GetExtension(name);
            var baseName = Regex.Replace(name, $"{extension}$", string.Empty);
            
            if (_options.Fingerprint)
            {
                var hash = GetHashString(contents);
                name = $"{baseName}-{hash}{extension}".ToLower();
            }
            
            var asset = new Asset
            {
                SimpleName = $"{baseName}{extension}".ToLower(),
                Path = $"{_options.ServePath}/{name.ToLower()}",
                Contents = contents
            };

            var pathString = new PathString(asset.Path);

            var existingAsset = _assetLookup.Values.FirstOrDefault(a => a.SimpleName == asset.SimpleName);
            if (!string.IsNullOrEmpty(existingAsset?.Path))
            {
                _assetLookup.Remove(new PathString(existingAsset.Path));
            }
            
            _assetLookup.Add(pathString, asset);
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
