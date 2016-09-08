using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArtCamSdk;
using System.IO;
using System.IO.Ports;
using System.Xml;
using System.Drawing.Imaging;
using System.Management;
namespace SideSeal
{
   

    public partial class Form_Main : Form
    {
        // khai bao ve cong com giao tiep voi Barcode co dinh

        SerialPort P = new SerialPort(); // Khai báo 1 Object SerialPort mới.
        string InputData = String.Empty; // Khai báo string buff dùng cho hiển thị dữ liệu sau này.
        delegate void SetTextCallback(string text); // Khai bao delegate SetTextCallBack voi tham so string
        int counter_keydown = 0;
        int flag_ng = 0;

        // dinh nghia thong tin phan mem
        #region ソフトVer情報定義 

        //  【Version Table】
        //2014/06/12　Ver 1.00　新規リリース // phat hanh moi v1.00
        //2014/06/14　Ver 1.01　多重起動防止追加、ワークカウント機能追加 // them tinh nang dem san pham
        //2014/09/03　Ver 1.02　Form_Mainのプロパティ変更 // them tinh nang quyen truy cap cai dat
        //2015/01/05  Ver 2.00 Them ket noi voi barcode cam tay + them tinh nang lua chon che do su dung barcode Reader
        //                     Thay doi duong ke soc mau do + bacode reader co dinh ket noi USB thanh RS232                      
        /// <summary> ソフトVer情報 </summary>
        private const string _SOFT_VER = "2.00";

        #endregion


        #region 公開変数

        /// <summary> IniFile読み出し構造体 </summary> // giao tiep voi file
        public IniFileSetting IniFileSettingObj;
        /// <summary> バーコードリーダ通信クラスObj </summary>// giao tiep voi barcode
        public AccessBarcode AccessBarcodeObj;
        /// <summary> カメラ通信クラスObj </summary>// giao tiep voi camera
        public AccessCamera AccessCameraObj;



        #endregion

        #region 非公開変数 // khai bao cac bien private
        /// <summary> 左カメラ画像フォルダパス </summary> // bien luu duong dan thu muc anh camera trai
        private string _Picture_Left_FolderPath;
        /// <summary> 右カメラ画像フォルダパス </summary> // bien luu duong dan thu muc amh camera phai
        private string _Picture_Right_FolderPath;
        /// <summary> エラーログフォルダパス </summary>
        private string _Errorlog_FolderPath; // bien luu duong dan log loi
        /// <summary> カメラ取得終了動作_開始フラグ </summary>
        private bool _CameraCaptureEndFlag; // co bao ket thuc chup
        /// <summary> カメラ取得終了動作_完了フラグ </summary>
        private bool _CameraCaptureEndComleteFlag;// co bao came ra chup thanh cong

        private string _Past_Barcode; // chuoi barcode cu


        private object _SavePictureObj;
        private bool flag_reader;

        /// <summary> 基準線情報(生産) </summary> // duong tieu chuan 
        private List< GikaiLib.Components.DatumGraphics> _Datum_Process;
        /// <summary> 基準線情報(日常点検) </summary> // duong tieu chuan kiem tra
        private  List<GikaiLib.Components.DatumGraphics> _Datum_Check; 




        #endregion

        #region 非公開定数

        /// <summary> Mes点滅回数 </summary> // so lan bat tat flash
        private const int _FLASHCOUNT = 15; 
        /// <summary> Mes点滅次官 </summary>
        private const int _FLASHTIME = 100; // thoi gian bat tat flash

        #endregion


        #region　公開メソッド

       // /// <summary>
       // /// エラーログ追記メソッド
       // /// </summary>
       // /// <param name="error"></param>
       // public void WriteErrorLog(ErrorCode error)
       // {
       ////     string mes = "Error_" + ((int)error).ToString("000") + error.ToString();
       ////     AddLogMes(true, mes, Color.Red);
       // }


        #endregion
        
        
        #region　Ham hien thi gia tri barcode nhan tu cong com

        // ham ve cong RS232 bat du lieu tu barcode co dinh
        private void DataReceive(object obj, SerialDataReceivedEventArgs e)
        {
            InputData = P.ReadLine();
            if (InputData != String.Empty)
            {
                SetText(InputData);
            }
        }
        
       
        private void SetText(string text)
        {
            if (this.Lbl_Sn.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText); // khởi tạo 1 delegate mới gọi đến SetText
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.Lbl_Sn.Text = text;
            }
                
        }

        #endregion

        #region　非公開メソッド // phuong thuc bao mat

        /// <summary>
        /// ログの上部分を消去するメソッド // phuong thuc xoa bo phan dau tien cua log
        /// </summary>
        private void Log_ClearFirst()
        {
            // 現在のログの行数を取得する // thu thap so dong hien tai cua log
            int count = Rtb_Log.Lines.GetLength(0);

            // 行数が1000行を超えていれば、500行消去処理を行う。// neu so dong vuot qua 1000 thi loai bo 500 dong dau
            int maxnum = 1000;
            int removenum = 500;
            if (count > maxnum)
            {
                string[] templines = new string[removenum];
                for (int i = 0; i < removenum; i++) { templines[i] = Rtb_Log.Lines[i + (count - removenum)]; }
                Rtb_Log.Lines = templines;
            }
        }

        /// <summary>
        /// Logメッセージ追記 // ghi thong bao tren log
        /// </summary>
        /// <param name="isAddTime">時間追記有無</param>
        /// <param name="mes">メッセージ</param>
        /// <param name="mesColor">メッセージ色</param>
        private void AddLogMes(bool isAddTime, string mes, Color mesColor)
        {
            string writemes = "";

            // 現在時刻を追加 // cap nhat thoi gian hien tai
            if (isAddTime)
            {
                DateTime dt = DateTime.Now;
                writemes += dt.Hour.ToString("00"); // gio
                writemes += ":";
                writemes += dt.Minute.ToString("00"); // phut 
                writemes += ":";
                writemes += dt.Second.ToString("00");// giay
                writemes += "\t";
            }

            // メッセージ // thong bao
            writemes += mes;
            writemes += "\n";

            this.Invoke(new Action(() =>
            {
                Log_ClearFirst();// xoa phan dau cua log

                Rtb_Log.SelectionColor = mesColor;
                Rtb_Log.AppendText(writemes);
                
            }));
            
        }
        /// <summary>
        /// Logメッセージへのエラー追記 // ghi loi vao log
        /// </summary>
        /// <param name="code"></param>
        private void AddLogErrorCode(ErrorCode code)
        {
            // メッセージ追記
            string mes =
              "\tError_No." + code.ErrorNo.ToString("000") + "_" + code.ErrorStr + "\n" +
              "\tDetail." + code.DetailNo.ToString("000") + "_" + code.DetailStr;

            AddLogMes(false, mes, Color.Yellow);
            

            // Logへの保存 // luu vao log
            var dt = DateTime.Now;


            // フォルダが無い場合は、フォルダを自動生成 // truong hop khong co thu muc thi tao thu muc moi
            string erropath = Application.StartupPath + "\\" + "Error" + "\\" + dt.ToString("yyMM");
            if (!System.IO.Directory.Exists(erropath))
            {
                System.IO.Directory.CreateDirectory(erropath);
            }

            erropath += "\\" + dt.ToString("dd") + ".csv";
            // ファイルが無い場合は、ヘッダ生成
            if (!System.IO.File.Exists(erropath))
            {
                var sw_b = new System.IO.StreamWriter(erropath, true, Encoding.Default);
                sw_b.WriteLine("Date,Time,ErrorNo,ErrorStr,DetailNo,DetailStr,");
                sw_b.Close();
                sw_b.Dispose();
            }

               
            // ファイル名設定、オープン
            var sw = new System.IO.StreamWriter(erropath, true, Encoding.Default);
            sw.WriteLine(
            dt.ToString("yy") + "/" + dt.ToString("MM") + "/" + dt.ToString("dd") + "," +
            dt.ToString("HH") + ":" + dt.ToString("mm") + ":" + dt.ToString("ss") + "," +
            code.ErrorNo.ToString("000") + "," +
            code.ErrorStr + "," +
            code.DetailNo.ToString("000") + "," +
            code.DetailStr + ",");
            sw.Close();
            sw.Dispose();
           
        }

        /// <summary>
        /// 画像保存メソッド // phuong thuc luu anh
        /// </summary>
        private void SavePicture()
        {
            if (!System.Threading.Monitor.TryEnter(_SavePictureObj)) { return; }
            try
            {

                // 現在日時を取得 // doc thoi gian hien tai 
                DateTime date = DateTime.Now;

                // 日付フォルダ名作成 // tao thu muc voi ten theo thoi gian
                string datefoldername = date.ToString("yyMMdd");

                // フォルダが無い場合は、フォルダを自動生成 // trong truong hop khong co thu muc thi tao thu muc moi
                if (!System.IO.Directory.Exists(_Picture_Left_FolderPath + "\\" + datefoldername))
                {
                    System.IO.Directory.CreateDirectory(_Picture_Left_FolderPath + "\\" + datefoldername);
                }

                if (!System.IO.Directory.Exists(_Picture_Right_FolderPath + "\\" + datefoldername))
                {
                    System.IO.Directory.CreateDirectory(_Picture_Right_FolderPath + "\\" + datefoldername);
                }

                // バーコードリード実行 // thuc hien ban barcode
                string barcode;
                ErrorCode code = new ErrorCode();
                code.Clear();

                this.Invoke(new Action(() =>
                {
                    Lbl_Mes.Text = "Đang đọc barcode...";
                }));

                ERROR_COMMON errcom;
                errcom = ERROR_COMMON.Reader_Read;
                AddLogMes(true, errcom.ToString(), Color.White);
                if(!flag_reader)
                {
                    barcode="";
                    try
                    {
                        P.DiscardInBuffer();
                        P.Write(System.Text.Encoding.ASCII.GetString(new byte[] { 0x1B }) + "A0.02\r");
                    }
                    catch
                    {
                        code.ErrorNo = (int)AccessBarcode.ERROR.READER_WRITE;
                        code.ErrorStr = AccessBarcode.ERROR.READER_WRITE.ToString();
                        AddLogErrorCode(code);
                        return;
                    }
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            timer_barcode.Enabled = true;
                        }));
                        barcode = P.ReadLine();// doc tu bo dem
                        this.Invoke(new Action(() =>
                        {
                            timer_barcode.Enabled = false;
                        }));
                        barcode = barcode.Remove(0, 1); // Xoa byte dau
                    }
                    catch
                    {
                        code.ErrorNo = (int)AccessBarcode.ERROR.READER_READ;
                        code.ErrorStr = AccessBarcode.ERROR.READER_READ.ToString();
                        AddLogErrorCode(code);
                        return;
                    }
                
                }
                else
                {
                    barcode=Lbl_Sn.Text;

                }

                    // 読取失敗の場合、エラーメッセージにて終了 // trong truong hop doc bi loi, ket thu bang 1 massage loi
                    if (barcode == "")
                    {
                        errcom = ERROR_COMMON.NG_Read_Barcode;
                        code.ErrorNo = (int)errcom;
                        code.ErrorStr = errcom.ToString();
                        code.DetailStr = barcode;
                        AddLogErrorCode(code);

                        this.Invoke(new Action(() =>
                        {
                            Lbl_Mes.Text = SelectMessage.NG_Read_Barcode.ToString();
                        }));

                        for (int i = 0; i < _FLASHCOUNT; i++)
                        {
                            this.Invoke(new Action(() => Pnl_Mes.BackColor = Color.Red));
                            System.Threading.Thread.Sleep(_FLASHTIME);
                            this.Invoke(new Action(() => Pnl_Mes.BackColor = Color.LightCoral));
                            System.Threading.Thread.Sleep(_FLASHTIME);
                        }

                        this.Invoke(new Action(() =>
                        {
                            Pnl_Mes.BackColor = Color.PaleGreen;
                            switch (cb_select.SelectedIndex)
                            {
                                case 0:
                                    Lbl_Sn.ReadOnly = true;
                                    Lbl_Mes.Text = "Hãy Ấn Enter";// SelectMessage.Press_Enter.ToString();
                                    break;
                                case 1:
                                    Lbl_Sn.ReadOnly = true;
                                    Lbl_Mes.Text = "Sử dụng barcode cầm tay";
                                    break;
                                case 2:
                                    Lbl_Sn.ReadOnly = true;
                                    Lbl_Mes.Text = "Hãy Ấn Enter";//SelectMessage.Press_Enter.ToString();
                                    break;
                            }
                            Lbl_Sn.Enabled = true;
                        }));
                        return;
                    }


                // Log追記 // ghi vao log
                AddLogMes(false, "\t" + barcode.ToString(), Color.White);

                // ファイル名生成 //tao file
                string picfilename = barcode + "_" + date.ToString("HHmmss") + ".jpg";

                Bitmap bmp;
                

                this.Invoke(new Action(() =>
                {
                    bmp = (Bitmap)Pbx_RightCamera.Image.Clone();
                    bmp.Save(_Picture_Right_FolderPath + "\\" + datefoldername + "\\" + picfilename, System.Drawing.Imaging.ImageFormat.Jpeg);


                    bmp = (Bitmap)Pbx_LeftCamera.Image.Clone();
                    bmp.Save(_Picture_Left_FolderPath + "\\" + datefoldername + "\\" + picfilename, System.Drawing.Imaging.ImageFormat.Jpeg);
                }));

                // バーコードが過去と等しい場合はSame SerialNo表示 // truong hop giong barcode cu hien thi same serial
                if (barcode == _Past_Barcode)
                {
                    this.Invoke(new Action(() =>
                    {
                        switch (cb_select.SelectedIndex)
                        {
                            case 0:
                                Lbl_Sn.ReadOnly = true;
                                Lbl_Mes.Text = "Hãy Ấn Enter";//SelectMessage.Press_Enter.ToString();
                                break;
                            case 1:
                                Lbl_Sn.ReadOnly = false;
                                Lbl_Mes.Text = "Sử dụng barcode cầm tay";
                                break;
                            case 2:
                                Lbl_Sn.ReadOnly = true;
                                Lbl_Mes.Text = "Hãy Ấn Enter";//SelectMessage.Press_Enter.ToString();
                                break;
                        }
                        
                        Lbl_Mes.Text = "Giống với mã cũ";
                        Lbl_Sn.Text = barcode;
                        Lbl_Sn.ReadOnly = true;
                    }));

                    for (int i = 0; i < _FLASHCOUNT; i++)
                    {
                        this.Invoke(new Action(() => Pnl_Mes.BackColor = Color.Yellow));
                        System.Threading.Thread.Sleep(_FLASHTIME);
                        this.Invoke(new Action(() => Pnl_Mes.BackColor = Color.Orange));
                        System.Threading.Thread.Sleep(_FLASHTIME);
                    }
                }

                // バーコードが過去と異なる場合はOK表示 // truong hop khac voi barcode cu hien thi OK
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        Lbl_Mes.Text = SelectMessage.OK.ToString();
                        Lbl_Sn.Text = barcode;

                        // ワークカウント加算(Ver1.01) // tinh toan san pham
                        WorkCountAdd();
                        Lbl_Sn.ReadOnly = true;
                    }));

                    for (int i = 0; i < _FLASHCOUNT; i++)
                    {
                        this.Invoke(new Action(() => Pnl_Mes.BackColor = Color.Aquamarine));
                        System.Threading.Thread.Sleep(_FLASHTIME);
                        this.Invoke(new Action(() => Pnl_Mes.BackColor = Color.Blue));
                        System.Threading.Thread.Sleep(_FLASHTIME);
                    }

                }

                this.Invoke(new Action(() =>
                {
                    Pnl_Mes.BackColor = Color.PaleGreen;
                    switch (cb_select.SelectedIndex)
                    { 
                        case 0:
                            Lbl_Mes.Text = "Hãy Ấn Enter";//SelectMessage.Press_Enter.ToString();
                            break;
                        case 1:
                            Lbl_Mes.Text = "Sử dụng Barcode cầm tay";
                            break;
                        case 2:
                            Lbl_Mes.Text = "Hãy Ấn Enter";//SelectMessage.Press_Enter.ToString();
                            break;
                    }
                }));
                Invoke(new Action(() =>
                {
                    timer_clear.Enabled = true;
                    switch (cb_select.SelectedIndex)
                    {
                        case 0:
                            Lbl_Sn.ReadOnly = true;
                            break;
                        case 1:
                            Lbl_Sn.ReadOnly = false;
                            break;
                        case 2:
                            Lbl_Sn.ReadOnly = true;
                            break;

                    }
                }));
                // バーコードの値を保存 // luu gia tri cua barcode
                _Past_Barcode = barcode;
            }
           finally
            {
                System.Threading.Monitor.Exit(_SavePictureObj);
            }


        }

        /// <summary>
        /// カメラ定期取得メソッド // phuong thuc chup dinh ki camera
        /// </summary>
        private void CameraWatch_Monitor()
        {

            _CameraCaptureEndFlag = false;
            _CameraCaptureEndComleteFlag = false;

            while (!_CameraCaptureEndFlag)
            {
                if (AccessCameraObj.IsOpen)
                {
                    byte[] rcamera, lcamera;

                    AccessCameraObj.Capture(out lcamera, out rcamera);

                    //左カメラ生画像表示
                    try
                    {
                        Bitmap bmp = new Bitmap(AccessCamera.PIC_WIDTH, AccessCamera.PIC_HEIGHT, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                        // short配列にデータをマーシャリング
                        int width = AccessCamera.PIC_WIDTH;
                        int height = AccessCamera.PIC_HEIGHT;
                        int length = width * height * 3;

                        // bitmapに圧縮データをコピー
                        var rect = new Rectangle(0, 0, width, height);
                        System.Drawing.Imaging.BitmapData bdata = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);
                        System.Runtime.InteropServices.Marshal.Copy(lcamera, 0, bdata.Scan0, length);
                        bmp.UnlockBits(bdata);

                        Invoke(new Action(() =>
                        {
                            //  Pbx_LeftCamera.Image = new Bitmap(bmp, Pbx_LeftCamera.Width, Pbx_LeftCamera.Height);
                            try { Pbx_LeftCamera.Image = bmp; }
                            catch { }
                        }));
                    }
                    catch { }

                    //右カメラ生画像表示
                    try
                    {
                        Bitmap bmp = new Bitmap(AccessCamera.PIC_WIDTH, AccessCamera.PIC_HEIGHT, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                        // short配列にデータをマーシャリング
                        int width = AccessCamera.PIC_WIDTH;
                        int height = AccessCamera.PIC_HEIGHT;
                        int length = width * height * 3;

                        // bitmapに圧縮データをコピー
                        var rect = new Rectangle(0, 0, width, height);
                        System.Drawing.Imaging.BitmapData bdata = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);
                        System.Runtime.InteropServices.Marshal.Copy(rcamera, 0, bdata.Scan0, length);
                        bmp.UnlockBits(bdata);

                        Invoke(new Action(() =>
                        {
                            //    Pbx_RightCamera.Image = new Bitmap(bmp, Pbx_RightCamera.Width, Pbx_RightCamera.Height);
                            try { Pbx_RightCamera.Image = bmp; }
                            catch { }
                        }));

                    }
                    catch { }


                }

                System.Threading.Thread.Sleep(10);
            }

            _CameraCaptureEndComleteFlag = true;

        }

        #endregion

        #region イベントハンドラ_Form_Timer関連

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form_Main()
        {


            InitializeComponent();

            // thong so cai dat cho cong COM1


            //P.PortName = IniFileSettingObj.Barcode_Com;
            ////   P.PortName = "COM1";
            //P.BaudRate = 9600;
            //P.DataBits = 8;
            //P.Parity = Parity.None;
            //P.StopBits = StopBits.One;
            //if (!P.IsOpen)
            //{
            //    P.Open();
            //    P.RtsEnable = true;

            //}
            //chon che do doc barcode
            string[] Reader = { "Barcode cố định", "Barcode cầm tay", "Sử dụng cả 2" };
            cb_select.Items.AddRange(Reader);
          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer_focus.Enabled = true;
            this.Location = new Point(0, 0);
            this.Size = new System.Drawing.Size(1280, 1024);
           
            ERROR_COMMON err_com;
            ErrorCode code;


            
            // 過去バーコード情報初期化
            _Past_Barcode = "";

            // 進行表示をエラー表示化
            Pnl_Mes.BackColor = Color.Red;
            Lbl_Mes.Text = SelectMessage.NG_MachineState.ToString();
            Lbl_Sn.Text = "";


            // Ver表示
            Lbl_Ver.Text = _SOFT_VER;

            //// 基準線描画コンポーネントの関連付け
            Dpb_RightCamera.SetParentControl(Pbx_RightCamera);
            Dpb_LeftCamera.SetParentControl(Pbx_LeftCamera);
            Dpb_RightCamera.GraphicsList = new List<GikaiLib.Components.DatumGraphics>();
            Dpb_LeftCamera.GraphicsList = new List<GikaiLib.Components.DatumGraphics>();

            // IniFile読み出し
            IniFileSettingObj = new IniFileSetting();
            IniFileSettingObj.IniFilePath = Application.StartupPath + "\\Setting.ini";

            err_com = ERROR_COMMON.Read_IniFile;
            AddLogMes(true, err_com.ToString(), Color.White);
            if (!IniFileSettingObj.ReadIniFile())
            {
                AddLogErrorCode(new ErrorCode((int)err_com, err_com.ToString(), 0, ""));
            }

            P.PortName = IniFileSettingObj.Barcode_Com;
            //   P.PortName = "COM1";
            P.BaudRate = 9600;
            P.DataBits = 8;
            P.Parity = Parity.None;
            P.StopBits = StopBits.One;
            if (!P.IsOpen)
            {
                P.Open();
                P.RtsEnable = true;

            }
           // cb_select.SelectedIndex = 2; // che do su dung ca 2 barcode
            switch (IniFileSettingObj.mode)
            {
                case "Fix":
                    cb_select.SelectedIndex = 0;
                    break;
                case "Handy":
                    cb_select.SelectedIndex = 1;
                    break;
                case "Both":
                    cb_select.SelectedIndex = 2;
                    break;
            }

            // IniFile情報を、Setting画面に反映
            Btn_Setting_ClearValue_Click(null, null);

            // Count情報を反映(Ver1.01追加)
            Txt_WorkCount.Text = IniFileSettingObj.WorkCountNo.ToString("0");



            // アプリケーションのStartUpパス取得
            _Picture_Left_FolderPath = Application.StartupPath + "\\Picture_Left";
            _Picture_Right_FolderPath = Application.StartupPath + "\\Picture_Right";

            // フォルダが無い場合は、フォルダを自動生成
            if (!System.IO.Directory.Exists(_Picture_Left_FolderPath))
            {
                System.IO.Directory.CreateDirectory(_Picture_Left_FolderPath);
            }
            if (!System.IO.Directory.Exists(_Picture_Right_FolderPath))
            {
                System.IO.Directory.CreateDirectory(_Picture_Right_FolderPath);
            }

            // 基準線の初期化
            Cbx_CangeLine_CheckedChanged(null, null);
            Tmr_Timer.Enabled = true;
           
            // インスタンス初期化

            AccessBarcodeObj = new AccessBarcode();
            AccessCameraObj = new AccessCamera();

            
            
            // モニタタイマ開始
            Tmr_Timer.Enabled = true;


            // バーコードリーダ初期化
           // err_com = ERROR_COMMON.Reader_Open;
           // AddLogMes(true, err_com.ToString(), Color.White);
           // code = AccessBarcodeObj.Open(IniFileSettingObj.Barcode_Com);
           //if (code.ErrorNo != (int)AccessBarcode.ERROR.READER_NONE)
           //{
           //    AddLogErrorCode(code);
           //     return;
           // }

            // カメラObj初期化
            err_com = ERROR_COMMON.Camera_Open;
            AddLogMes(true, err_com.ToString(), Color.White);
            code = AccessCameraObj.Open(this.Handle, IniFileSettingObj.R_Cam_Gain, IniFileSettingObj.L_Cam_Gain);
            if (code.ErrorNo != (int)AccessBarcode.ERROR.READER_NONE)
            {
                AddLogErrorCode(code);
                return;
            }
            
          
            // カメラ定期モニタ実行
            new MethodInvoker(CameraWatch_Monitor).BeginInvoke(null, null);

            // 進行表示初期化
            Pnl_Mes.BackColor = Color.PaleGreen;
           // Lbl_Mes.Text = "Hãy ấn Enter";//SelectMessage.Press_Enter.ToString();

            _SavePictureObj = new object();
           
           
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            //// カメラ解放
            //if (AccessCameraObj.IsOpen)
            //{
            //    _CameraCaptureEndFlag = true;

            //    while (!_CameraCaptureEndComleteFlag)
            //    {
            //        System.Threading.Thread.Sleep(10);
            //    }

            //    AccessCameraObj.Close();
            //}

            //this.Close();
            P.Close();

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                switch (cb_select.SelectedIndex)
                { 
                    case 0://doc barcode co dinh
                        Lbl_Sn.ReadOnly = true;
                        if(counter_keydown<21)
                        {
                            counter_keydown = 0;
                            flag_reader = false;
                            new MethodInvoker(SavePicture).BeginInvoke(null, null);
                        }
                        break;
                    case 1:// doc barcode cam tay

                        if (counter_keydown >= 20)
                        {
                            counter_keydown = 0;
                            flag_reader = true;
                            new MethodInvoker(SavePicture).BeginInvoke(null, null);
                        }
                        break;
                    case 2:// doc ca 2 barcode
                        if (counter_keydown < 20)
                        {
                            if (Lbl_Sn.Text != "")
                            {
                                flag_reader = true;
                            }
                            else
                            {
                                flag_reader = false;
                                Invoke(new Action(() =>
                                {
                                    Lbl_Sn.Text = "";
                                }));
                            }
                            counter_keydown = 0;
                            new MethodInvoker(SavePicture).BeginInvoke(null, null);
                        }
                        else
                        {
                            counter_keydown = 0;
                            if (Lbl_Sn.Text != "")
                            {
                                flag_reader = true;
                                new MethodInvoker(SavePicture).BeginInvoke(null, null);
                            }
                        }
                        break;
                }
            }
  
            else
            {
                counter_keydown++;
            }
                       
        }
        // danh sach cac usb duoc ket noi
        private List<USBDeviceInfo> GetUSBDevices()
        {
            List<USBDeviceInfo> devices = new List<USBDeviceInfo>();
            ManagementObjectCollection colection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_Keyboard"))
            colection = searcher.Get();
            foreach (var device in colection)
            {
                devices.Add(new USBDeviceInfo((string)device.GetPropertyValue("DeviceID")));
            }
            colection.Dispose();
            return devices;
        }
        // dinh nghia lop USBDeviceInfo
        class USBDeviceInfo
        {
            public USBDeviceInfo(string deviceID)
            {
                this.DeviceID = deviceID;
            }
            public String DeviceID { get; private set; }
            
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            int er = 0;
            // kiem tra barcode co dinh
            if (P.CtsHolding)
            {
                Pnl_State_Fix_Reader.BackColor = Color.Lime;
            }
            else
            {
                Pnl_State_Fix_Reader.BackColor = Color.Red;
                er = 1;
            }
            // 機器状態モニタ実行
            {
                if (AccessCameraObj.IsOpen)
                {
                    Pnl_State_Rcamera.BackColor = Color.Lime;
                    Pnl_State_Lcamera.BackColor = Color.Lime;
                }
                else
                {
                    Pnl_State_Rcamera.BackColor = Color.Red;
                    Pnl_State_Lcamera.BackColor = Color.Red;
                    er = 1;
                }

            }
            // kiem tra barcode cam tay
            var usbDevices = GetUSBDevices();
            int find = 0;// trang thai cua bacode cam tay
            foreach (var usbDevice in usbDevices)
            {
                if (usbDevice.DeviceID.IndexOf("VID_0745") == 4)// VID cua barcode cam tay : 0745
                {
                    find = 1;
                }
            }
            if (find == 1)
                // neu tim thay thi hien thi mau xanh
                Pnl_State_Reader.BackColor = Color.Lime;
            else
                {
                // neu khong tim thay thi mau do
                Pnl_State_Reader.BackColor = Color.Red;
                er = 1;
                }
            if (er==1)
            {
                
                Pnl_Mes.BackColor = Color.Red;
                Lbl_Mes.Text = "Máy đang lỗi";
            }
            

            // 現在時刻更新
            {
                DateTime dt = DateTime.Now;

                string day = dt.Year.ToString("0000");
                day += "/";
                day += dt.Month.ToString("00");
                day += "/";
                day += dt.Day.ToString("00");

                string time = dt.Hour.ToString("00");
                time += ":";
                time += dt.Minute.ToString("00");
                time += ":";
                time += dt.Second.ToString("00");
                Lbl_Date.Text = day;
                Lbl_Time.Text = time;
            }

            // パスワード画面_画面色表示
            {
                MaskedTextBox tbx;
                int intval;
                bool res;

                tbx = Mtb_Setting_BarcodeComNo;
                if (tbx.Text == IniFileSettingObj.Barcode_Com) { tbx.BackColor = Color.Aquamarine; }
                else { tbx.BackColor = Color.Violet; }

                tbx = Mtb_Setting_Lcamgain;

                res = int.TryParse(tbx.Text, out intval);
                if (intval == IniFileSettingObj.L_Cam_Gain && res) { tbx.BackColor = Color.Aquamarine; }
                else { tbx.BackColor = Color.Violet; }


                tbx = Mtb_Setting_Rcamgain;
                res = int.TryParse(tbx.Text, out intval);
                if (intval == IniFileSettingObj.R_Cam_Gain && res) { tbx.BackColor = Color.Aquamarine; }
                else { tbx.BackColor = Color.Violet; }
            }
        }

        #endregion

        #region イベントハンドラ_その他(Btn, Text, MenuStrip) 
        // cai dat cac tham so
        private void Smi_Setting_Click(object sender, EventArgs e)
        {
            Mtb_Setting_PassWord.Text = "";
            Gbx_Pass.Visible = false;
            Gbx_Setting.Visible = true;
        }
        // thoat khoi cai dat
        private void Btn_Setting_Close_Click(object sender, EventArgs e)
        {
            Mtb_Setting_PassWord.Text = "";
            Gbx_Pass.Visible = false;
            Gbx_Setting.Visible = false;
            Lbl_Sn.Focus();
        }

        
        private void Mtb_Setting_PassWord_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }
        // kiem tra password
        private void Mtb_Setting_PassWord_TextChanged(object sender, EventArgs e)
        {
            if (Mtb_Setting_PassWord.Text == "nanling") { Gbx_Pass.Visible = true; }
            else { Gbx_Pass.Visible = false; }


        }

        private void Btn_Setting_ClearValue_Click(object sender, EventArgs e)
        {

            Mtb_Setting_BarcodeComNo.Text = IniFileSettingObj.Barcode_Com;
            Mtb_Setting_Lcamgain.Text = IniFileSettingObj.L_Cam_Gain.ToString("0");
            Mtb_Setting_Rcamgain.Text = IniFileSettingObj.R_Cam_Gain.ToString("0");
        }

        #endregion

        private void Btn_Setting_SaveValue_Click(object sender, EventArgs e)
        {
            // 値読み出し
            int r_camgain,l_camgain;
            string comno;
            bool res;

            res = int.TryParse(Mtb_Setting_Rcamgain.Text, out r_camgain);
            if (!res) { r_camgain = 1; }

            res = int.TryParse(Mtb_Setting_Lcamgain.Text, out l_camgain);
            if (!res) { l_camgain = 1; } 

            comno = Mtb_Setting_BarcodeComNo.Text;

            // 値制限
            if (r_camgain > 63){ r_camgain = 63;}
            if (l_camgain > 63) { l_camgain = 63; }

            // 値の書き込み
            IniFileSettingObj.Barcode_Com = comno;
            IniFileSettingObj.L_Cam_Gain = l_camgain;
            IniFileSettingObj.R_Cam_Gain = r_camgain;
            //IniFileSettingObj.mode = //cb_select.Text[cb_select.SelectedIndex];
            switch (cb_select.SelectedIndex)
            {
                case 0:
                    IniFileSettingObj.mode = " Fix";
                    break;
                case 1:
                    IniFileSettingObj.mode = "Handy";
                    break;
                case 2:
                    IniFileSettingObj.mode = "Both";
                    break;
            }

            ERROR_COMMON err_com;
            ErrorCode code;

            // IniFile読み出し
            err_com = ERROR_COMMON.Write_IniFile;
            AddLogMes(true, err_com.ToString(), Color.White);
            if (!IniFileSettingObj.SaveIniFile())
            {
                AddLogErrorCode(new ErrorCode((int)err_com, err_com.ToString(), 0, ""));
            }

            // カメラゲイン反映
            err_com = ERROR_COMMON.Camera_ChangeGain;
            AddLogMes(true, err_com.ToString(), Color.White);
            code = AccessCameraObj.ChangeGain(r_camgain, l_camgain);
            if (code.ErrorNo != (int)AccessBarcode.ERROR.READER_NONE)
            {
                AddLogErrorCode(code);
                return;
            }

            // 値の画面反映
            Mtb_Setting_BarcodeComNo.Text = IniFileSettingObj.Barcode_Com;
            Mtb_Setting_Lcamgain.Text = IniFileSettingObj.L_Cam_Gain.ToString("0");
            Mtb_Setting_Rcamgain.Text = IniFileSettingObj.R_Cam_Gain.ToString("0");
        }

        private void Mtb_Setting_Rcamgain_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {




        }

        private void Cbx_CangeLine_CheckedChanged(object sender, EventArgs e)
        {

            // 生産状態基準線への切り替え
            if (!Cbx_CangeLine.Checked)
            {
                var graphiclist = new List<GikaiLib.Components.DatumGraphics>();

                for (int i = 1; i <= 72; i++)
                {
                    Pen p;
                    if (i % 4 == 0) { p = new Pen(Color.Red); }
                    else { p = new Pen(Color.Blue); }

                    p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    graphiclist.Add(new GikaiLib.Components.DatumLineVer(p, i * 8));
                }

                Dpb_RightCamera.GraphicsList = graphiclist;
                Dpb_LeftCamera.GraphicsList = graphiclist;

                Dpb_RightCamera.DrawDatum();
                Dpb_LeftCamera.DrawDatum();
            }

            // 校正状態基準線への切り替え
            else
            {
                var graphiclist = new List<GikaiLib.Components.DatumGraphics>();


                Pen p=new Pen(Color.Red); 
                                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                Rectangle rect = new Rectangle();
                rect.X = Pbx_LeftCamera.Width/5*2;
                rect.Y = Pbx_LeftCamera.Height/5*2;
                rect.Width = Pbx_LeftCamera.Width/5;
                rect.Height = Pbx_LeftCamera.Height/5;

                graphiclist.Add(new GikaiLib.Components.DatumRectangle(p,rect));

                Dpb_RightCamera.GraphicsList = graphiclist;
                Dpb_LeftCamera.GraphicsList = graphiclist;

                Dpb_RightCamera.DrawDatum();
                Dpb_LeftCamera.DrawDatum();

            }

        }

        private void panel22_Paint(object sender, PaintEventArgs e)
        {

        }

        /// <summary>
        /// ワークカウントクリア(Ver1.01追加)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Work_Count_Clear_Click(object sender, EventArgs e)
        {
            IniFileSettingObj.WorkCountNo = 0;
            IniFileSettingObj.SaveIniFile();
            Txt_WorkCount.Text = IniFileSettingObj.WorkCountNo.ToString("0");
        }

        /// <summary>
        /// ワークカウント加算メソッド(Ver1.01追加)
        /// </summary>
        private void WorkCountAdd()
        {
            IniFileSettingObj.WorkCountNo++;
            IniFileSettingObj.SaveIniFile();
            Txt_WorkCount.Text = IniFileSettingObj.WorkCountNo.ToString("0");
        }

        private void Pbx_LeftCamera_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Gbx_Pass_Enter(object sender, EventArgs e)
        {

        }

        private void Gbx_Setting_Enter(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label38_Click(object sender, EventArgs e)
        {

        }

        private void Lbl_Date_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void Rtb_Log_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer_barcode_Tick(object sender, EventArgs e)
        {
            timer_barcode.Enabled = false;
            Tmr_Timer.Enabled = false;
            P.Close();  
            this.Invoke(new Action(() =>
            {
                Pnl_Mes.BackColor = Color.Red;
                Lbl_Mes.Text = "Barcode không đọc được";
                Lbl_Sn.ReadOnly = true;
                Lbl_Sn.Text = "";
                timer_ketnoi.Enabled = true;
                Tim_clear.Enabled = true;
            }));
            return;  
        }

        private void Pnl_Mes_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //ErrorCode cod = new ErrorCode();
            //string value;
            //cod = AccessBarcodeObj.Read(out value);
            //SetText ( value);
            
        }

        private void Pbx_Brother_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void Pbx_RightCamera_Click(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void Pnl_State_Reader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void Lbl_Sn_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        // timer xoa 
        private void timer_clear_Tick(object sender, EventArgs e)
        {
            Lbl_Sn.Text = "";
            Lbl_Sn.Enabled = false;
            timer_clear.Enabled = false;
            switch (cb_select.SelectedIndex)
            {
                case 0:
                    Lbl_Sn.ReadOnly = true;
                    break;
                case 1:
                    Lbl_Sn.ReadOnly = false;
                    Lbl_Sn.Enabled = true;
                    Lbl_Sn.Focus();
                    break;
                case 2:
                    Lbl_Sn.ReadOnly = true;
                    break;

            }
        }
        // timer focus 
        private void timer_focus_Tick(object sender, EventArgs e)
        {
            Lbl_Sn.Focus();
            timer_focus.Enabled = false;
            Lbl_Sn.ReadOnly = true;
        }
        // timer ketnoi voi cong com
        private void timer_ketnoi_Tick(object sender, EventArgs e)
        {
            timer_ketnoi.Enabled = false;
            P.Open();
            Tmr_Timer.Enabled = true;
        }
        // lua chon che do
        private void cb_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cb_select.SelectedIndex)
            {
                case 0: 
                    Lbl_Sn.ReadOnly = true;
                    Lbl_Mes.Text = "Hãy Ấn Enter";
                    break;
                case 1:
                    Lbl_Sn.ReadOnly = false;
                    Lbl_Mes.Text = "Sử dụng barcode cầm tay";
                    Lbl_Sn.Enabled = true;
                    break;
                case 2:
                    Lbl_Mes.Text = " Hãy Ấn Enter";
                    Lbl_Sn.ReadOnly = true;
 
                    break;
            
            }
        }
        private void Tim_clear_Tick(object sender, EventArgs e)
        {
            Tim_clear.Enabled = false;
            Pnl_Mes.BackColor = Color.PaleGreen;
            Lbl_Sn.Enabled = true;
            switch (cb_select.SelectedIndex)
            {
                case 0:
                    Lbl_Sn.ReadOnly = true;
                    Lbl_Mes.Text = "Hãy Ấn Enter";
                    break;
                case 1:
                    Lbl_Sn.ReadOnly = false;
                    Lbl_Mes.Text = "Sử dụng barcode cầm tay";
                    break;
                case 2:
                    Lbl_Mes.Text = " Sử dụng barcode cầm tay";
                    Lbl_Sn.ReadOnly = false;
                    break;

            }

            Lbl_Sn.Focus();
            

        }

        

        
    }
}
