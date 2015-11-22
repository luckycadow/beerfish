using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Beerfish.Compilation
{
    internal class ProcessorHandler
    {
        private readonly IEnumerable<IAssetProcessor> _compilers;
        private readonly IAssetRegistry _registry;
        private IEnumerable<DirectoryInfo> _watchedDirectories;

        public ProcessorHandler(IEnumerable<IAssetProcessor> compilers, IAssetRegistry registry)
        {
            _compilers = compilers;
            _registry = registry;
        }
        
        public void ExecuteCompilers(IEnumerable<DirectoryInfo> directories)
        {
            if (!directories.Any())
                return;

            Parallel.ForEach(_compilers, c => {
                try
                {
                    var assetContents = c.ProcessAssets(directories);
                    foreach (var asset in assetContents)
                    {
                        _registry.RegisterAsset(asset.Key, asset.Value);
                    }
                }
                catch (IOException e)
                {
                    Console.Error.WriteLine(e.ToString());
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
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
                watcher.Changed += new FileSystemEventHandler(OnChange);
                watcher.Created += new FileSystemEventHandler(OnChange);
                watcher.Deleted += new FileSystemEventHandler(OnChange);
                watcher.EnableRaisingEvents = true;
            }
        }

        private void OnChange(object sender, FileSystemEventArgs e)
        {
            var watcher = sender as FileSystemWatcher;
            // Try to filter temp files out here
            if (watcher == null || e.Name.EndsWith("TMP") || e.Name.StartsWith("~") || e.Name.EndsWith("~")) return;
            ExecuteCompilers(new List<DirectoryInfo>() { new DirectoryInfo(watcher.Path) });
        }
        
    }
}
