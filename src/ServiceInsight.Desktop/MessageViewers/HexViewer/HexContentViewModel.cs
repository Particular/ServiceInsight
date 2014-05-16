namespace Particular.ServiceInsight.Desktop.MessageViewers.HexViewer
{
    using System;
    using System.Text;
    using Caliburn.Micro;
    using Events;
    using ExtensionMethods;

    public class HexContentViewModel : Screen,
        IHandle<SelectedMessageChanged>
    {
        internal static Func<byte, string> ByteToStringConverter;
        static Encoding Encoding;

        IHexContentView view;

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
            get;
            private set;
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            this.view = (IHexContentView)view;
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
            if (view == null || SelectedMessage == null)
                return;

            DisplayMessage();
        }

        void DisplayMessage()
        {
            ClearHexParts();
            CreateHexParts();
        }

        void ClearHexParts()
        {
            HexParts.Clear();
        }

        void CreateHexParts()
        {
            var columnNumber = 0;
            var lineNumber = 1;
            var hexLine = new HexPart(lineNumber);

            foreach (var currentByte in SelectedMessage)
            {
                if (!HexParts.Contains(hexLine))
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

        static void AppendText(HexNumber number, byte b)
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

        static void AppendHex(HexNumber hexValue, byte b)
        {
            hexValue.Hex = string.Format(b < 0x10 ? "0{0:X000} " : "{0:X000} ", b);
        }
    }
}