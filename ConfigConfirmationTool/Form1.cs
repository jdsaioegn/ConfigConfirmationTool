using ConfigConfirmationTool.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConfigConfirmationTool
{
    public partial class Form1 : Form
    {
        private List<DataModel> data = new List<DataModel>();

        private string log;

        public Form1()
        {
            InitializeComponent();
            // 
            log = GetAppSetting("LogFilePath");
        }

        /// <summary>
        /// 画面初期化時発生イベント
        /// </summary>
        /// <remarks>①チェック対象ファイル読み込み</remarks>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">イベント詳細</param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // ①チェック対象ファイル読み込み
            GetConfirmationList();
            // 
            foreach (DataModel item in data)
            {
                // 
                foreach (DataModel.FileInfo fileinfo in item.FileInfos)
                {
                    // 
                    foreach (DataModel.XmlInfo xmlinfo in item.XmlInfos)
                    {
                        // トレースモードがONの場合は検索対象をログに出力
                        if (GetAppSetting("TraceMode") == "ON") File.AppendAllText(log, "■検索対象ファイル：" + fileinfo.FilePath + "□検索条件：" + xmlinfo.SearchXmlNode + Environment.NewLine);
                        // 

                    }
                }
            }
        }

        /// <summary>
        /// App.config設定値取得
        /// </summary>
        /// <param name="key">設定値(key)</param>
        /// <returns>設定値(value)</returns>
        private string GetAppSetting(string key)
        {
            // App.configからkeyを持つvalueを呼び出し元へ返却
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// チェック対象ファイル読み込み
        /// </summary>
        private void GetConfirmationList()
        {
            // チェック対象ファイル.iniファイルパス取得
            string path = GetAppSetting("InitialFilePath");
            // チェック対象ファイル.ini読み込み
            string[] lines = File.ReadAllLines(path);
            // XML情報登録タグ用文字列
            string str = string.Empty;
            // 1行ごとに繰り返し
            foreach (string line in lines)
            {
                // セクション解析
                if (line.First() == '[')
                {
                    // Files項目のセクションの場合
                    if (line.Contains("Files]"))
                    {
                        // 変更IDごとに追加
                        data.Add(new DataModel()
                        {
                            // 変更IDの登録
                            ChangeID = line.Trim('[').Replace("Files]", "")
                        });
                    }
                    // Check項目のセクションの場合
                    else if (line.Contains("Check]"))
                    {
                        // 該当する変更IDが存在する場合はXML情報登録タブ用文字列を設定
                        if (data.Where(p => p.ChangeID == line.Trim('[').Replace("Check]", "")).Count() == 1)
                        {
                            str = line.Trim('[').Replace("Check]", "");
                        }
                        // 該当する変更IDが存在しない場合はメッセージを出力し処理を継続
                        else
                        {
                            // メッセージ出力
                            MessageBox.Show("Warning：検索ファイルと紐づかない検索情報が存在します。(詳細はログに記載)");
                            // ログファイルに変更ID不備セクション情報を出力
                            File.AppendAllText(log, line + "に該当する変更IDが存在しません。" + Environment.NewLine);
                        }
                    }
                    // 上記以外のセクションが設定されていた場合はメッセージを出力しツールを終了
                    else
                    {
                        // メッセージ出力
                        MessageBox.Show("Error：異常なセクションが登録されています。(詳細はログに記載)");
                        // ログファイルに異常検知セクション情報を出力
                        File.AppendAllText(log, "異常検知セクション：" + line + Environment.NewLine);
                        // アプリを強制終了
                        Environment.Exit(0);
                    }
                }
                // データの解析
                else
                {
                    // 先頭がFileの場合（ファイル情報）
                    if (line.StartsWith("\tFile"))
                    {
                        // ファイル情報登録
                        data.Last().FileInfos.Add(new DataModel.FileInfo()
                        {
                            // 完全パス
                            FilePath = line.Split('=').ElementAt(1).Split('|').ElementAt(0),
                            // ファイル名
                            FileName = Path.GetFileName(line.Split('=').ElementAt(1).Split('|').ElementAt(0))
                        });
                    }
                    // 先頭がFile以外の場合（XML情報）
                    else
                    {
                        // 登録対象となる変更IDが1つ存在する場合のみ登録する
                        if (data.Where(p => p.ChangeID == str).Count() == 1)
                        {
                            // 先頭タブを削除
                            string tmp = line.Trim('\t');
                            // XML情報登録
                            data.Where(p => p.ChangeID == str).Single().XmlInfos.Add(new DataModel.XmlInfo()
                            {
                                // 検索判定（[追加][変更](edi) または [削除](del) か判断）
                                Operation = tmp.Split('|').Last().Contains("del=") ? "del" : "edi",
                                // 
                                SearchXmlNode = GetSearchXmlNode(tmp)
                            });
                        }

                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string GetSearchXmlNode(string line)
        {
            // 戻り値用
            string str = string.Empty;
            // 
            string value = line.Split('=')?.ElementAt(2);
            // 階層情報設定
            string tmp = line.Split('=').ElementAt(0).Replace("p:", "");
            // 
            string[] attr = GetValue(line.Split('=').ElementAt(1), value);
            // 
            tmp.Split('/').ToList().ForEach(p => str += GetNode(p, attr));
            // 戻り値
            return str;
        }

        /// <summary>
        /// 解析文字列()から要素検索値を設定
        /// </summary>
        /// <param name="cmd">解析文字列</param>
        /// <returns>要素検索値設定済配列</returns>
        private string[] GetValue(string cmd, string value)
        {
            // 戻り値用：2つ目の「=」以降の値を読み取り「,」区切りで設定されている値を適宜要素検索値に設定
            string[] sp = cmd.Split('|').ElementAt(0).Split(',').Where(p => !string.IsNullOrEmpty(p)).ToArray();
            // 要素検索値が設定されている場合は要素を設定
            if (cmd.Split('|').ElementAt(1).EndsWith("attr"))
            {
                // 要素を追加
                Array.Resize(ref sp, sp.Count() + 1);
                // 要素検索値を設定
                sp[sp.Count() - 1] = value;
            }
            // 戻り値
            return sp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private string GetNode(string cmd, string[] value)
        {
            // 
            string[] sp = cmd.Split('|');
            // 戻り値用
            string str = "/" + sp.First();
            // 
            if (sp.Count() != 1)
            {
                // 
                str += "[";
                // 
                for (int i = 1; i < sp.Count(); i++)
                {
                    // 
                    str += "@" + sp[i] + "='" + value[i - 1] + "'";
                    // 
                    if (i < sp.Count() - 1)
                    {
                        str += " and ";
                    }
                }
                // 
                str += "]";
            }
            // 戻り値
            return str;
        }
    }
}
