namespace ServiceInsight.LogWindow
{
    using System;
    using System.Collections.Specialized;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Windows;

    public partial class LogWindowView
    {
        IDisposable logSubscription;

        public LogWindowView()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = DataContext as LogWindowViewModel;
            if (vm == null)
            {
                return;
            }

            Interlocked.Exchange(ref logSubscription, null)?.Dispose();

            logSubscription = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => vm.Logs.CollectionChanged += h,
                h => vm.Logs.CollectionChanged -= h)
                .Subscribe(_ => richTextBox.ScrollToEnd());
        }
    }
}