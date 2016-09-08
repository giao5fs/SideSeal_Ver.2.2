using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SideSeal
{
    /// <summary>
    /// INIファイルクラス
    /// </summary>
    public static class IniFile
    {

        /// <summary>
        /// 使用方法：
        /// public void Func()
        /// {
        ///     string fileName = IniFile.ReadString("Path", "FileName", "Not Found", @"C:\temp\test.ini");
        ///     int    xPos     = IniFile.ReadInteger("Data", "XPos", -1, @"C:\temp\test.ini");
        ///     double yPos     = IniFile.ReadDouble("Data", "YPos", -1, @"C:\temp\test.ini");
        ///
        ///     IniFile.WriteString("Path", "FileName", @"C:\Data\Data.dat", @"C:\temp\test.ini");
        ///     IniFile.WriteInteger("Data", "XPos", 100, @"C:\temp\test.ini");
        ///     IniFile.WriteDouble("Data", "YPos", 20.5, @"C:\temp\test.ini");
        /// }
        /// </summary>

        static IniFile() { }

        #region DllImport定義
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int GetPrivateProfileString(string appName, string keyName, string defaultValue,
                                                          StringBuilder returnValue, uint size, string fileName);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int GetPrivateProfileInt(string appName, string keyName, int defaultValue, string fileName);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int WritePrivateProfileString(string appName, string keyName, string writeValue, string fileName);
        #endregion

        #region メンバ関数（静的メソッド）
        public static string ReadString(string appName, string keyName, string defaultValue, string fileName)
        {
            StringBuilder retString = new StringBuilder(1024);

            GetPrivateProfileString(appName, keyName, "Not Found", retString, (uint)retString.Capacity, fileName);
            return (retString.ToString() == "Not Found") ? defaultValue : retString.ToString();
        }
        public static int ReadInteger(string appName, string keyName, int defaultValue, string fileName)
        {
            return (int)GetPrivateProfileInt(appName, keyName, defaultValue, fileName);
        }
        public static double ReadDouble(string appName, string keyName, double defaultValue, string fileName)
        {
            StringBuilder retString = new StringBuilder(80);

            GetPrivateProfileString(appName, keyName, "Not Found", retString, (uint)retString.Capacity, fileName);
            return (retString.ToString() == "Not Found") ? defaultValue : Convert.ToDouble(retString.ToString());
        }
        public static bool ReadBool(string appName, string keyName, bool defaultValue, string fileName)
        {
            StringBuilder retString = new StringBuilder(80);

            GetPrivateProfileString(appName, keyName, "Not Found", retString, (uint)retString.Capacity, fileName);
            if (retString.ToString() == "Not Found")
            {
                return defaultValue;
            }

            int result;
            if (!Int32.TryParse(retString.ToString(), out result))
            {
                return defaultValue;
            }
            return (result == 1) ? true : false;
        }
        public static bool WriteString(string appName, string keyName, string writeValue, string fileName)
        {
            return (WritePrivateProfileString(appName, keyName, writeValue, fileName) != 0) ? true : false;
        }
        public static bool WriteInteger(string appName, string keyName, int writeValue, string fileName)
        {
            return (WritePrivateProfileString(appName, keyName, writeValue.ToString(), fileName) != 0) ? true : false;
        }
        public static bool WriteDouble(string appName, string keyName, double writeValue, string fileName)
        {
            return (WritePrivateProfileString(appName, keyName, writeValue.ToString(), fileName) != 0) ? true : false;
        }
        public static bool WriteBool(string appName, string keyName, bool writeValue, string fileName)
        {
            string value = (writeValue == true) ? "1" : "0";

            return (WritePrivateProfileString(appName, keyName, value, fileName) != 0) ? true : false;
        }
        #endregion
    }
}
