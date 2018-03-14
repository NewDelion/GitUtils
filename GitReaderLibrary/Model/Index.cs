using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GitReaderLibrary.Model
{
    public class Index
    {
        public const uint Signature = 0x44495243;

        public uint Version { get; set; }
        public uint EntryCount { get; set; }
        public IndexEntry[] Entries { get; set; }

        public static Index Load(string path)
        {
            var result = new Index();
            using(var fs = new FileStream(path, FileMode.Open))
            {
                if(fs.ReadUInt32BE() != Signature)
                    throw new InvalidDataException("シグネチャがindexフォーマットではありません");
                result.Version = fs.ReadUInt32BE();
                result.EntryCount = fs.ReadUInt32BE();
                result.Entries = new IndexEntry[result.EntryCount];
                for(int i = 0; i < result.EntryCount; ++i)
                {
                    var entry = new IndexEntry();
                    entry.LastMetadataChangedSecond = fs.ReadUInt32BE();
                    entry.LastMetadataChangedNanosecondFraction = fs.ReadUInt32BE();
                    entry.LastFileChangedSecond = fs.ReadUInt32BE();
                    entry.LastFileChangedNanosecondFraction = fs.ReadUInt32BE();
                    entry.Dev = fs.ReadUInt32BE();
                    entry.Inode = fs.ReadUInt32BE();
                    entry.Mode = fs.ReadUInt32BE();
                    entry.Uid = fs.ReadUInt32BE();
                    entry.Gid = fs.ReadUInt32BE();
                    entry.FileSize = fs.ReadUInt32BE();
                    entry.SHA1 = fs.Read(20).ToHex();
                    entry.FilePathLength = fs.ReadUInt16BE();//ここまで62byte
                    entry.FilePath = Encoding.UTF8.GetString(fs.Read(entry.FilePathLength));

                    int pad_size = 1;
                    while ((62 + entry.FilePathLength + pad_size) % 8 > 0) ++pad_size;
                    fs.Seek(pad_size, SeekOrigin.Current);

                    result.Entries[i] = entry;
                }
            }
            return result;
        }
    }
}
