using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GikaiLib.FileAccess
{
    /// <summary>
    /// ログ保存クラス
    /// </summary>
    public class WorkLogComm
    {
        #region 公開プロパティ

        /// <summary> 初期化メソッドで設定した項目Key列挙型obj </summary>
        public Enum KeysObj { get; private set; }

        #endregion

        #region 非公開変数

        /// <summary> 書き込み値記録用Dictionary </summary>
        protected Dictionary<string, string> _Dic;
        /// <summary> Data書き込み用StreamWriter </summary>
        protected System.IO.StreamWriter _Sw;

        #endregion

        #region 公開メソッド

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WorkLogComm()
        {
            _Dic = new Dictionary<string, string>();
            _Dic.Clear();

            KeysObj = null;
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <returns></returns>
        public bool Init(string filepath, Enum keys)
        {
            KeysObj = null;

            // ファイルの存在確認
            bool isflie = System.IO.File.Exists(filepath);

            // Logファイルをオープン
            try
            {
                _Sw = new System.IO.StreamWriter(filepath, true, Encoding.Default);
            }
            catch
            {
                return false;
            }

            // 列挙型の名称を列挙して、Dicに登録
            _Dic = new Dictionary<string, string>();
            _Dic.Clear();

            string[] tempstr = System.Enum.GetNames(keys.GetType());
            foreach (string item in tempstr)
            {
                _Dic.Add(item, "");
            }

            // ファイルが存在しない場合、項目記述
            if (!isflie)
            {
                foreach (var item in _Dic)
                {
                    _Sw.Write(item.Key);
                    _Sw.Write(",");
                }
                _Sw.WriteLine();
            }

            KeysObj = keys;
            return true;
        }

        /// <summary>
        /// ログデータ記録
        /// </summary>
        /// <param name="key">項目Key</param>
        /// <param name="value">記録値</param>
        /// <returns>成否flug</returns>
        public bool AddData(Enum key, string value)
        {
            if (key.GetType().FullName != KeysObj.GetType().FullName) { return false; }

            try
            {
                _Dic[key.ToString()] = value;
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 現在のログデータ確認
        /// </summary>
        /// <param name="key">項目Key</param>
        /// <param name="value">記録値</param>
        /// <returns></returns>
        public bool CheckData(Enum key, out string value)
        {
            value = "";

            try
            {
                if (key.GetType().FullName != KeysObj.GetType().FullName) { return false; }
            }
            catch
            {
                return false;
            }

            value = _Dic[key.ToString()];

            return true;
        }

        /// <summary>
        /// Log書き込み
        /// </summary>
        /// <returns></returns>
        public bool WriteLog()
        {
            string writetext = "";

            foreach (var item in _Dic)
            {
                writetext += item.Value;
                writetext += ",";
            }

            _Sw.WriteLine(writetext);
            _Sw.Flush();

            return true;
        }

        /// <summary>
        /// 値クリアメソッド
        /// </summary>
        /// <returns></returns>
        public bool ClearValues()
        {
//            List<string> tempkeylist = new List<string>();

            string[] keys = Enum.GetNames(KeysObj.GetType());

            foreach (var item in keys)
            {
                _Dic[item] = "";
            }

            return true;
        }

        #endregion

    }
}

