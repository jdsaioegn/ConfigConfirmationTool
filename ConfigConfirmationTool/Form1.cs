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

        public Form1()
        {
            InitializeComponent();
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
                            MessageBox.Show("Warning：検索ファイルと紐づかない検索情報が存在します。");
                        }
                    }
                    // 上記以外のセクションが設定されていた場合はメッセージを出力しツールを終了
                    else
                    {
                        MessageBox.Show("Error：異常なセクションが登録されています。");
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
                            // XML情報登録
                            data.Where(p => p.ChangeID == str).Single().XmlInfos.Add(new DataModel.XmlInfo()
                            {
                                // TODO:汎用的
                                Operation = line
                            });
                        }

                    }
                }
            }
        }
    }
}
