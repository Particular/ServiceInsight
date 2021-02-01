namespace ServiceInsight.Saga
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class SagaValueDiffControl : Control
    {
        const string SpaceMoniker = "•";

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(SagaValueDiffControl));

        public static readonly DependencyProperty WhitespaceBrushProperty = DependencyProperty.Register(
            "WhitespaceBrush",
            typeof(Brush),
            typeof(SagaValueDiffControl));

        public Brush WhitespaceBrush
        {
            get { return GetValue(WhitespaceBrushProperty) as Brush; }
            set { SetValue(WhitespaceBrushProperty, value); }
        }

        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (Text == null)
            {
                return;
            }

            var shouldPrintWhiteSpace = IsValueTypeString(Text);
            var renderString = GetRenderString();

            if (!shouldPrintWhiteSpace)
            {
                var formattedText = CreateFormattedText(renderString);
                drawingContext.DrawText(formattedText, new Point(0, 0));
            }
            else
            {
                var x = 0d;

                foreach (var character in Text)
                {
                    var whitespace = char.IsWhiteSpace(character);
                    var whitespaceChar = $" {SpaceMoniker} ";
                    var charToDraw = whitespace ? whitespaceChar : character.ToString();
                    var formattedText = CreateFormattedText(charToDraw, whitespace ? WhitespaceBrush : Foreground);

                    drawingContext.DrawText(formattedText, new Point(x, 0));

                    x += formattedText.WidthIncludingTrailingWhitespace;
                }
            }
        }

        string GetRenderString()
        {
            var shouldPrintWhiteSpace = IsValueTypeString(Text);
            if (!shouldPrintWhiteSpace)
            {
                return Text;
            }

            var builder = new StringBuilder();
            foreach (var character in Text)
            {
                var whitespace = char.IsWhiteSpace(character);
                var whitespaceChar = $" {SpaceMoniker} ";
                var charToDraw = whitespace ? whitespaceChar : character.ToString();

                builder.Append(charToDraw);
            }

            return builder.ToString();
        }

        FormattedText CreateFormattedText(string text, Brush foreground = null)
        {
            var pixelPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretches.Normal);
            return new FormattedText(
                text,
                CultureInfo.CurrentUICulture,
                FlowDirection,
                typeface,
                FontSize,
                foreground ?? Foreground,
                pixelPerDip);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var stringToRender = GetRenderString();
            var ft = CreateFormattedText(stringToRender);
            var size = new Size(Math.Min(availableSize.Width, ft.Width), Math.Min(availableSize.Height, ft.Height));

            return size;
        }

        bool IsValueTypeString(string value)
        {
            var isDate = IsDateTime(value);
            var isGuid = IsGuid(value);
            var isNumber = IsNumber(value);
            var isBoolean = IsBoolean(value);

            return !isDate && !isGuid && !isNumber && !isBoolean;
        }

        bool IsBoolean(string value)
        {
            var isBool = bool.TryParse(value, out _);
            return isBool;
        }

        bool IsNumber(string value)
        {
            var integer = int.TryParse(value, out _);
            var dec = decimal.TryParse(value, out _);

            return integer || dec;
        }

        bool IsGuid(string value)
        {
            return Guid.TryParse(value, out _);
        }

        bool IsDateTime(string value)
        {
            var isDate = DateTime.TryParse(value, out _);
            var isDto = DateTimeOffset.TryParse(value, out _);

            return isDate || isDto;
        }
    }
}