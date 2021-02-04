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
            public string Element1 { get; set; }
            public string Element2 { get; set; }
            public string Attribute2 { get; set; }
            public string AttributeValue2 { get; set; }
            public string Element3 { get; set; }
            public string Attribute3 { get; set; }
            public string AttributeValue3 { get; set; }
            public string Element4 { get; set; }
            public string Attribute4 { get; set; }
            public string AttributeValue4 { get; set; }
            public string Element5 { get; set; }
            public string Attribute5 { get; set; }
            public string AttributeValue5 { get; set; }
            public string Element6 { get; set; }
            public string Attribute6 { get; set; }
            public string AttributeValue6 { get; set; }
            public string ChangeAttribute { get; set; }
        }
    }
}
