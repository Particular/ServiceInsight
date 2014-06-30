namespace Particular.ServiceInsight.Desktop.LogWindow
{
    using System;
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
                return;

            if (logSubscription != null)
                logSubscription.Dispose();

            logSubscription = vm.Logs.ItemsAdded.Subscribe(_ => richTextBox.ScrollToEnd());
        }
    }
}