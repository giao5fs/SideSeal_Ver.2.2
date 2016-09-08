using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
//using FVXCOMIMAGELib;
//using FVXCOMFILEACCESSLib;
//using FVDATASLib;
//using FVXCOMVIDEOLib;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace GikaiLib.MachineAccess
{

    /// <summary>
    /// カメラ通信クラス
    /// </summary>
    public abstract class CameraComm
    {
        #region 公開プロパティ

        /// <summary> X方向_PixSize </summary>
        public abstract double PixSize_X { get; set; }
        /// <summary> Y方向_PixSize </summary>
        public abstract double PixSize_Y { get; set; }
        /// <summary> 画像Width </summary>
        public abstract int Width { get; protected set; }
        /// <summary> 画像Height </summary>
        public abstract int Height { get; protected set; }

        #endregion

        #region 公開メソッド

        /// <summary>
        /// 初期化メソッド
        /// </summary>
        /// <returns> エラーコード </returns>
        public abstract bool Init();

        /// <summary>
        /// 画像取得(Bitmap)
        /// </summary>
        /// <param name="pic">画像データ</param>
        /// <param name="extime">露光時間(msec)</param>
        /// <returns>エラーコード</returns>
        public abstract bool GetPictureForBMP(out Bitmap pic, int extime);

        /// <summary>
        /// 画像取得(Byte配列)
        /// </summary>
        /// <param name="pic">画像データ</param>
        /// <param name="extime">露光時間(msec)</param>
        /// <returns>エラーコード</returns>
        public abstract bool GetPictureForByte(out byte[] pic, int extime);

        /// <summary>
        /// 画像取得(Byte and Bitmap配列)
        /// </summary>
        /// <param name="pic">画像データ</param>
        /// <param name="extime">露光時間(msec)</param>
        /// <returns>エラーコード</returns>
        public abstract bool GetPictureForByteandBMP(out Bitmap pic, out byte[] picbyte, int extime);

        /// <summary>
        /// 指定パスへの画像保存
        /// </summary>
        /// <param name="filepath">保存先パス(拡張子無)</param>
        /// <param name="isjpg">保存形式選択 False:bmp, True:Jpg</param>
        /// <returns>エラーコード</returns>
        public abstract bool SavePicture(string filepath, bool isjpg, int extime);

        /// <summary>
        /// 値変換_X:Pix ⇒ Mm
        /// </summary>
        /// <param name="pix">変換前の値</param>
        /// <returns>変換後の値</returns>
        public abstract double PixToMm_X(double pix);

        /// <summary>
        /// 値変換_Y:Pix ⇒ Mm
        /// </summary>
        /// <param name="pix">変換前の値</param>
        /// <returns>変換後の値</returns>
        public abstract double PixToMm_Y(double pix);

        /// <summary>
        /// 値変換_X:Mm ⇒ Pix
        /// </summary>
        /// <param name="mm">変換前の値</param>
        /// <returns>変換後の値</returns>
        public abstract double MmToPix_X(double mm);

        /// <summary>
        /// 値変換_Y:Mm ⇒ Pix
        /// </summary>
        /// <param name="mm">変換前の値</param>
        /// <returns>変換後の値</returns>
        public abstract double MmToPix_Y(double mm);

        #endregion

    }

}
