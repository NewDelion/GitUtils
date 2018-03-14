using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitReaderLibrary.Model
{
    public class IndexEntry
    {
        public uint LastMetadataChangedSecond { get; set; }
        public uint LastMetadataChangedNanosecondFraction { get; set; }

        public uint LastFileChangedSecond { get; set; }
        public uint LastFileChangedNanosecondFraction { get; set; }

        public uint Dev { get; set; }

        public uint Inode { get; set; }

        public uint Mode { get; set; }

        public uint Uid { get; set; }

        public uint Gid { get; set; }

        public uint FileSize { get; set; }

        public string SHA1 { get; set; }

        /// <summary>
        /// null終端とパディングは含みません。
        /// </summary>
        public ushort FilePathLength { get; set; }

        public string FilePath { get; set; }
    }
}
