using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GitReaderLibrary.Model
{
    public class Blob : GitObject
    {
        public byte[] Content { get; set; }

        public static Blob Load(string path)
        {
            var result = new Blob();
            using (var decompressed_ms = new MemoryStream(Decompress(path)))
            {
                result.Content = ReadHeader(decompressed_ms, result);
                if(result.Type != "blob")
                    throw new InvalidDataException("blob形式のファイルではありません");
            }
            return result;
        }
    }
}
