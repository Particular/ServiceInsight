using System;
using System.Collections.Generic;
using System.Text;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.ExtensionMethods;
using NServiceBus.Profiler.Common.Plugins;

namespace NServiceBus.Profiler.HexViewer
{
    public class HexContentViewModel : Screen, IHexContentViewModel
    {
        private IHexContentView _view;
        private static readonly Encoding Encoding;
        internal static Func<byte, string> ByteToStringConverter;

        static HexContentViewModel()
        {
            Encoding = new UTF8Encoding(false);
            ByteToStringConverter = byteValue => Encoding.GetString(new[] { byteValue });
        }

        public HexContentViewModel()
        {
            HexParts = new BindableCollection<HexPart>();
            ContextMenuItems = new List<PluginContextMenu>();
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _view = (IHexContentView) view;
            OnCurrentContentChanged();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            DisplayName = "Hex";
        }

        public byte[] CurrentContent { get; set; }

        public virtual void OnCurrentContentChanged()
        {
            if(_view == null || CurrentContent == null) 
                return;

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

            foreach (var currentByte in CurrentContent)
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

        public IObservableCollection<HexPart> HexParts
        {
            get; private set;
        }

        public IList<PluginContextMenu> ContextMenuItems
        {
            get; private set;
        }

        public int TabOrder
        {
            get { return 5; }
        }

        public void Handle(MessageBodyLoadedEvent @event)
        {
            HexParts.Clear();

            CurrentContent = null;
            CurrentContent = @event.Message != null ? @event.Message.BodyRaw : null;
        }

        public void Handle(SelectedMessageChangedEvent @event)
        {
            if (@event.SelectedMessage == null)
            {
                HexParts.Clear();
                CurrentContent = null;
            }
        }
    }
}