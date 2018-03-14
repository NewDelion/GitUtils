using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitReaderLibrary.Model
{
    public class TreeEntry
    {
        public string Mode { get; set; }
        public string FileName { get; set; }
        public string SHA1 { get; set; }

        public bool IsDirectory => Mode == "040000";
        public bool IsBlob => !IsDirectory;
    }
}
