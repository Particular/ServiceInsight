using System;
using System.Text;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.ExtensionMethods;

namespace NServiceBus.Profiler.Desktop.MessageViewers.HexViewer
{
    public class HexContentViewModel : Screen, IHexContentViewModel
    {
        internal static Func<byte, string> ByteToStringConverter;
        private static readonly Encoding Encoding;

        private IHexContentView _view;

        static HexContentViewModel()
        {
            Encoding = new UTF8Encoding(false);
            ByteToStringConverter = byteValue => Encoding.GetString(new[] { byteValue });
        }

        public HexContentViewModel()
        {
            HexParts = new BindableCollection<HexPart>();
        }

        public byte[] SelectedMessage { get; set; }

        public IObservableCollection<HexPart> HexParts
        {
            get; private set;
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _view = (IHexContentView) view;
            OnSelectedMessageChanged();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            DisplayName = "Hex";
        }

        public void Handle(SelectedMessageChanged @event)
        {
            byte[] body = null;

            if (@event.Message != null && @event.Message.Body != null)
            {
                body = Encoding.Default.GetBytes(@event.Message.Body);
            }

            SelectedMessage = body;
        }

        public void OnSelectedMessageChanged()
        {
            if (_view == null || SelectedMessage == null) 
                return;

            DisplayMessage();
        }

        private void DisplayMessage()
        {
            ClearHexParts();
            CreateHexParts();
        }

        private void ClearHexParts()
        {
            HexParts.Clear();
        }

        private void CreateHexParts()
        {
            var columnNumber = 0;
            var lineNumber = 1;
            var hexLine = new HexPart(lineNumber);

            foreach (var currentByte in SelectedMessage)
            {
                if(!HexParts.Contains(hexLine))
                    HexParts.Add(hexLine);

                var hexChar = new HexNumber();

                AppendHex(hexChar, currentByte);
                AppendText(hexChar, currentByte);

                hexLine.Numbers.Add(hexChar);
                columnNumber++;

                if (columnNumber == 16)
                {
                    lineNumber++;
                    columnNumber = 0;
                    hexLine = new HexPart(lineNumber);
                }
            }
        }

        private static void AppendText(HexNumber number, byte b)
        {
            var c = ByteToStringConverter.TryGetValue(b, " ");
            if (c == "\r" || c == "\n" || c == "\t")
            {
                number.Text = ".";
            }
            else
            {
                number.Text = c;
            }
        }

        private static void AppendHex(HexNumber hexValue, byte b)
        {
            hexValue.Hex = string.Format(b < 0x10 ? "0{0:X000} " : "{0:X000} ", b);
        }
    }
}