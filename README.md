# b-PACSample

## セットアップ

### b-PAC SDKをインストール
[アプリケーション開発ツール b-PAC｜開発者ツール｜ブラザー](http://www.brother.co.jp/dev/bpac/index.htm)

### 任意のGit用ディレクトリへ移動
```
cd path\to\dir
```

### GitHubからカレントディレクトリへclone
```
path\to\dir>git clone https://github.com/thinkAmi-sandbox/b-PACSample.git
```

### ラベルファイルの設定
#### `test.lbx`ファイルの作成
以下の内容で作成

|オブジェクト名|種類|
|---|---|
|Content|テキスト|
|Barcode|バーコード|


#### プロジェクトへの設定

- `test.lbx`ファイルをプロジェクトへ追加
- `test.lbx`ファイルのプロパティ`出力ディレクトリへとコピー`を`常にコピー`とする

　    
## テスト環境

- Windows10
- Visual Studio2015 Update1
- .NET Framework 4.6.1
- b-PAC SDK 3.0.17

　  
## 関係するブログ

- [ブラザーのラベルプリンタまわりを操作する b-PAC SDK を使ってみた - メモ的な思考的な](http://thinkami.hatenablog.com/entry/2016/02/12/062736)