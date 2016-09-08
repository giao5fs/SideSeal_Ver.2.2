using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SideSeal
{
  public  class AccessBarcode
  {
      #region　公開変数

      public bool IsOpen
      {
          get
          {
              if (_FixBarcodePort != null)
              {
                   
                  return _FixBarcodePort.IsOpen;
              }
              else
              {
                  return false;
              }
          }
      }


      #endregion


      #region　非公開変数

      /// <summary> Fix Barcode Reader 通信ポート </summary>
      private System.IO.Ports.SerialPort _FixBarcodePort;
      /// <summary> Handy Barcode Reader 通信ポート </summary>
      private System.IO.Ports.SerialPort _HandBarcodePort;

      #endregion



      #region 構造体、列挙型

      /// <summary>
      /// エラー列挙型
      /// </summary>
      public enum ERROR : uint
      {
          READER_NONE = 0,
          READER_OPEN = 101,
          READER_CLOSE = 102,
          READER_READ = 103,
          READER_WRITE = 104,
          READER_TIMEOUT = 198,
          READER_UNKNOWN = 199,
      }

      #endregion

      /// <summary>
      /// コンストラクタ
      /// </summary>
      public AccessBarcode()
      {
          
          _FixBarcodePort = new System.IO.Ports.SerialPort();
          _FixBarcodePort.ReadTimeout = 2000;
         // _BarcodePort.DataReceive += new System.IO.Ports.SerialDataReceivedEventHandler(DataReceive);
          
      }

      private void DataReceive(object obj, System.IO.Ports.SerialDataReceivedEventArgs e)
      {

      }

      /// <summary>
      /// オープン
      /// </summary>
      /// <param name="portname"></param>
      /// <returns></returns>
      public ErrorCode Open(string portname)
      {
          ERROR er = ERROR.READER_UNKNOWN;
          ErrorCode code = new ErrorCode();
          code.Clear();

          _FixBarcodePort.PortName = portname;

          _FixBarcodePort.BaudRate = 115200;
          _FixBarcodePort.DataBits = 8;
          _FixBarcodePort.Parity = System.IO.Ports.Parity.None;
          _FixBarcodePort.StopBits = System.IO.Ports.StopBits.One;
          try { _FixBarcodePort.Open(); }
          catch 
          {
              er = ERROR.READER_OPEN;
              code.ErrorNo = (int)er;
              code.ErrorStr = er.ToString();
              return code;
          }

          er = ERROR.READER_NONE;
          code.ErrorNo = (int)er;
          code.ErrorStr = er.ToString();
          _FixBarcodePort.RtsEnable = true;
         

          return code;
      }

      /// <summary>
      /// 読取
      /// </summary>
      /// <param name="portname"></param>
      /// <returns></returns>
      public   ErrorCode Read(out string serial)
      {


          ERROR er = ERROR.READER_UNKNOWN;
          ErrorCode code = new ErrorCode();
          code.Clear();

          serial = "";
          //try
          //{
          //    _BarcodePort.DiscardInBuffer();
          //    _BarcodePort.WriteLine(System.Text.Encoding.ASCII.GetString(new byte[] { 0x1B }) + "A0.02\r");
          //}

          //catch
          //{
          //    er = ERROR.READER_WRITE;
          //    code.ErrorNo = (int)er;
          //    code.ErrorStr = er.ToString();
          //    return code;
          //}

          try
          {
              serial = _FixBarcodePort.ReadLine();

              // フッタ切り取り // xoa byte dau va byte cuoi

          }
          catch
          {
              er = ERROR.READER_READ;
              code.ErrorNo = (int)er;
              code.ErrorStr = er.ToString();
              return code;
          }

          er = ERROR.READER_NONE;
          code.ErrorNo = (int)er;
          code.ErrorStr = er.ToString();
          return code;

      }

      /// <summary>
      /// 終了
      /// </summary>
      /// <returns></returns>
      public void  Close()
      {
         _FixBarcodePort.Close(); 
         
      }
      public bool Write_C(string tp)
      {
          _FixBarcodePort.Write(tp);
          return true;
      }

    }
}
