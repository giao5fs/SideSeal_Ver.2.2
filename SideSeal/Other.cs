using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SideSeal
{
    /// <summary>
    /// IniFile設定値
    /// </summary>
    public struct IniFileSetting
    {
        /// <summary> IniFileパス </summary>
        public string IniFilePath;


        /// <summary> バーコードCOM </summary>
        public string Barcode_Com;
        /// <summary> 左カメラゲイン値 </summary>
        public int L_Cam_Gain;
        /// <summary> 右カメラゲイン値 </summary>
        public int R_Cam_Gain;
        /// <summary> ワークカウント回数(Ver1.01追加) </summary>
        public int WorkCountNo;
        /// <summary>
        public string mode;
        /// </summary>
        
        /// <summary>
        /// IniFile読み出しメソッド
        /// </summary>
        /// <returns></returns>
        public bool ReadIniFile()
        {

            Barcode_Com = IniFile.ReadString("Setting", "Barcode_Com", "", IniFilePath);
            if (Barcode_Com == "") { return false; }

            L_Cam_Gain = IniFile.ReadInteger("Setting", "L_Cam_Gain", -1, IniFilePath);
            if (L_Cam_Gain == -1) { return false; }

            R_Cam_Gain = IniFile.ReadInteger("Setting", "R_Cam_Gain", -1, IniFilePath);
            if (R_Cam_Gain == -1) { return false; }

            //(Ver1.01追加)
            WorkCountNo = IniFile.ReadInteger("Setting", "WorkCountNo", -1, IniFilePath);
            if (WorkCountNo == -1) { return false; }


            mode = IniFile.ReadString("Setting", "Mode", "", IniFilePath);
            if (mode == "") { return false; }

            return true;
        }

        /// <summary>
        /// IniFile保存メソッド
        /// </summary>
        /// <returns></returns>
        public bool SaveIniFile()
        {
            if (!IniFile.WriteString("Setting", "Barcode_Com", Barcode_Com, IniFilePath)) { return false; }
            if (!IniFile.WriteInteger("Setting", "L_Cam_Gain", L_Cam_Gain, IniFilePath)) { return false; }
            if (!IniFile.WriteInteger("Setting", "R_Cam_Gain", R_Cam_Gain, IniFilePath)) { return false; }
            if (!IniFile.WriteInteger("Setting", "WorkCountNo", WorkCountNo, IniFilePath)) { return false; }
            if (!IniFile.WriteString("Setting", "Mode", mode, IniFilePath)) { return false; }
            return true;
        }
    }

    /// <summary>
    /// 画面メッセージ列挙型
    /// </summary>
    public enum SelectMessage
    {
        NG_MachineState,
        NG_Read_Barcode,
        Same_SerialNo,
        Press_Enter,
        OK,
        Reading,
    }

    public struct ErrorCode
    {
        public int ErrorNo;
        public string ErrorStr;
        public int DetailNo;
        public string DetailStr;


        public ErrorCode(int errorNo, string errorStr, int detailNo, string detailStr)
        {
            ErrorNo = errorNo;
            ErrorStr = errorStr;
            DetailNo = detailNo;
            DetailStr = detailStr;
        }



        public void Clear()
        {
            this.ErrorNo = 0;
            this.ErrorStr = "";
            this.DetailNo = 0;
            this.DetailStr = "";

        }
    }


    public enum ERROR_COMMON
    {
        NONE = 0,
        Read_IniFile = 1,
        Write_IniFile =2,
        NG_Read_Barcode = 3,
        NG_Same_SerialNo = 4,

        Reader_Open,
        Reader_Read,
        Reader_Close,

        Camera_Open,
        Camera_Capture,
        Camera_ChangeGain,
        Camera_Close,
    }

}
