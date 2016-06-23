using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SamSeifert.CSCV.Cameras
{
    [Guid("16d975c3-8460-4385-899c-3e3e2764a568")]
    public class Camera : IDisposable
    {
        private int mCapHwnd;
        private IDataObject tempObj;

        #region API Declarations

        [DllImport("user32", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
            int hWnd, 
            uint Msg, 
            int wParam, 
            int lParam);

        [DllImport("avicap32.dll", EntryPoint = "capCreateCaptureWindowA")]
        private static extern int capCreateCaptureWindowA(
            string lpszWindowName,
            int dwStyle,
            int X,
            int Y,
            int nWidth,
            int nHeight,
            int hwndParent, 
            int nID);

        /*
        [DllImport("user32", EntryPoint = "OpenClipboard")]
        public static extern int OpenClipboard(int hWnd);

        [DllImport("user32", EntryPoint = "EmptyClipboard")]
        public static extern int EmptyClipboard();

        [DllImport("user32", EntryPoint = "CloseClipboard")]
        public static extern int CloseClipboard();
        */

        #endregion

        #region API Constants

        public const int WM_USER = 1024;

        public const int WM_CAP_CONNECT = 1034;
        public const int WM_CAP_DISCONNECT = 1035;
        public const int WM_CAP_GET_FRAME = 1084;
        public const int WM_CAP_COPY = 1054;

        public const int WM_CAP_START = WM_USER;

        public const int WM_CAP_DLG_VIDEOFORMAT = WM_CAP_START + 41;
        public const int WM_CAP_DLG_VIDEOSOURCE = WM_CAP_START + 42;
        public const int WM_CAP_DLG_VIDEODISPLAY = WM_CAP_START + 43;
        public const int WM_CAP_GET_VIDEOFORMAT = WM_CAP_START + 44;
        public const int WM_CAP_SET_VIDEOFORMAT = WM_CAP_START + 45;
        public const int WM_CAP_DLG_VIDEOCOMPRESSION = WM_CAP_START + 46;
        public const int WM_CAP_SET_PREVIEW = WM_CAP_START + 50;

        #endregion

        public Camera() { }

        void IDisposable.Dispose()
        {
            this.Stop();
        }

        /// <summary>
        /// Starts the video capture
        /// </summary>
        /// <param name="FrameNumber">the frame number to start at. 
        /// Set to 0 to let the control allocate the frame number</param>
        public void Start(Int32 handle)
        {
            // for safety, call stop, just in case we are already running
            this.Stop();

            // setup a capture window
            this.mCapHwnd = capCreateCaptureWindowA(
                "Select A Camera", // TItle
                0, // Style (See "Extended Window Styles")
                0, // x
                0, // y
                0, // width
                0, // height
                handle, // handle to parent window
                0); // window intifier

            // connect to the capture device
            Application.DoEvents();
            SendMessage(mCapHwnd, WM_CAP_CONNECT, 0, 0);
            SendMessage(mCapHwnd, WM_CAP_SET_PREVIEW, 0, 0);
        }

        /// <summary>
        /// Change Resolution
        /// </summary>
        public void configureResolution()
        {
            SendMessage(mCapHwnd, WM_CAP_DLG_VIDEOFORMAT, 0, 0);
        }

        /// <summary>
        /// Change Advanced Settings
        /// </summary>
        public void configureSource()
        {
            SendMessage(mCapHwnd, WM_CAP_DLG_VIDEOSOURCE, 0, 0);
        }

        /// <summary>
        /// Stops the video capture
        /// </summary>
        public void Stop()
        {
//            try
            {
                // disconnect from the video source
                Application.DoEvents();
                SendMessage(mCapHwnd, WM_CAP_DISCONNECT, 0, 0);
            }

//            catch { }
        }

        /// <summary>
        /// Capture the next frame from the video feed.  Can throw exceptions if user is using clipboard.
        /// </summary>
        public Bitmap capture()
        {
            // get the next frame;
            SendMessage(mCapHwnd, WM_CAP_GET_FRAME, 0, 0);

            // copy the frame to the clipboard
            SendMessage(mCapHwnd, WM_CAP_COPY, 0, 0);

            return (System.Drawing.Bitmap)Clipboard.GetDataObject().GetData(DataFormats.Bitmap);
        }


    }
}
