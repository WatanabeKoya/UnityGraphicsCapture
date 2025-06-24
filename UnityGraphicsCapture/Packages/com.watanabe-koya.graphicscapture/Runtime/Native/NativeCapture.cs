using System;

namespace Ruccho.GraphicsCapture.Native
{
    public static class NativeCapture
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        public static IntPtr CreateCaptureFromWindow(IntPtr hWnd) => WindowsCapture.CreateCaptureFromWindow(hWnd);
        public static IntPtr CreateCaptureFromMonitor(IntPtr hMon) => WindowsCapture.CreateCaptureFromMonitor(hMon);
        public static void StartCapture(IntPtr capture) => WindowsCapture.StartCapture(capture);
        public static void CloseCapture(IntPtr capture) => WindowsCapture.CloseCapture(capture);
        public static int GetWidth(IntPtr capture) => WindowsCapture.GetWidth(capture);
        public static int GetHeight(IntPtr capture) => WindowsCapture.GetHeight(capture);
        public static void Render(IntPtr capture, IntPtr renderTexture) => WindowsCapture.Render(capture, renderTexture);
#else
        public static IntPtr CreateCaptureFromWindow(IntPtr hWnd) => throw new NotImplementedException();
        public static IntPtr CreateCaptureFromMonitor(IntPtr hMon) => throw new NotImplementedException();
        public static void StartCapture(IntPtr capture) => throw new NotImplementedException();
        public static void CloseCapture(IntPtr capture) => throw new NotImplementedException();
        public static int GetWidth(IntPtr capture) => throw new NotImplementedException();
        public static int GetHeight(IntPtr capture) => throw new NotImplementedException();
        public static void Render(IntPtr capture, IntPtr renderTexture) => throw new NotImplementedException();
#endif
    }
}