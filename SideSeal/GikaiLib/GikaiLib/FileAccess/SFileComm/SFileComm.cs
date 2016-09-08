using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GikaiLib.FileAccess
{
    /// <summary>
    /// SystemFile読出しクラス
    /// </summary>
    public class SFileComm
    {

        #region プロパティ
        /// <summary> 初期化メソッドで設定した読出しKey列挙型obj </summary>
        public Enum KeysObj { get { return _KeysObj; } }

        #endregion

        #region 非公開変数

        /// <summary> 読み出し結果格納dic </summary>
        private Dictionary<string, string> _Dic;
        /// <summary> Key情報格納リスト </summary>
        private List<string> _KeyStrings;
        /// <summary> 読出しKey列挙型obj </summary>
        private Enum _KeysObj;

        #endregion

        #region 公開メソッド

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SFileComm()
        {
            _Dic = new Dictionary<string, string>();
            _KeyStrings = new List<string>();
            _KeysObj = null;
        }

        /// <summary>
        /// 初期化メソッド
        /// </summary>
        /// <param name="filepath">SystemFileのパス</param>
        /// <param name="enumkeys">読出Key列挙型obj</param>
        /// <returns>エラーコード</returns>
        public bool Init(string filepath, Enum keys)
        {
            _KeysObj = null;
            _KeyStrings.Clear();
            _Dic.Clear();

            // EnumよりKey登録実行
            string[] keynames = System.Enum.GetNames(keys.GetType());
            foreach (string item in keynames) { _KeyStrings.Add(item); }

            // File読み出し処理
            var tempdic = new Dictionary<string, string>();
            tempdic.Clear();

            try
            {
                var sr = new System.IO.StreamReader(filepath, Encoding.Default);
                while (true)
                {
                    string temp = sr.ReadLine();
                    if (temp == null) { break; }
                    else if (temp != "")
                    {
                        string[] tempsp = temp.Split(',');

                        if (tempsp.GetLength(0) == 3)
                        {
                            tempdic.Add(tempsp[1], tempsp[2]);
                        }
                    }
                }
                sr.Dispose();
            }
            catch
            {
                return false;
            }

            // 仮リストの値が設定Key全てを満たすかを調査
            _Dic.Clear();

            if (_KeyStrings.Count == 0) { return false; }

            foreach (string item in _KeyStrings)
            {
                string resultstr;
                if (!tempdic.TryGetValue(item, out resultstr))
                {
                    _Dic.Clear();
                    return false;

                }
                _Dic.Add(item, resultstr);
            }

            _KeysObj = keys;
            return true;
        }

        /// <summary>
        /// 値読み出しメソッド
        /// </summary>
        /// <param name="enumkeys">読出Key列挙型</param>
        /// <param name="value">読出結果</param>
        /// <returns>エラーコード</returns>
        public bool ReadValue(Enum enumkeys, out int value)
        {
            string tempst = "";
            value = 0;

            if (enumkeys.GetType().FullName != _KeysObj.GetType().FullName) { return false; }

            if (_KeysObj == null) { return false; }
            if (!_Dic.TryGetValue(enumkeys.ToString(), out tempst)) { return false; }
            if (!int.TryParse(tempst, out value)) { return false; }

            return true;
        }

        /// <summary>
        /// 値読み出しメソッド
        /// </summary>
        /// <param name="enumkeys">読出Key列挙型</param>
        /// <param name="value">読出結果</param>
        /// <returns>エラーコード</returns>
        public bool ReadValue(Enum enumkeys, out double value)
        {
            string tempst = "";
            value = 0;

            if (enumkeys.GetType().FullName != _KeysObj.GetType().FullName) { return false; }

            if (_KeysObj == null) { return false; }
            if (!_Dic.TryGetValue(enumkeys.ToString(), out tempst)) { return false; }
            if (!double.TryParse(tempst, out value)) { return false; }

            return true;
        }

        /// <summary>
        /// 値読み出しメソッド
        /// </summary>
        /// <param name="enumkeys">読出Key列挙型</param>
        /// <param name="value">読出結果</param>
        /// <returns>エラーコード</returns>
        public bool ReadValue(Enum enumkeys, out string value)
        {
            string tempst = "";
            value = "";

            if (enumkeys.GetType().FullName != _KeysObj.GetType().FullName) { return false; }

            if (_KeysObj == null) { return false; }
            if (!_Dic.TryGetValue(enumkeys.ToString(), out tempst)) { return false; }
            value = tempst;

            return true;
        }

        /// <summary>
        /// 値読み出しメソッド
        /// </summary>
        /// <param name="enumkeys">読出Key列挙型</param>
        /// <param name="value">読出結果</param>
        /// <returns>エラーコード</returns>
        public bool ReadValue(Enum enumkeys, out bool value)
        {
            string tempst = "";
            value = false;

            if (enumkeys.GetType().FullName != _KeysObj.GetType().FullName) { return false; }

            if (_KeysObj == null) { return false; }
            if (!_Dic.TryGetValue(enumkeys.ToString(), out tempst)) { return false; }
            if (!bool.TryParse(tempst, out value)) { return false; }

            return true;
        }

        #endregion

        #region 非公開メソッド

        #endregion

    }

}
