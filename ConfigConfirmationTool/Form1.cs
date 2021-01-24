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
            // TODO: チェック対象ファイル.ini読み込み

            // 
        }
    }
}
