namespace ServiceInsight.LogWindow
{
    using System.Windows;
    using System.Windows.Media;

    public class LogMessage
    {
        readonly string log;
        readonly Brush brush;
        readonly FontWeight weight;

        public LogMessage(string log, Color color, bool bold = false)
        {
            this.log = log;
            brush = new SolidColorBrush(color);
            weight = bold ? FontWeights.Bold : FontWeights.Normal;
        }

        public string Log => log;

        public Brush Brush => brush;

        public FontWeight Weight => weight;
    }
}