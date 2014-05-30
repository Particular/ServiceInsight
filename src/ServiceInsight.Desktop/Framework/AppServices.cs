namespace Particular.ServiceInsight.Desktop.Framework
{
    using System.Runtime.InteropServices;
    using System.Windows;

    public interface IClipboard
    {
        void CopyTo(string text);
    }

    static class AppServices
    {
        static IClipboard clipboard;

        public static IClipboard Clipboard
        {
            get
            {
                if (clipboard != null)
                    return clipboard;

                clipboard = new DefaultClipboard();
                return clipboard;
            }
            set { clipboard = value; }
        }

        class DefaultClipboard : IClipboard
        {
            public void CopyTo(string text)
            {
                try
                {
                    System.Windows.Clipboard.SetText(text, TextDataFormat.Text);
                }
                catch (COMException ex)
                {
                    // http://connect.microsoft.com/VisualStudio/feedback/details/775218/comexception-when-calling-clipboard-settext-or-clipboard-setdata-when-bing-desktop-is-running
                    if (ex.ErrorCode != -2147221040)
                    {
                        throw;
                    }
                }
            }
        }
    }
}