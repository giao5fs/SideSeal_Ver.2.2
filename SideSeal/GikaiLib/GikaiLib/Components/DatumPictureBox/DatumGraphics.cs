using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace GikaiLib.Components
{
    /// <summary>
    /// 描画情報抽象クラス
    /// </summary>
    [Serializable]
    public abstract class DatumGraphics
    {
        #region プロパティ

        /// <summary> 描画用PenObj </summary>
        public Pen Penobj { get; set; }
        /// <summary> クラス名 </summary>
        public abstract string ClassName { get; }

        #endregion

        #region メソッド

        #endregion

        #region Private変数, メソッド

        #endregion

    }

    /// <summary>
    /// 線描画情報クラス
    /// </summary>
    [Serializable]
    public class DatumLineSeg : DatumGraphics
    {
        #region プロパティ

        /// <summary> 線開始位置(pix) </summary>
        public Point PStart { get; set; }
        /// <summary> 線終了位置(pix) </summary>
        public Point PEnd { get; set; }

        /// <summary> クラス名 </summary>
        public override string ClassName
        {
            get { return this.GetType().Name; }
        }

        #endregion

        #region メソッド

        /// <summary> コンストラクタ </summary>
        public DatumLineSeg(Pen penobj, Point pstart, Point pend)
        {
            Penobj = penobj;
            PStart = pstart;
            PEnd = pend;
        }

        #endregion

        #region Private変数, メソッド

        #endregion


    }

    /// <summary>
    /// 水平線描画情報クラス
    /// </summary>
    [Serializable]
    public class DatumLineHor : DatumGraphics
    {
        #region プロパティ

        /// <summary> 描画位置(Height方向) </summary>
        public int Value{get;set;}

        /// <summary> クラス名 </summary>
        public override string ClassName
        {
            get { return this.GetType().Name; }
        }

        #endregion

        #region メソッド

        /// <summary> コンストラクタ </summary>
        public DatumLineHor(Pen penobj, int value)
        {
            Penobj = penobj;
            Value = value;
        }

        #endregion

        #region Private変数, メソッド

        #endregion

    }

    /// <summary>
    /// 垂直線描画情報クラス
    /// </summary>
    [Serializable]
    public class DatumLineVer : DatumGraphics
    {
        #region プロパティ

        /// <summary> 描画位置(Height方向) </summary>
        public int Value { get; set; }

        /// <summary> クラス名 </summary>
        public override string ClassName
        {
            get { return this.GetType().Name; }
        }

        #endregion

        #region メソッド

        /// <summary> コンストラクタ </summary>
        public DatumLineVer(Pen penobj, int value)
        {
            Penobj = penobj;
            Value = value;
        }

        #endregion

        #region Private変数, メソッド

        #endregion

    }

    /// <summary>
    /// 十字線描画情報クラス
    /// </summary>
    [Serializable]
    public class DatumCross : DatumGraphics
    {
        #region プロパティ

        /// <summary> 交差位置(pix) </summary>
        public Point Pcross { get; set; }

        /// <summary> クラス名 </summary>
        public override string ClassName
        {
            get { return this.GetType().Name; }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="p">描画用PenObj</param>
        /// <param name="cross">交差位置(pix)</param>
        public DatumCross(Pen penobj, Point pcross)
        {
            Penobj = penobj;
            Pcross = pcross;
        }

        #endregion

        #region Private変数, メソッド

        #endregion


    }

    /// <summary>
    /// ポイント描画情報クラス
    /// </summary>
    [Serializable]
    public class DatumPoint : DatumGraphics
    {

        #region プロパティ

        /// <summary> ポイント表示位置(pix) </summary>
        public Point Ppoint { get; set; }

        /// <summary> クラス名 </summary>
        public override string ClassName
        {
            get { return this.GetType().Name; }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="p">描画用PenObj</param>
        /// <param name="cross">ポイント位置(pix)</param>
        public DatumPoint(Pen penobj, Point ppoint)
        {
            Penobj = penobj;
            Ppoint = ppoint;
        }

        #endregion

        #region Private変数, メソッド

        #endregion

    }

    /// <summary>
    /// 長方形描画情報クラス
    /// </summary>
    [Serializable]
    public class DatumRectangle : DatumGraphics
    {
        #region プロパティ

        /// <summary> 長方形情報(pix) </summary>
        public Rectangle Rect { get; set; }

        /// <summary> クラス名 </summary>
        public override string ClassName
        {
            get { return this.GetType().Name; }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="p">描画用PenObj</param>
        /// <param name="rect">長方形情報(pix)</param>
        public DatumRectangle(Pen penobj, Rectangle rect)
        {
            Penobj = penobj;
            Rect = rect;
        }

        #endregion

        #region Private変数, メソッド

        #endregion

    }
}

