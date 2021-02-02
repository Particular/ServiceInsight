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
            if (!(DataContext is LogWindowViewModel vm))
            {
                return;
            }

            Interlocked.Exchange(ref logSubscription, null)?.Dispose();

            logSubscription = vm.Logs.ChangedCollection().Subscribe(_ => richTextBox.ScrollToEnd());
        }
    }
}