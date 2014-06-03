using System.Windows;

namespace Particular.ServiceInsight.Desktop.LogWindow
{
    using System;

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
                return;

            if (logSubscription != null)
                logSubscription.Dispose();

            logSubscription = vm.Logs.ItemsAdded.Subscribe(_ => scroll.ScrollToEnd());
        }
    }
}