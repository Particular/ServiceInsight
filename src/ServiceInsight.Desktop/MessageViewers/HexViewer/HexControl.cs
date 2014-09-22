namespace Particular.ServiceInsight.Desktop.MessageViewers.HexViewer
{
    using System;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using ExtensionMethods;

    [TemplatePart(Name = "PART_HEXGRID", Type = typeof(Grid))]
    public class HexControl : Control
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data", typeof(byte[]), typeof(HexControl), new PropertyMetadata(default(byte[]), DataChanged));

        static void DataChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((HexControl)dependencyObject).OnDataChanged();
        }

        public byte[] Data
        {
            get { return (byte[])GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty HexLineStyleProperty = DependencyProperty.Register(
            "HexLineStyle", typeof(Style), typeof(HexControl), new PropertyMetadata(default(Style)));

        public Style HexLineStyle
        {
            get { return (Style)GetValue(HexLineStyleProperty); }
            set { SetValue(HexLineStyleProperty, value); }
        }

        public static readonly DependencyProperty HexNumberStyleProperty = DependencyProperty.Register(
            "HexNumberStyle", typeof(Style), typeof(HexControl), new PropertyMetadata(default(Style)));

        public Style HexNumberStyle
        {
            get { return (Style)GetValue(HexNumberStyleProperty); }
            set { SetValue(HexNumberStyleProperty, value); }
        }

        public static readonly DependencyProperty HexCharStyleProperty = DependencyProperty.Register(
            "HexCharStyle", typeof(Style), typeof(HexControl), new PropertyMetadata(default(Style)));

        public Style HexCharStyle
        {
            get { return (Style)GetValue(HexCharStyleProperty); }
            set { SetValue(HexCharStyleProperty, value); }
        }

        static HexControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HexControl), new FrameworkPropertyMetadata(typeof(HexControl)));
        }

        private Grid hexGrid;

        public override void OnApplyTemplate()
        {
            hexGrid = GetTemplateChild("PART_HEXGRID") as Grid;
        }

        private void OnDataChanged()
        {
            if (hexGrid == null || Data == null)
                return;

            hexGrid.Children.Clear();

            var lines = (int)Math.Ceiling(Data.Length / 16.0);
            var linePadding = (int)Math.Ceiling(Math.Log10(lines + 1));
            hexGrid.RowDefinitions.Clear();
            for (var i = 0; i < lines; i++)
            {
                hexGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            for (var i = 0; i < lines; i++)
            {
                var line = new TextBlock { Style = HexLineStyle, Text = (i + 1).ToString().PadLeft(linePadding) };
                Grid.SetColumn(line, 0);
                Grid.SetRow(line, i);
                hexGrid.Children.Add(line);
            }

            for (var i = 0; i < Data.Length; i++)
            {
                var x = i % 16;
                var y = i / 16;

                var number = new TextBlock { Style = HexNumberStyle, Text = ToHex(Data[i]) };
                Grid.SetColumn(number, x + 1);
                Grid.SetRow(number, y);
                hexGrid.Children.Add(number);

                var chr = new TextBlock { Style = HexCharStyle, Text = ToText(Data[i]) };
                Grid.SetColumn(chr, x + 18);
                Grid.SetRow(chr, y);
                hexGrid.Children.Add(chr);
            }
        }

        static string ToText(byte b)
        {
            Func<byte, string> ByteToStringConverter = byteValue => Encoding.UTF8.GetString(new[] { byteValue });
            var c = ByteToStringConverter.TryGetValue(b, " ");
            if (c == "\r" || c == "\n" || c == "\t")
            {
                return ".";
            }
            else
            {
                return c;
            }
        }

        static string ToHex(byte b)
        {
            return string.Format(b < 0x10 ? "0{0:X000} " : "{0:X000} ", b);
        }
    }
}