namespace ServiceInsight
{
    using System;
    using System.Runtime.InteropServices;

    public class User32API
    {
        [DllImport("User32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

#pragma warning disable SA1310 // Field names should not contain underscore
        public const int SW_RESTORE = 9;
#pragma warning restore SA1310 // Field names should not contain underscore
    }
}