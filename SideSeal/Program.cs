using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace SideSeal
{
    static class Program
    {
        
        /// <summary> アプリケーション固定名 </summary>
        private static string appConstName;
        /// <summary> 多重起動を禁止するミューテックス </summary>
        private static Mutex mutexObject;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 多重起動禁止処理
            appConstName = @"Global\SideSeal";
            try
            {
                mutexObject = new Mutex(false, appConstName);
            }
            catch
            {
                MessageBox.Show("Already Run!", "Warning");
                return;
            }

            // Mutex取得成功⇒画面起動　失敗⇒Message後終了
            if (mutexObject.WaitOne(0, false))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form_Main());

                mutexObject.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("Already Run!", "Warning");
            }

            // Mutexの破棄
            mutexObject.Close();
        }
    }
}
