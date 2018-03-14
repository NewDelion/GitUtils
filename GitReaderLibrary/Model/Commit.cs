using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitReaderLibrary.Model
{
    public class Commit : GitObject
    {
        public string TreeSHA1 { get; set; }

        /// <summary>
        /// マージされたコミットの場合、複数のparentが指定されます
        /// </summary>
        public string[] ParentSHA1 { get; set; }

        public string Author { get; set; }

        public string Committer { get; set; }

        public string CommitMessage { get; set; }


        public static Commit Load(string path)
        {
            var result = new Commit();
            using (var decompressed_ms = new MemoryStream(Decompress(path)))
            using (var content_ms = new MemoryStream(ReadHeader(decompressed_ms, result)))
            using (var reader = new StreamReader(content_ms))
            {
                if (result.Type != "commit")
                    throw new InvalidDataException("commit形式のファイルではありません");

                string GetTitle(string line)
                {
                    int space = line.IndexOf(' ');
                    if (space <= 0)
                        throw new InvalidDataException("不正なファイル形式");
                    return line.Substring(0, space);
                }

                var parent_list = new List<string>();
                while (reader.Peek() != -1)
                {
                    var line = reader.ReadLine();
                    if (line == "")
                        break;
                    var title = GetTitle(line);
                    if (title == "tree")
                        result.TreeSHA1 = line.Substring(5);
                    else if (title == "parent")
                        parent_list.Add(line.Substring(7));
                    else if (title == "author")
                        result.Author = line.Substring(7);
                    else if (title == "committer")
                        result.Committer = line.Substring(9);
                }
                result.CommitMessage = reader.ReadToEnd();
                result.ParentSHA1 = parent_list.ToArray();

                return result;
            }
        }
    }
}
