using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace GikaiLib.Components
{
    /// <summary>
    /// 入力Bar表示コンポーネント(水平表示)
    /// </summary>
    public partial class PanelBarHor : System.Windows.Forms.Panel
    {
        #region プロパティ

        /// <summary> 表示範囲_最小値 </summary>
        public double BarMin { get; set; }
        /// <summary> 表示範囲_最大値 </summary>
        public double BarMax { get; set; }
        /// <summary> 許容範囲_最小値 </summary>
        public double SafeMin { get; set; }
        /// <summary> 許容範囲_最大値 </summary>
        public double SafeMax { get; set; }
        /// <summary> 許容範囲Shift量 </summary>
        public double SafeShift { get; set; }

        #endregion

        #region 非公開変数
        /// <summary> 同期処理用Obj </summary>
        object _SyncObj;

        #endregion

        #region メソッド

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PanelBarHor()
        {
            InitializeComponent();
            _SyncObj = new object();
        }

        /// <summary>
        /// 入力値描画実行
        /// </summary>
        /// <param name="value">入力値</param>
        /// <returns>入力値許容範囲内でtrue</returns>
        public bool DrawValue(double value)
        {
            int width = this.Width;
            int height = this.Height;
            bool isvalueok = false;

            Bitmap bit = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Graphics g = Graphics.FromImage(bit);

            // 画像の表示を初期化
            Pen p = new Pen(Color.Yellow, height + 2);

            // 初めに、黒色で塗りつぶし
            g.Clear(Color.Black);

            // ＯＫ範囲の計算
            int min = CalcPosForSafe(SafeMin + SafeShift);
            int max = CalcPosForSafe(SafeMax + SafeShift);

            // 最小, 最大の値が反転していれば、入れ替える。
            if (min > max) { SwapValue(ref min, ref max); }

            // 表示値を塗りつぶし

            int inval = CalcPosForValue(value);



            if (min < inval & inval < max)
            {
                p.Color = Color.Aquamarine;
            }
            else
            {
                p.Color = Color.Yellow;
            }

            g.DrawLine(p, min, height / 2 + 1, max, height / 2 + 1);

            if (min < inval & inval < max)
            {
                p.Color = Color.Turquoise;
                isvalueok = true;

                g.DrawLine(p, (inval - 9), height / 2 + 1, (inval + 9), height / 2 + 1);
            }
            else
            {
                p.Color = Color.Red;
                isvalueok = false;

                g.DrawLine(p, (inval - 9), height / 2 + 1, (inval + 9), height / 2 + 1);
            }

            g.Dispose();

            lock (_SyncObj)
            {
                // 書込み実行
                g = Graphics.FromHwnd(this.Handle);
                g.DrawImage((Image)bit, 0, 0);
                g.Dispose();
            }

            return isvalueok;
        }

        /// <summary>
        /// BarをGrayに塗りつぶす
        /// </summary>
        public void DrawGray()
        {
            // 灰色塗りつぶし画像作成
            int width = this.Width;
            int height = this.Height;

            Bitmap bit = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bit);

            g.Clear(Color.Black);
            g.Dispose();

            lock (_SyncObj)
            {
                // 書込み実行
                g = Graphics.FromHwnd(this.Handle);
                g.DrawImage((Image)bit, 0, 0);
                g.Dispose();
            }
        }

        #endregion

        #region 非公開メソッド

        /// <summary>
        /// 指定値から、Bar上のPositionを計算するメソッド(Value)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int CalcPosForValue(double value)
        {
            int width = this.Width;
            int height = this.Height;

            double barmin_loc = BarMin;
            double barmax_loc = BarMax;

            if (barmin_loc > barmax_loc) { SwapValue(ref barmin_loc, ref barmax_loc); }


            double temp = (value - barmin_loc) / (barmax_loc - barmin_loc) * width;

            if (temp < 9) { temp = 9; }
            else if (temp > width - 9) { temp = width - 9; }

            return (int)temp;
        }

        /// <summary>
        /// 指定値から、Bar上のPositionを計算するメソッド(Safe)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int CalcPosForSafe(double value)
        {
            int width = this.Width;
            int height = this.Height;

            double barmin_loc = BarMin;
            double barmax_loc = BarMax;

            if (barmin_loc > barmax_loc) { SwapValue(ref barmin_loc, ref barmax_loc); }


            double temp = (value - barmin_loc) / (barmax_loc - barmin_loc) * width;

            if (temp < 0) { temp = 0; }
            else if (temp > width) { temp = width; }

            return (int)temp;
        }

        /// <summary>
        /// 値入れ替えメソッド
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        private void SwapValue(ref int value1, ref int value2)
        {
            int temp = value2;
            value2 = value1;
            value1 = temp;
        }

        /// <summary>
        /// 値入れ替えメソッド
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        private void SwapValue(ref double value1, ref double value2)
        {
            double temp = value2;
            value2 = value1;
            value1 = temp;
        }

        #endregion
    }

    /// <summary>
    /// 入力Bar表示コンポーネント(垂直表示)
    /// </summary>
    public partial class PanelBarVer : System.Windows.Forms.Panel
    {
        #region プロパティ

        /// <summary> 表示範囲_最小値 </summary>
        public double BarMin { get; set; }
        /// <summary> 表示範囲_最大値 </summary>
        public double BarMax { get; set; }
        /// <summary> 許容範囲_最小値 </summary>
        public double SafeMin { get; set; }
        /// <summary> 許容範囲_最大値 </summary>
        public double SafeMax { get; set; }
        /// <summary> 許容範囲Shift量 </summary>
        public double SafeShift { get; set; }

        #endregion

        #region 非公開変数
        /// <summary> 同期処理用Obj </summary>
        object _SyncObj;

        #endregion

        #region メソッド

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PanelBarVer()
        {
            InitializeComponent();
            _SyncObj = new object();
        }

        /// <summary>
        /// 入力値描画実行
        /// </summary>
        /// <param name="value">入力値</param>
        /// <returns>入力値許容範囲内でtrue</returns>
        public bool DrawValue(double value)
        {
            int width = this.Width;
            int height = this.Height;
            bool isvalueok = false;

            Bitmap bit = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Graphics g = Graphics.FromImage(bit);

            // 画像の表示を初期化
            Pen p = new Pen(Color.Yellow, width + 2);

            // 初めに、黄色で塗りつぶし
            g.Clear(Color.Yellow);

            // safety範囲を、水色で塗りつぶし
            int min = CalcPosForSafe(SafeMin + SafeShift);
            int max = CalcPosForSafe(SafeMax + SafeShift);

            // 最小, 最大の値が反転していれば、入れ替える。
            if (min > max) { SwapValue(ref min, ref max); }

            p.Color = Color.Aquamarine;
            g.DrawLine(p, width / 2 + 1, height - min, width / 2 + 1, height-max);

            // 表示値を塗りつぶし
            int inval = CalcPosForValue(value);

            if (min < inval & inval < max)
            {
                p.Color = Color.Blue;
                isvalueok = true;
            }
            else
            {
                p.Color = Color.Red;
                isvalueok = false;
            }
            g.DrawLine(p, width / 2 + 1,height- (inval - 9), width / 2 + 1,height- (inval + 9));

            g.Dispose();

            lock (_SyncObj)
            {
                // 書込み実行
                g = Graphics.FromHwnd(this.Handle);
                g.DrawImage((Image)bit, 0, 0);
                g.Dispose();
            }

            return isvalueok;
        }

        /// <summary>
        /// BarをGrayに塗りつぶす
        /// </summary>
        public void DrawGray()
        {
            // 灰色塗りつぶし画像作成
            int width = this.Width;
            int height = this.Height;

            Bitmap bit = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bit);

            g.Clear(Color.Gray);
            g.Dispose();

            lock (_SyncObj)
            {
                // 書込み実行
                g = Graphics.FromHwnd(this.Handle);
                g.DrawImage((Image)bit, 0, 0);
                g.Dispose();
            }
        }

        #endregion

        #region 非公開メソッド

        /// <summary>
        /// 指定値から、Bar上のPositionを計算するメソッド(Value)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int CalcPosForValue(double value)
        {
            int width = this.Width;
            int height = this.Height;

            double barmin_loc = BarMin;
            double barmax_loc = BarMax;

            if (barmin_loc > barmax_loc) { SwapValue(ref barmin_loc, ref barmax_loc); }


            double temp = (value - barmin_loc) / (barmax_loc - barmin_loc) * height;

            if (temp < 9) { temp = 9; }
            else if (temp > height - 9) { temp = height - 9; }

            return (int)temp;
        }

        /// <summary>
        /// 指定値から、Bar上のPositionを計算するメソッド(Safe)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int CalcPosForSafe(double value)
        {
            int width = this.Width;
            int height = this.Height;

            double barmin_loc = BarMin;
            double barmax_loc = BarMax;

            if (barmin_loc > barmax_loc) { SwapValue(ref barmin_loc, ref barmax_loc); }


            double temp = (value - barmin_loc) / (barmax_loc - barmin_loc) * height;

            if (temp < 0) { temp = 0; }
            else if (temp > height) { temp = height; }

            return (int)temp;
        }

        /// <summary>
        /// 値入れ替えメソッド
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        private void SwapValue(ref int value1, ref int value2)
        {
            int temp = value2;
            value2 = value1;
            value1 = temp;
        }

        /// <summary>
        /// 値入れ替えメソッド
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        private void SwapValue(ref double value1, ref double value2)
        {
            double temp = value2;
            value2 = value1;
            value1 = temp;
        }

        #endregion
    }

    

        /// <summary>
    /// 入力Bar表示コンポーネント(2次元表示)
    /// </summary>
    public partial class PanelBar2Dim : System.Windows.Forms.Panel
    {
        #region プロパティ

        /// <summary> 表示範囲_X_最小値 </summary>
        public double BarMin_X { get; set; }
        /// <summary> 表示範囲_X_最大値 </summary>
        public double BarMax_X { get; set; }
        /// <summary> 許容範囲_X_最小値 </summary>
        public double SafeMin_X { get; set; }
        /// <summary> 許容範囲_X_最大値 </summary>
        public double SafeMax_X { get; set; }
        /// <summary> 許容範囲_XShift量 </summary>
        public double SafeShift_X { get; set; }

        /// <summary> 表示範囲_Y_最小値 </summary>
        public double BarMin_Y { get; set; }
        /// <summary> 表示範囲_Y_最大値 </summary>
        public double BarMax_Y { get; set; }
        /// <summary> 許容範囲_Y_最小値 </summary>
        public double SafeMin_Y { get; set; }
        /// <summary> 許容範囲_Y_最大値 </summary>
        public double SafeMax_Y { get; set; }
        /// <summary> 許容範囲_YShift量 </summary>
        public double SafeShift_Y { get; set; }

        #endregion

        #region 非公開変数
        /// <summary> 同期処理用Obj </summary>
        object _SyncObj;

        #endregion
        /// <summary> 表示値の表示幅 </summary>
        public const int VALUERECT_WIDTH = 30;
        public const int VALUERECT_HEIGHT = 30;


        #region メソッド

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PanelBar2Dim()
        {
            InitializeComponent();
            _SyncObj = new object();
        }

        /// <summary>
        /// 入力値描画実行
        /// </summary>
        /// <param name="value">入力値</param>
        /// <returns>入力値許容範囲内でtrue</returns>
        public bool DrawValue(double value_x, double value_y)
        {
            int width = this.Width;
            int height = this.Height;
            bool isvalueok = false;

            Bitmap bit = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Graphics g = Graphics.FromImage(bit);

            // 初めに、黄色で塗りつぶし
            g.Clear(Color.Black);

            // safety範囲を、水色で塗りつぶし
            int min_X = CalcPos_X(SafeMin_X + SafeShift_X);
            int max_X = CalcPos_X(SafeMax_X + SafeShift_X);

            int min_Y = CalcPos_Y(SafeMin_Y + SafeShift_Y);
            int max_Y = CalcPos_Y(SafeMax_Y + SafeShift_Y);

            // 最小, 最大の値が反転していれば、入れ替える。
            if (min_X > max_X) { SwapValue(ref min_X, ref max_X); }
            if (min_Y > max_Y) { SwapValue(ref min_Y, ref max_Y); }

       

            //g.FillRectangle(Brushes.Aquamarine,min_X, min_Y, (max_X - min_X), (max_Y - min_Y));
            //g.FillRectangle(Brushes.Red, CalcRectAngle(value_x, value_y));

            // 塗りつぶし、Safe範囲内の場合
            if (SafeMin_X < value_x & value_x < SafeMax_X &
                SafeMin_Y < value_y & value_y < SafeMax_Y)
            {
                g.FillRectangle(Brushes.Aquamarine, min_X, min_Y, (max_X - min_X), (max_Y - min_Y));
                //  g.FillRectangle(Brushes.Red, CalcRectAngle(value_x, value_y));
                isvalueok = true;
            }
            else
            {
                g.FillRectangle(Brushes.Yellow, min_X, min_Y, (max_X - min_X), (max_Y - min_Y));
                g.FillRectangle(Brushes.Red, CalcRectAngle(value_x, value_y));
                isvalueok = false;
            }
      

            // 表示値を塗りつぶし
      //      int inval = CalcPosForValue(value);

    //        if (min < inval & inval < max)
       //     {
      //          p.Color = Color.Blue;
      //          isvalueok = true;
      //      }
       //     else
       //     {
        //        p.Color = Color.Red;
        //        isvalueok = false;
       //     }
         
            g.Dispose();

            lock (_SyncObj)
            {
                // 書込み実行
                g = Graphics.FromHwnd(this.Handle);
                g.DrawImage((Image)bit, 0, 0);
                g.Dispose();
            }

            return isvalueok;
        }

        /// <summary>
        /// BarをGrayに塗りつぶす
        /// </summary>
        public void DrawGray()
        {
            // 灰色塗りつぶし画像作成
            int width = this.Width;
            int height = this.Height;

            Bitmap bit = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bit);

            g.Clear(Color.Gray);
            g.Dispose();

            lock (_SyncObj)
            {
                // 書込み実行
                g = Graphics.FromHwnd(this.Handle);
                g.DrawImage((Image)bit, 0, 0);
                g.Dispose();
            }
        }

        #endregion

        #region 非公開メソッド

        /// <summary>
        /// 指定値から、XPositionを計算するメソッド
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int CalcPos_X(double value)
        {
            int width = this.Width;
            int height = this.Height;

            double barmin_loc = BarMin_X;
            double barmax_loc = BarMax_X;

            if (barmin_loc > barmax_loc) { SwapValue(ref barmin_loc, ref barmax_loc); }


            double temp = (value - barmin_loc) / (barmax_loc - barmin_loc) * width;

            if (temp < 0) { temp = 0; }
            else if (temp > width) { temp = width; }

            return (int)temp;
        }

        /// <summary>
        /// 指定値から、YPositionを計算するメソッド
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int CalcPos_Y(double value)
        {
            int width = this.Width;
            int height = this.Height;

            double barmin_loc = BarMin_Y;
            double barmax_loc = BarMax_Y;

            if (barmin_loc > barmax_loc) { SwapValue(ref barmin_loc, ref barmax_loc); }

            double temp = (value - barmin_loc) / (barmax_loc - barmin_loc) * height;

            if (temp < 0) { temp = 0; }
            else if (temp > height) { temp = height; }

            return (int)temp;
        }

        /// <summary>
        /// 位置から、表示Rectangleを計算するメソッド
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Rectangle CalcRectAngle(double pos_x, double pos_y)
        {
            int width = this.Width;
            int height = this.Height;

            double barmin_loc_x = BarMin_X;
            double barmax_loc_x = BarMax_X;

            double barmin_loc_y = BarMin_Y;
            double barmax_loc_y = BarMax_Y;

            if (barmin_loc_x > barmax_loc_x) { SwapValue(ref barmin_loc_x, ref barmax_loc_x); }
            if (barmin_loc_y > barmax_loc_y) { SwapValue(ref barmin_loc_y, ref barmax_loc_y); }

            Rectangle rect = new Rectangle();

            rect.X = (int)((pos_x - barmin_loc_x) / (barmax_loc_x - barmin_loc_x) * (double)width);
            rect.Y = (int)((pos_y - barmin_loc_y) / (barmax_loc_y - barmin_loc_y) * (double)height);

            rect.X -= VALUERECT_WIDTH / 2;
            rect.Y -= VALUERECT_HEIGHT / 2;

            rect.Width = VALUERECT_WIDTH;
            rect.Height = VALUERECT_HEIGHT;

            if (rect.X < 0) { rect.X = 0; }
            else if (rect.X + rect.Width > width) { rect.X = width - rect.Width; }

            if (rect.Y < 0) { rect.Y = 0; }
            else if (rect.Y + rect.Height > height) { rect.Y = height - rect.Height; }

            return rect;
        }

        /// <summary>
        /// 値入れ替えメソッド
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        private void SwapValue(ref int value1, ref int value2)
        {
            int temp = value2;
            value2 = value1;
            value1 = temp;
        }

        /// <summary>
        /// 値入れ替えメソッド
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        private void SwapValue(ref double value1, ref double value2)
        {
            double temp = value2;
            value2 = value1;
            value1 = temp;
        }

        #endregion
    }



}
