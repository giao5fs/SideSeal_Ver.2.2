using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GikaiLib.Components
{
    /// <summary>
    /// 基準線描画機能付加PictureBox
    /// </summary>
    public partial class DatumPictureBox : System.Windows.Forms.PictureBox
    {
        #region プロパティ

        /// <summary> 描画基準線情報格納配列 </summary>
        public List<DatumGraphics> GraphicsList;

        #endregion

        #region 非公開変数, メソッド

        #endregion

        #region メソッド

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DatumPictureBox()
        {
            InitializeComponent();
            GraphicsList = new List<DatumGraphics>();
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        /// <summary>
        /// 描画先コントロール設定
        /// </summary>
        /// <param name="parent">描画先コントロール</param>
        public void SetParentControl(Control parent)
        {
            this.Parent = parent;
            this.Size = new Size(parent.Width, parent.Height);
            this.Location = new Point(0, 0);
            this.BringToFront();
            this.BackColor = Color.Transparent;
        }

        /// <summary>
        /// 基準線描画メソッド
        /// </summary>
        public void DrawDatum()
        {
            Bitmap bmp = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
            bmp.MakeTransparent();
            Graphics g = Graphics.FromImage(bmp);

            using (g)
            {
                g.Clear(Color.Transparent);
                if (!WriteGraphics(g, 1.0)) { return; }
            }

            this.Image = bmp;
        }

        /// <summary>
        /// 基準線描画メソッド
        /// </summary>
        /// <param name="rate">描画倍率</param>
        public void DrawDatum(double rate)
        {
            Bitmap bmp = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
            bmp.MakeTransparent();
            Graphics g = Graphics.FromImage(bmp);

            using (g)
            {
                g.Clear(Color.Transparent);
                if (!WriteGraphics(g, rate)) { return; }
            }

            this.Image = bmp;
        }



        /// <summary>
        /// 画像への基準線表示
        /// </summary>
        /// <returns></returns>
        public bool BitmapAddDatum(Bitmap inbmp,out Bitmap outbmp)
        {
            outbmp = new Bitmap((int)(inbmp.Width ), (int)(inbmp.Height ), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, inbmp.Width, inbmp.Height);
            if (inbmp.PixelFormat != PixelFormat.Format8bppIndexed) { return false; }

            Graphics g = Graphics.FromImage(outbmp);
            using (g)
            {
                try
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    g.DrawImage(inbmp, new Rectangle(0, 0, (int)(rect.Width), (int)(rect.Height)));
                    if (!WriteGraphics(g, 1.0)) { return false; }
                }
                catch
                {
                    return false;
                }
            }

            return true;

        }

        #endregion

        #region Private変数, メソッド


        /// <summary>
        /// 基準線書込みメソッド
        /// </summary>
        /// <param name="g"> 書込み先Graphics </param>
        /// <param name="rate"> 描画倍率 </param>
        /// <returns></returns>
        protected bool WriteGraphics(Graphics g, double rate)
        {
            try
            {
                foreach (DatumGraphics item in GraphicsList)
                {
                    // LineSeg描画の場合
                    if (item is DatumLineSeg)
                    {
                        DatumLineSeg tempitem = (DatumLineSeg)item;

                        Point start = new Point((int)(tempitem.PStart.X * rate), (int)(tempitem.PStart.Y * rate));
                        Point end = new Point((int)(tempitem.PEnd.X * rate), (int)(tempitem.PEnd.Y * rate));

                        g.DrawLine(tempitem.Penobj, start, end);
                    }
                    // CrossLine描画の場合
                    else if (item is DatumCross)
                    {
                        DatumCross tempitem = (DatumCross)item;

                        Point cross = new Point((int)(tempitem.Pcross.X * rate), (int)(tempitem.Pcross.Y * rate));

                        Point sp = new Point(0, cross.Y);
                        Point ep = new Point(Width, cross.Y);
                        g.DrawLine(tempitem.Penobj, sp, ep);

                        sp = new Point(cross.X, 0);
                        ep = new Point(cross.X, Height);
                        g.DrawLine(tempitem.Penobj, sp, ep);
                    }

                    // LineHor描画の場合
                    else if (item is DatumLineHor)
                    {
                        DatumLineHor tempitem = (DatumLineHor)item;

                        Point start = new Point(0, (int)(tempitem.Value * rate));
                        Point end = new Point(Width, (int)(tempitem.Value * rate));

                        g.DrawLine(tempitem.Penobj, start, end);
                    }

                    // LineVer描画の場合
                    else if (item is DatumLineVer)
                    {
                        DatumLineVer tempitem = (DatumLineVer)item;

                        Point start = new Point((int)(tempitem.Value * rate), 0);
                        Point end = new Point((int)(tempitem.Value * rate), Height);

                        g.DrawLine(tempitem.Penobj, start, end);
                    }

                    // Point描画の場合
                    else if (item is DatumPoint)
                    {
                        DatumPoint tempitem = (DatumPoint)item;

                        Point pnt = new Point((int)(tempitem.Ppoint.X * rate), (int)(tempitem.Ppoint.Y * rate));

                        Point sp = new Point(pnt.X - 3, pnt.Y);
                        Point ep = new Point(pnt.X + 3, pnt.Y);
                        g.DrawLine(tempitem.Penobj, sp, ep);

                        sp = new Point(pnt.X, pnt.Y - 3);
                        ep = new Point(pnt.X, pnt.Y + 3);
                        g.DrawLine(tempitem.Penobj, sp, ep);
                    }

                    // 長方形描画の場合
                    else if (item is DatumRectangle)
                    {
                        DatumRectangle tempitem = (DatumRectangle)item;

                        Rectangle rectangle = new Rectangle
                            ((int)(tempitem.Rect.X * rate), (int)(tempitem.Rect.Y * rate),
                            (int)(tempitem.Rect.Width * rate), (int)(tempitem.Rect.Height * rate));

                        g.DrawRectangle(tempitem.Penobj, rectangle);
                    }

                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 画像変換メソッド(Byte[] ⇒ Bitmap(8bpp))
        /// </summary>
        /// <param name="pic">画像データ</param>
        /// <param name="extime">露光時間(msec)</param>
        protected void ConvertByteToBitmap(byte[] bytebmp,int width, int height, out Bitmap bmp)
        {
            // 変換実行
            bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            BitmapData bdata = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            Marshal.Copy(bytebmp, 0, bdata.Scan0, width * height);
            bmp.UnlockBits(bdata);

            // パレット設定
            ColorPalette palette = bmp.Palette;
            int nColors = 256;
            for (uint i = 0; i < nColors; i++)
            {
                uint Alpha = 0xFF;
                uint Intensity = (uint)(i * 0xFF / (nColors - 1));
                palette.Entries[i] = Color.FromArgb(
                    (int)Alpha,
                    (int)Intensity,
                    (int)Intensity,
                    (int)Intensity);
            }
            bmp.Palette = palette;
        }

        #endregion
    }
}
