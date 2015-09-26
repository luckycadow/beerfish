using System.IO;

namespace Beerfish
{
    public class Asset
    {
        public string Contents { get; set; }
        public AssetTypes Type { get; set; }
        public string SimpleName { get; set; }
        public string Path { get; set; }
        public string ContentType
        {
            get
            {
                return Type == AssetTypes.Css ? "text/css" : "application/javascript";
            }
        }
    }
}
