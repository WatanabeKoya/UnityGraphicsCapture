using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;

namespace Ruccho.GraphicsCapture
{
    public static class Utils
    {
        private delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lParam);

        delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum,
            IntPtr dwData);

        public static IEnumerable<ICaptureTarget> GetTargets(bool includeMonitor = true,
            bool includeNonCapturableWindows = false)
        {
            IEnumerable<ICaptureTarget> targets = GetTopWindows(includeNonCapturableWindows);
            if (includeMonitor)
                targets = targets.Concat(GetMonitors());

            return targets;
        }

        public static IEnumerable<WindowInfo> GetTopWindows(bool includeNonCapturableWindows = false)
        {
            var windowHandles = new List<IntPtr>();
            var handle = GCHandle.Alloc(windowHandles);
            try
            {
                EnumWindows(EnumWindowsCallback, GCHandle.ToIntPtr(handle));
            }
            finally
            {
                handle.Free();
            }

            return windowHandles
                .Select(hWnd => new WindowInfo(hWnd))
                .Where(windowInfo => includeNonCapturableWindows || windowInfo.IsCapturable());
        }

        [MonoPInvokeCallback(typeof(EnumMonitorsDelegate))]
        private static bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
        {
            var handle = GCHandle.FromIntPtr(lParam);
            if (handle.Target is List<IntPtr> list)
            {
                list.Add(hWnd);
            }
            return true;
        }

        public static IEnumerable<MonitorInfo> GetMonitors()
        {
            return EnumDisplayMonitors().Select(monitor => new MonitorInfo(monitor));
        }

        private static IEnumerable<IntPtr> EnumDisplayMonitors()
        {
            var monitors = new List<IntPtr>();
            var handle = GCHandle.Alloc(monitors);

            try
            {
                EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, EnumDisplayMonitorsCallback, GCHandle.ToIntPtr(handle));
            }
            finally
            {
                handle.Free();
            }

            return monitors;
        }

        [MonoPInvokeCallback(typeof(EnumMonitorsDelegate))]
        private static bool EnumDisplayMonitorsCallback(IntPtr monitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
        {
            var handle = GCHandle.FromIntPtr(dwData);
            if (handle.Target is List<IntPtr> list)
            {
                list.Add(monitor);
            }
            return true;
        }
    }
}