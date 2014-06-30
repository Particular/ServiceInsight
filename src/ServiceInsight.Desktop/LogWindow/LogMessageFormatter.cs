using System.Windows.Documents;

namespace Particular.ServiceInsight.Desktop.LogWindow
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Xml.Linq;
    using Xceed.Wpf.Toolkit;

    public class LogMessageFormatter : ITextFormatter
    {
        public string GetText(FlowDocument document)
        {
            var tr = new TextRange(document.ContentStart, document.ContentEnd);
            using (var ms = new MemoryStream())
            {
                tr.Save(ms, DataFormats.Xaml);
                var xelement = XElement.Parse(ASCIIEncoding.Default.GetString(ms.ToArray()));
                var paragraph = xelement.Descendants().First().Descendants();
                if (paragraph.Descendants().Any())
                    return paragraph.Descendants().First().ToString();
                return "";
            }
        }

        public void SetText(FlowDocument document, string text)
        {
            try
            {
                //if the text is null/empty clear the contents of the RTB. If you were to pass a null/empty string
                //to the TextRange.Load method an exception would occur.
                if (String.IsNullOrEmpty(text))
                {
                    document.Blocks.Clear();
                }
                else
                {
                    var wrappedText = "<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\"><Paragraph>" + text + "</Paragraph></Section>";

                    var tr = new TextRange(document.ContentStart, document.ContentEnd);
                    using (var ms = new MemoryStream(Encoding.ASCII.GetBytes(wrappedText)))
                    {
                        tr.Load(ms, DataFormats.Xaml);
                    }
                }
            }
            catch
            {
                throw new InvalidDataException("Data provided is not in the correct Xaml format.");
            }
        }
    }
}