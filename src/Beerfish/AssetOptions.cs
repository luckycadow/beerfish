﻿using System.Collections.Generic;

namespace Beerfish
{
    public class AssetOptions
    {
        /// <summary>
        /// A list of directories to search for assets in.
        /// This is relative to the current working directory of the application.
        /// </summary>
        public IEnumerable<string> Assets { get; set; }

        /// <summary>
        /// A path to serve compiled assets from
        /// </summary>
        public string ServePath { get; set; }

        /// <summary>
        /// Append a hash to the file name
        /// </summary>
        public bool Fingerprint { get; set; }

        /// <summary>
        /// Watch for changes in the asset directories and recompile as needed
        /// </summary>
        public bool WatchFiles { get; set; }
       
        public bool Minify { get; set; }
    }
    
}
