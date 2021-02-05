using System.Collections.Generic;

namespace ConfigConfirmationTool.model
{
    /// <summary>
    /// 処理用モデルクラス
    /// </summary>
    /// <remarks>モデル</remarks>
    public class DataModel
    {
        /// <summary>
        /// 変更ID
        /// </summary>
        public string ChangeID { get; set; }

        /// <summary>
        /// ファイルメタ情報
        /// </summary>
        public List<FileInfo> FileInfos { get; set; }

        /// <summary>
        /// XML検証タグ情報
        /// </summary>
        public List<XmlInfo> XmlInfos { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataModel()
        {
            FileInfos = new List<FileInfo>();
            XmlInfos = new List<XmlInfo>();
        }

        /// <summary>
        /// 
        /// </summary>
        public class FileInfo
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class XmlInfo
        {
            public string Operation { get; set; }
            public string ChangeData { get; set; }
            public string SearchXmlNode { get; set; }
        }
    }
}
