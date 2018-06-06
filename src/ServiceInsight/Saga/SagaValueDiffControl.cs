namespace ServiceInsight.Saga
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class SagaValueDiffControl : Control
    {
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

            foreach (var character in Text)
            {
                var whitespace = char.IsWhiteSpace(character);
                var formattedText = new FormattedText(
                    whitespace ? " ˽ " : character.ToString(),
                    CultureInfo.CurrentUICulture,
                    FlowDirection,
                    new Typeface(FontFamily, FontStyle, FontWeight, FontStretches.Normal),
                    FontSize,
                    whitespace ? WhitespaceBrush : Foreground);

                drawingContext.DrawText(formattedText, new Point(currentPosition, 0));
                currentPosition += formattedText.Width;
            }
        }
    }
}