namespace Particular.ServiceInsight.Desktop.LogWindow
{
    using System.Windows;
    using System.Windows.Media;

    public class LogMessage
    {
        private readonly string log;
        private readonly Brush brush;
        private readonly FontWeight weight;

        public LogMessage(string log, Color color, bool bold = false)
        {
            this.log = log;
            brush = new SolidColorBrush(color);
            weight = bold ? FontWeights.Bold : FontWeights.Normal;
        }

        public string Log { get { return log; } }

        public string DisplayLog { get { return log.TrimEnd(); } }

        public Brush Brush { get { return brush; } }

        public FontWeight Weight { get { return weight; } }
    }
}