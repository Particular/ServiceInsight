namespace ServiceInsightInstaller.Managed
{
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using Caliburn.Micro;

    static class InstallerUtils
    {
        public static bool HResultSucceeded(int status)
        {
            return status >= 0;
        }

        static IntPtr hwnd = IntPtr.Zero;

        public static IntPtr HWnd
        {
            get
            {
                if (hwnd == IntPtr.Zero)
                {
                    Execute.OnUIThread(() =>
                    {
                        hwnd = new WindowInteropHelper(Application.Current.MainWindow).Handle;
                    });
                }

                return hwnd;
            }
        }
    }
}