namespace ServiceInsight.LogWindow
{
    using System;
    using System.Threading;
    using System.Windows;
    using ExtensionMethods;

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

            logSubscription = vm.Logs.Changed().Subscribe(_ => richTextBox.ScrollToEnd());
        }
    }
}