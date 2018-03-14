using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GitReaderLibrary
{
    internal static class UtilExtensions
    {
        public static string ToHex(this byte[] buffer) => BitConverter.ToString(buffer).Replace("-", "").ToLower();

        public static byte[] Read(this Stream stream, int length)
        {
            var buf = new byte[length];
            var size = stream.Read(buf, 0, length);
            if (size < length)
                throw new Exception("読み取り可能なデータが不足しています");
            return buf;
        }

        public static int ReadInt32(this Stream stream) => BitConverter.ToInt32(stream.Read(4), 0);
        public static uint ReadUInt32(this Stream stream) => BitConverter.ToUInt32(stream.Read(4), 0);
        public static int ReadInt32BE(this Stream stream) => BitConverter.ToInt32(stream.Read(4).Reverse().ToArray(), 0);
        public static uint ReadUInt32BE(this Stream stream) => BitConverter.ToUInt32(stream.Read(4).Reverse().ToArray(), 0);

        public static short ReadInt16(this Stream stream) => BitConverter.ToInt16(stream.Read(2), 0);
        public static ushort ReadUInt16(this Stream stream) => BitConverter.ToUInt16(stream.Read(2), 0);
        public static short ReadInt16BE(this Stream stream) => BitConverter.ToInt16(stream.Read(2).Reverse().ToArray(), 0);
        public static ushort ReadUInt16BE(this Stream stream) => BitConverter.ToUInt16(stream.Read(2).Reverse().ToArray(), 0);

        private static string ReadToX(this Stream stream, byte to, bool must_end = true)
        {
            using(var ms = new MemoryStream())
            {
                while(stream.Position < stream.Length)
                {
                    var b = stream.ReadByte();
                    if (b == -1)
                    {
                        if (must_end)
                            throw new Exception("読み取り可能なデータが不足しています");
                        else
                            break;
                    }
                    if (b == to)
                        break;
                    ms.WriteByte((byte)b);
                }
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        public static string ReadToSpace(this Stream stream, bool must_end = true) => stream.ReadToX(0x20, must_end);
        public static string ReadToNull(this Stream stream, bool must_end = true) => stream.ReadToX(0x00, must_end);
    }
}
