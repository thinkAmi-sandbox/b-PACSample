using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bPACSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var doc = new bpac.Document();

            // プリンタの情報を列挙
            ShowPrinterInfo(doc);

            // プリンタが印刷できる状態にあるか確認
            var enabledPrinter = GetEnabledPrinterName(doc);
            if (string.IsNullOrEmpty(enabledPrinter))
            {
                Console.WriteLine("利用できるプリンタがありません。");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"何かキーを押すと、{enabledPrinter}から印刷します。");
            Console.ReadLine();

            // ラベルの編集と印刷
            // 念のため、有効なプリンタを再設定
            doc.SetPrinter(enabledPrinter, false);

            var runDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var labelFilePath = System.IO.Path.Combine(runDir, @"test.lbx");

            var hasOpened = doc.Open(labelFilePath);
            if (!hasOpened)
            {
                Console.WriteLine("指定されたラベルを開けませんでした");
                Console.ReadLine();
                return;
            }


            // 印刷完了イベントに行う処理を追加
            doc.Printed += new bpac.IPrintEvents_PrintedEventHandler(HandlePrinted);


            // 印刷設定開始：最後でカットし、品質優先印刷を行う
            // C#の場合、オプションを複数設定するには、"論理OR演算子" (|)を使う
            // https://msdn.microsoft.com/ja-jp/library/6a71f45d.aspx
            doc.StartPrint("", bpac.PrintOptionConstants.bpoCutAtEnd | PrintOptionConstants.bpoQuality);

            // ラベルの編集
            for (int i = 1; i < 3; i++)
            {
                // ラベルのテキストオブジェクトを上書き
                doc.GetObject("Content").Text = $"No.{i.ToString()}";

                // ラベルのバーコードオブジェクトを上書き
                // GetBarcodeIndex()にオブジェクト名を渡す
                doc.SetBarcodeData(doc.GetBarcodeIndex("Barcode"), i.ToString());

                // 現在のシートの印刷を印刷ジョブに追加する
                // なお、第二引数は"無効"らしいので、何を指定しても変化しないっぽい
                doc.PrintOut(1, bpac.PrintOptionConstants.bpoAutoCut);
            }

            doc.EndPrint();
            doc.Close();

            Console.ReadLine();
        }


        /// <summary>
        /// プリンタまわりの情報を表示する
        /// </summary>
        /// <param name="doc"></param>
        private static void ShowPrinterInfo(bpac.IDocument doc)
        {
            Console.WriteLine("プリンタ情報を表示します");

            // GetInstalledPrinters()で取得できるのは、P-Touchのみ？
            var printerNameList = (object[])doc.Printer.GetInstalledPrinters();

            foreach (string printerName in printerNameList)
            {
                // プリンタ情報
                var support = doc.Printer.IsPrinterSupported(printerName) ? "Yes" : "No";
                var status = doc.Printer.IsPrinterOnline(printerName) ? "Online" : "Offline";
                Console.WriteLine($"{printerName} - Support: {support} , Status: {status}");

                // 複数プリンタがあってもdoc.Printerだけだと片方しか取得できないため、
                // doc.SetPrinter()で明示的にプリンタを指定してからdoc.Printerで情報を取得する
                // なお、ラベルの調整はしない
                doc.SetPrinter(printerName, false);


                // ラベル情報：
                // プリンタがオフライン・ラベルがセットされていない場合は
                // mediaId=0、mediaNameは長さゼロの文字列が返ってくる
                var mediaId = doc.Printer.GetMediaId();
                var mediaName = doc.Printer.GetMediaName();
                var mediaStatus = string.IsNullOrEmpty(doc.Printer.GetMediaName()) ?
                                  "No Media" :
                                  $"Label - {mediaId} : {mediaName}";
                Console.WriteLine(mediaStatus);
            }
        }


        /// <summary>
        /// 利用可能なプリンタ名を取得する(複数ある場合は、最初に見つかったものを選択)
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static string GetEnabledPrinterName(bpac.IDocument doc)
        {
            var printerNameList = (object[])doc.Printer.GetInstalledPrinters();

            foreach (string printerName in printerNameList)
            {
                doc.SetPrinter(printerName, false);

                if (doc.Printer.IsPrinterSupported(printerName) &&
                    doc.Printer.IsPrinterOnline(printerName) &&
                    !string.IsNullOrEmpty(doc.Printer.GetMediaName()))
                {
                    // プリンターがサポート・オンライン、かつ、
                    // メディアが設定されている場合に印刷可能なプリンタとみなす
                    return printerName;
                }
            }

            return "";
        }


        /// <summary>
        /// 印刷完了時のイベントでの処理
        /// </summary>
        /// <param name="status"></param>
        /// <param name="value"></param>
        private static void HandlePrinted(int status, object value)
        {
            Console.WriteLine("印刷が終わりました。");
            Console.WriteLine($"status: {status} / value: {value.ToString()}");
        }

    }
}
