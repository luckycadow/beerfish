using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Beerfish.Compilation
{
    internal class CompilationHandler
    {
        private readonly IEnumerable<IAssetCompiler> _compilers;
        private readonly IAssetRegistry _registry;
        private IEnumerable<DirectoryInfo> _watchedDirectories;

        public CompilationHandler(IEnumerable<IAssetCompiler> compilers, IAssetRegistry registry)
        {
            _compilers = compilers;
            _registry = registry;
        }
        
        public void ExecuteCompilers(IEnumerable<DirectoryInfo> directories)
        {
            if (!directories.Any())
                return;

            Parallel.ForEach(_compilers, c => {
                var assetContents = c.CompileAssets(directories);
                foreach(var asset in assetContents)
                {
                    _registry.RegisterAsset(asset.Key, asset.Value, c.Type);
                }
            });
        }

        public void RegisterFileWatcher(IEnumerable<DirectoryInfo> directories)
        {
            _watchedDirectories = directories;
            foreach(var dir in directories)
            {
                var watcher = new FileSystemWatcher(dir.FullName);
                watcher.IncludeSubdirectories = true;
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Changed += OnChange;
            }
        }

        private void OnChange(object sender, FileSystemEventArgs e)
        {
            if (_watchedDirectories == null) return;

            // If the directory that changed is in our list of watched directories then 
            // run the compilers against it.  If it is not (and therefore a child) run the 
            // compilers against all of our watched directores because we don't know if 
            // compilation is safe to do directly on a child directory.
            if (_watchedDirectories.Any(d => d.FullName == e.FullPath))
            {
                ExecuteCompilers(new List<DirectoryInfo>() { new DirectoryInfo(e.FullPath) });
            }
            else
            {
                ExecuteCompilers(_watchedDirectories);
            }
        }
        
    }
}
