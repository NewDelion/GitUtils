using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitReaderLibrary.Model
{
    public class Tree : GitObject
    {
        public TreeEntry[] Entries { get; set; }

        public static Tree Load(string path)
        {
            var result = new Tree();
            using (var decompressed_ms = new MemoryStream(Decompress(path)))
            using (var content_ms = new MemoryStream(ReadHeader(decompressed_ms, result)))
            {
                if (result.Type != "tree")
                    throw new InvalidDataException("tree形式のファイルではありません");

                var list = new List<TreeEntry>();
                while (content_ms.Position < content_ms.Length)
                {
                    list.Add(new TreeEntry
                    {
                        Mode = int.Parse(content_ms.ReadToSpace()).ToString("000000"),
                        FileName = content_ms.ReadToNull(),
                        SHA1 = content_ms.Read(20).ToHex()
                    });
                }
                result.Entries = list.ToArray();
            }
            return result;
        }
    }
}
