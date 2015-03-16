namespace Particular.ServiceInsight.Desktop.ValueConverters
{
    using System.Threading;

    public class MessageDateTimeFormatProvider
    {
        static MessageDateTimeFormatProvider()
        {
            var culture = Thread.CurrentThread.CurrentCulture;

            MessageDateFormat = string.Format("{0} hh:mm:ss.ffff tt", culture.DateTimeFormat.ShortDatePattern);
        }

        public static string MessageDateFormat { get; private set; }
    }
}