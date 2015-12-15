namespace ServiceInsight.Framework
{
    using System.Runtime.InteropServices;
    using System.Windows;

    public interface IClipboard
    {
        void CopyTo(string text);
    }

    class DefaultClipboard : IClipboard
    {
        public void CopyTo(string text)
        {
            try
            {
                Clipboard.SetText(text, TextDataFormat.Text);
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