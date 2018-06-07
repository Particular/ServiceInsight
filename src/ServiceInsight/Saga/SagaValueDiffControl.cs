namespace ServiceInsight.Saga
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class SagaValueDiffControl : Control
    {
        const string SpaceMoniker = "˽";

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text",
            typeof(string),
            typeof(SagaValueDiffControl));

        public static readonly DependencyProperty WhitespaceBrushProperty = DependencyProperty.Register("WhitespaceBrush",
            typeof(Brush),
            typeof(SagaValueDiffControl));

        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        public Brush WhitespaceBrush
        {
            get { return GetValue(WhitespaceBrushProperty) as Brush; }
            set { SetValue(WhitespaceBrushProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (Text == null)
            {
                return;
            }

            var currentPosition = 0d;
            var firstChar = true;
            var shouldPrintWhiteSpace = IsValueTypeString(Text);

            foreach (var character in Text)
            {
                var whitespace = char.IsWhiteSpace(character);
                var whitespaceChar = firstChar ? $"{SpaceMoniker} " : $" {SpaceMoniker} ";
                var charToDraw = whitespace && shouldPrintWhiteSpace ? whitespaceChar : character.ToString();

                var formattedText = new FormattedText(
                    charToDraw,
                    CultureInfo.CurrentUICulture,
                    FlowDirection,
                    new Typeface(FontFamily, FontStyle, FontWeight, FontStretches.Normal),
                    FontSize,
                    whitespace ? WhitespaceBrush : Foreground);

                drawingContext.DrawText(formattedText, new Point(currentPosition, 0));
                currentPosition += formattedText.WidthIncludingTrailingWhitespace;
                firstChar = false;
            }
        }

        private bool IsValueTypeString(string value)
        {
            return !IsDateTime(value) &&
                   !IsGuid(value);
        }

        private bool IsGuid(string value)
        {
            return Guid.TryParse(value, out _);
        }

        private bool IsDateTime(string value)
        {
            return DateTime.TryParse(value, out _);
        }
    }
}