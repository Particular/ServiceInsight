namespace ServiceInsight.MessageViewers.HexViewer
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;

    [TemplatePart(Name = "PART_LINES", Type = typeof(ItemsControl))]
    public class HexControl : Control
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Body", typeof(string), typeof(HexControl), new PropertyMetadata(null, DataChanged));

        static void DataChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var hexControl = (HexControl)dependencyObject;

            if (hexControl.items == null)
            {
                return;
            }

            if (hexControl.Body == null)
            {
                hexControl.items.ItemsSource = new List<HexContentLine>(0);
                return;
            }

            var body = Encoding.Default.GetBytes(hexControl.Body);
            var lines = (int)Math.Ceiling(body.Length / 16.0);
            var contentLines = new List<HexContentLine>(lines);

            for (var i = 0; i < lines; i++)
            {
                contentLines.Add(new HexContentLine(body, i));
            }

            hexControl.items.ItemsSource = contentLines;
        }

        public override void OnApplyTemplate()
        {
            items = GetTemplateChild("PART_LINES") as ItemsControl;
            DataChanged(this, new DependencyPropertyChangedEventArgs());
        }

        public string Body
        {
            get { return (string)GetValue(DataProperty); }
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

        ItemsControl items;

        public Style HexCharStyle
        {
            get { return (Style)GetValue(HexCharStyleProperty); }
            set { SetValue(HexCharStyleProperty, value); }
        }

        static HexControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HexControl), new FrameworkPropertyMetadata(typeof(HexControl)));
        }
    }
}