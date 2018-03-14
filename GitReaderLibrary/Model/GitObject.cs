using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zlib;

namespace GitReaderLibrary.Model
{
    public abstract class GitObject
    {
        public string Type { get; set; }
        public int ContentLength { get; set; }

        protected static byte[] Decompress(string path)
        {
            using (var ms = new MemoryStream())
            using (var in_fs = new FileStream(path, FileMode.Open))
            using (var zlib = new ZOutputStream(ms))
            {
                int len;
                var buf = new byte[512];
                while ((len = in_fs.Read(buf, 0, buf.Length)) > 0)
                    zlib.Write(buf, 0, len);
                zlib.Flush();
                zlib.finish();
                return ms.ToArray();
            }
        }

        protected static byte[] ReadHeader(MemoryStream ms, GitObject obj)
        {
            obj.Type = ms.ReadToSpace();
            obj.ContentLength = int.Parse(ms.ReadToNull());

            var result = new byte[obj.ContentLength];
            if (ms.Read(result, 0, result.Length) < result.Length)
                throw new Exception("読み取り可能なデータが不足しています");
            return result;
        }
    }
}
