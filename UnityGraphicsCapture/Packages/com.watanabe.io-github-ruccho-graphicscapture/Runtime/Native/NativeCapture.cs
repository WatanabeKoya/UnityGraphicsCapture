using System;

namespace Ruccho.GraphicsCapture.Native
{
    public static class NativeCapture
    {
        public static IntPtr CreateCaptureFromWindow(IntPtr hWnd) =>
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
            WindowsCapture.CreateCaptureFromWindow(hWnd);
        #else
            throw new NotImplementedException();
        #endif

        public static IntPtr CreateCaptureFromMonitor(IntPtr hMon) =>
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
            WindowsCapture.CreateCaptureFromMonitor(hMon);
        #else
            throw new NotImplementedException();
        #endif

        public static void StartCapture(IntPtr capture) =>
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
            WindowsCapture.StartCapture(capture);
        #else
            throw new NotImplementedException();
        #endif

        public static void CloseCapture(IntPtr capture) =>
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
            WindowsCapture.CloseCapture(capture);
        #else
            throw new NotImplementedException();
        #endif

        public static int GetWidth(IntPtr capture) =>
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
            WindowsCapture.GetWidth(capture);
        #else
            throw new NotImplementedException();
        #endif

        public static int GetHeight(IntPtr capture) =>
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
            WindowsCapture.GetHeight(capture);
        #else
            throw new NotImplementedException();
        #endif

        public static IntPtr GetTexturePtr(IntPtr capture) =>
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
            WindowsCapture.GetTexturePtr(capture);
        #else
            throw new NotImplementedException();
        #endif
    }
}