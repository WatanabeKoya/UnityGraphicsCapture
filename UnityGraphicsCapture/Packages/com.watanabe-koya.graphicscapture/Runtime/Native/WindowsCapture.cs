using System;
using System.Runtime.InteropServices;

namespace Ruccho.GraphicsCapture.Native
{
    #if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public static class WindowsCapture
    {
        [DllImport("GraphicsCapture")]
        public static extern IntPtr CreateCaptureFromWindow(IntPtr hWnd);

        [DllImport("GraphicsCapture")]
        public static extern IntPtr CreateCaptureFromMonitor(IntPtr hMon);

        [DllImport("GraphicsCapture")]
        public static extern void StartCapture(IntPtr capture);

        [DllImport("GraphicsCapture")]
        public static extern void CloseCapture(IntPtr capture);

        [DllImport("GraphicsCapture")]
        public static extern int GetWidth(IntPtr capture);

        [DllImport("GraphicsCapture")]
        public static extern int GetHeight(IntPtr capture);

        [DllImport("GraphicsCapture")]
        public static extern IntPtr GetTexturePtr(IntPtr capture);
    }
    #endif
}

