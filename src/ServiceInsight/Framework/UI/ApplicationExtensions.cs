namespace ServiceInsight.Framework.UI
{
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    public static class ApplicationExtensions
    {
        public static void DoEvents(this Application current)
        {
            current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(() => { }));
        }
    }
}