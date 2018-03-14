using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using GitReaderLibrary.Model;

namespace GitDownloader
{
    class Program
    {
        /// <summary>
        /// endpointを返します。ダウンロードに失敗した場合nullを返します。
        /// </summary>
        static string Download(string base_url, string endpoint)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    if (File.Exists(endpoint))
                        return null;
                    var data = client.GetByteArrayAsync(base_url + endpoint).Result;
                    if ((endpoint.Contains('/') || endpoint.Contains('\\')) && !Directory.Exists(Path.GetDirectoryName(endpoint)))
                        Directory.CreateDirectory(Path.GetDirectoryName(endpoint));
                    File.WriteAllBytes(endpoint, data);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Download: {0}", endpoint);
                    Console.ResetColor();
                    return endpoint;
                }
                catch(Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Fail: {0}", endpoint);
                    Console.ResetColor();
                    return null;
                }
            }
        }

        class DownloadJob
        {
            public string Path { get; set; }
            public string SavePath { get; set; }
            public Func<string, DownloadJob[]> Callback { get; set; }
        }

        static void Main(string[] args)
        {
            Console.Write("BaseUrl(http://～/.git/): ");
            string base_url = Console.ReadLine();

            Console.Write("OutputDir: ");
            string output_dir = Console.ReadLine() + "/.git/";

            if (!Directory.Exists(output_dir))
                Directory.CreateDirectory(output_dir);
            Directory.SetCurrentDirectory(output_dir);

            var GetQueue = new Queue<DownloadJob>();
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "index",
                Callback = Read_index
            });
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "config",
                Callback = null
            });
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "description",
                Callback = null
            });
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "HEAD",
                Callback = null
            });
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "ORIG_HEAD",
                Callback = null
            });
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "FETCH_HEAD",
                Callback = null
            });
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "packed-refs",
                Callback = null
            });
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "COMMIT_EDITMSG",
                Callback = null
            });
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "info/exclude",
                Callback = null
            });
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "info/refs",
                Callback = null
            });
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "logs/HEAD",
                Callback = null
            });
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "logs/refs/heads/master",
                Callback = null
            });
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "logs/refs/remotes/origin/HEAD",
                Callback = null
            });
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "logs/refs/remotes/origin/master",
                Callback = null
            });
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "refs/heads/master",
                Callback = Read_Ref
            });
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "refs/remotes/origin/HEAD",
                Callback = null
            });
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "refs/remotes/origin/master",
                Callback = null
            });
            GetQueue.Enqueue(new DownloadJob
            {
                Path = "objects/info/packs",
                Callback = null
            });

            while(GetQueue.Count > 0)
            {
                var job = GetQueue.Dequeue();
                if (Download(base_url, job.Path) != null)
                {
                    if (job.Callback != null)
                    {
                        foreach (var new_job in job.Callback(job.Path))
                        {
                            GetQueue.Enqueue(new_job);
                        }
                    }
                }
            }
            
            Console.WriteLine("Finish!!");
            Console.ReadKey(true);
        }

        static DownloadJob[] Read_index(string path)
        {
            var NewJobs = new List<DownloadJob>();
            var index = Index.Load(path);
            foreach (var entry in index.Entries)
            {
                NewJobs.Add(new DownloadJob
                {
                    Path = "objects/" + entry.SHA1.Insert(2, "/"),
                    Callback = null
                });
            }
            return NewJobs.ToArray();
        }

        static DownloadJob[] Read_Tree(string path)
        {
            var NewJobs = new List<DownloadJob>();
            var tree = Tree.Load(path);
            foreach(var entry in tree.Entries)
            {
                var job = new DownloadJob();
                job.Path = "objects/" + entry.SHA1.Insert(2, "/");
                if (entry.IsDirectory)
                    job.Callback = Read_Tree;
                else
                    job.Callback = null;
                NewJobs.Add(job);
            }
            return NewJobs.ToArray();
        }

        static DownloadJob[] Read_Commit(string path)
        {
            var NewJobs = new List<DownloadJob>();
            var commit = Commit.Load(path);
            NewJobs.Add(new DownloadJob
            {
                Path = "objects/" + commit.TreeSHA1.Insert(2, "/"),
                Callback = Read_Tree
            });
            foreach (var parent in commit.ParentSHA1)
            {
                NewJobs.Add(new DownloadJob
                {
                    Path = "objects/" + parent.Insert(2, "/"),
                    Callback = Read_Commit
                });
            }
            return NewJobs.ToArray();
        }

        static DownloadJob[] Read_Ref(string path)
        {
            var lines = File.ReadAllLines(path);
            if (lines.Length == 0 || lines[0].Length != 40)
                return new DownloadJob[0];
            return new DownloadJob[1]
            {
                new DownloadJob
                {
                    Path = "objects/" + lines[0].Insert(2, "/"),
                    Callback = Read_Commit// .git/refs/heads/masterのため(メソッド名を変えようかな…)
                }
            };
        }
    }
}
