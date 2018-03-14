# GitUtils
Git関連で書いたコードを投げ

GitDownloaderは、誤って公開されている.gitディレクトリにアクセスし、gitの仕組みに従ってファイルを再帰的にダウンロードするプログラムです。packedファイルの参照はgit gcコマンドでのinfoファイル生成に依存しているのでサポートしていません。

git logから最新のコミットIDをコピーし、git reset --hard [commit id]というコマンドによりソースコードを復元することができます。

zlib.netを使っています。NuGetからインストールしてください。
