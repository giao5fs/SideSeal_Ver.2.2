using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArtCamSdk;


namespace SideSeal
{


    public class AccessCamera
    {
        public bool IsOpen;

        /// <summary> 右側カメラObj </summary>
        private CArtCam _RightCameraObj;
        /// <summary> 左側カメラObj </summary>
        private CArtCam _LeftCameraObj;

        public const int PIC_WIDTH = 752;
        public const int PIC_HEIGHT = 480;

        public const int BITMAP_WIDTH = 600;
        public const int BITMAP_HEIGHT = 480;

        private object _SyncObj;

        /// <summary>
        /// エラー列挙型
        /// </summary>
        public enum ERROR : uint
        {
            CAMERA_NONE = 0,
            CAMERA_LOADLIBRARY = 201,
            CAMERA_INITIALIZE = 202,
            CAMERA_SETDEVICENUMBER = 203,
            CAMERA_SETHALFCLOCK = 204,
            CAMERA_CAPTURE = 205,
            CAMERA_SETGAIN = 206,
            CAMERA_SNAPSHOT = 207,
            CAMERA_SETMIRROR = 208,
            CAMERA_IDCHECK = 209,
            CAMERA_TIMEOUT = 298,
            CAMERA_UNKNOWN = 299,
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AccessCamera()
        {
            // カメラObj初期化
            
       
            _RightCameraObj = new CArtCam();
            _LeftCameraObj = new CArtCam();
            _RightCameraObj.FreeLibrary();
            _LeftCameraObj.FreeLibrary();

            _SyncObj = new object();

            IsOpen = false;

        }

        /// <summary>
        /// OPEN
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public ErrorCode Open(IntPtr handle, int gain_right, int gain_left)
        {
            ERROR er = ERROR.CAMERA_UNKNOWN;
            ErrorCode code = new ErrorCode();
            code.Clear();

            IsOpen = false;

            if (!_RightCameraObj.LoadLibrary("ArtCamSdk_036MI2_WOM.dll"))
            {
                _RightCameraObj.GetLastError();


                er = ERROR.CAMERA_LOADLIBRARY;
                code.ErrorNo = (int)er;
                code.ErrorStr = er.ToString();
                code.DetailNo = _RightCameraObj.GetLastError();
                return code;
            }

            if (!_LeftCameraObj.LoadLibrary("ArtCamSdk_036MI2_WOM.dll"))
            {
                er = ERROR.CAMERA_LOADLIBRARY;
                code.ErrorNo = (int)er;
                code.ErrorStr = er.ToString();
                code.DetailNo = _LeftCameraObj.GetLastError();
                return code;
            }

            if (!_RightCameraObj.Initialize(handle))
            {
                er = ERROR.CAMERA_INITIALIZE;
                code.ErrorNo = (int)er;
                code.ErrorStr = er.ToString();
                code.DetailNo = _RightCameraObj.GetLastError();
                return code;
            }

            if (!_LeftCameraObj.Initialize(handle))
            {
                er = ERROR.CAMERA_INITIALIZE;
                code.ErrorNo = (int)er;
                code.ErrorStr = er.ToString();
                code.DetailNo = _LeftCameraObj.GetLastError();
                return code;
            }

            if (_RightCameraObj.SetDeviceNumber(0) != 1)
            {
                er = ERROR.CAMERA_SETDEVICENUMBER;
                code.ErrorNo = (int)er;
                code.ErrorStr = er.ToString();
                code.DetailNo = _RightCameraObj.GetLastError();
                return code;
            }

            if (_LeftCameraObj.SetDeviceNumber(1) != 1)
            {
                er = ERROR.CAMERA_SETDEVICENUMBER;
                code.ErrorNo = (int)er;
                code.ErrorStr = er.ToString();
                code.DetailNo = _LeftCameraObj.GetLastError();
                return code;
            }


            if (_RightCameraObj.SetHalfClock(1) != 1)
            {
                er = ERROR.CAMERA_SETHALFCLOCK;
                code.ErrorNo = (int)er;
                code.ErrorStr = er.ToString();
                code.DetailNo = _RightCameraObj.GetLastError();
                return code;
            }

            if (_LeftCameraObj.SetHalfClock(1) != 1)
            {
                er = ERROR.CAMERA_SETHALFCLOCK;
                code.ErrorNo = (int)er;
                code.ErrorStr = er.ToString();
                code.DetailNo = _LeftCameraObj.GetLastError();
                return code;
            }

            // IDチェック
            int leftidNo = 1;
            int rightidNo = 2;

            int left_id_now = _LeftCameraObj.ReadSromID(0);
            int right_id_now = _LeftCameraObj.ReadSromID(0);

            if (left_id_now == leftidNo && right_id_now == rightidNo)
            {
                // スルー
            }

            else if (left_id_now == rightidNo && right_id_now == leftidNo)
            {
                CArtCam tempobj = _RightCameraObj;
                _RightCameraObj = _LeftCameraObj;
                _LeftCameraObj = tempobj;
            }
            else
            {
                _RightCameraObj.WriteSromID(0, rightidNo);
                _LeftCameraObj.WriteSromID(0, rightidNo);
            }


            if (_RightCameraObj.SetGlobalGain(gain_right) != 1)
            {
                er = ERROR.CAMERA_SETGAIN;
                code.ErrorNo = (int)er;
                code.ErrorStr = er.ToString();
                code.DetailNo = _RightCameraObj.GetLastError();
                return code;
            }

            if (_LeftCameraObj.SetGlobalGain(gain_left) != 1)
            {
                er = ERROR.CAMERA_SETGAIN;
                code.ErrorNo = (int)er;
                code.ErrorStr = er.ToString();
                code.DetailNo = _LeftCameraObj.GetLastError();
                return code;
            }

            if (_RightCameraObj.SetMirrorH(true) != 1)
            {
                er = ERROR.CAMERA_SETMIRROR;
                code.ErrorNo = (int)er;
                code.ErrorStr = er.ToString();
                code.DetailNo = _RightCameraObj.GetLastError();
                return code;
            }

            if (_LeftCameraObj.SetMirrorH(true) != 1)
            {
                er = ERROR.CAMERA_SETMIRROR;
                code.ErrorNo = (int)er;
                code.ErrorStr = er.ToString();
                code.DetailNo = _LeftCameraObj.GetLastError();
                return code;
            }

            if (_RightCameraObj.Capture() != 1)
            {
                er = ERROR.CAMERA_CAPTURE;
                code.ErrorNo = (int)er;
                code.ErrorStr = er.ToString();
                code.DetailNo = _RightCameraObj.GetLastError();
                return code;
            }

            if (_LeftCameraObj.Capture() != 1)
            {
                er = ERROR.CAMERA_CAPTURE;
                code.ErrorNo = (int)er;
                code.ErrorStr = er.ToString();
                code.DetailNo = _LeftCameraObj.GetLastError();
                return code;
            }








            IsOpen = true;


            er = ERROR.CAMERA_NONE;
            code.ErrorNo = (int)er;
            code.ErrorStr = er.ToString();
            return code;
        }

        public ErrorCode Capture(out byte[] lpic, out byte[] rpic)
        {
            ERROR er = ERROR.CAMERA_UNKNOWN;
            ErrorCode code = new ErrorCode();
            code.Clear();

            lpic = new byte[PIC_WIDTH * PIC_HEIGHT*3];
            rpic = new byte[PIC_WIDTH * PIC_HEIGHT*3];

            lock (_SyncObj)
            {
                if (_LeftCameraObj.SnapShot(lpic, PIC_WIDTH * PIC_HEIGHT * 3, 0) != 1)
                {
                    er = ERROR.CAMERA_CAPTURE;
                    code.ErrorNo = (int)er;
                    code.ErrorStr = er.ToString();
                    code.DetailNo = _LeftCameraObj.GetLastError();
                    return code;
                }
                if (_RightCameraObj.SnapShot(rpic, PIC_WIDTH * PIC_HEIGHT * 3, 0) != 1)
                {
                    er = ERROR.CAMERA_CAPTURE;
                    code.ErrorNo = (int)er;
                    code.ErrorStr = er.ToString();
                    code.DetailNo = _RightCameraObj.GetLastError();
                    return code;
                }
            }

            //System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\Users\\masudako\\Documents\\test.csv");

            //int length = lpic.Length - 1000000;
            //for (int i = 0; i < length; i++)
            //{
            //    sw.WriteLine(lpic[i+1000000]);
            //}

            //sw.Close();
            //sw.Dispose();

            er = ERROR.CAMERA_NONE;
            code.ErrorNo = (int)er;
            code.ErrorStr = er.ToString();
            return code;
        }

        public ErrorCode ChangeGain(int gain_right, int gain_left)
        {
            ERROR er = ERROR.CAMERA_UNKNOWN;
            ErrorCode code = new ErrorCode();
            code.Clear();

            lock (_SyncObj)
            {
                if (_RightCameraObj.SetGlobalGain(gain_right) != 1)
                {
                    er = ERROR.CAMERA_SETGAIN;
                    code.ErrorNo = (int)er;
                    code.ErrorStr = er.ToString();
                    code.DetailNo = _RightCameraObj.GetLastError();
                    return code;

                }

                if (_LeftCameraObj.SetGlobalGain(gain_left) != 1)
                {
                    er = ERROR.CAMERA_SETGAIN;
                    code.ErrorNo = (int)er;
                    code.ErrorStr = er.ToString();
                    code.DetailNo = _LeftCameraObj.GetLastError();
                    return code;
                }
            }
            er = ERROR.CAMERA_NONE;
            code.ErrorNo = (int)er;
            code.ErrorStr = er.ToString();
            return code;
        }

        public ErrorCode Close()
        {
            ERROR er = ERROR.CAMERA_UNKNOWN;
            ErrorCode code = new ErrorCode();
            code.Clear();

            if (IsOpen)
            {
                _RightCameraObj.Close();
                _LeftCameraObj.Close();

                _RightCameraObj.Release();
                _LeftCameraObj.Release();
            }

            er = ERROR.CAMERA_NONE;
            code.ErrorNo = (int)er;
            code.ErrorStr = er.ToString();
            return code;




        }

    }
}
