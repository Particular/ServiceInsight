using System.Windows;

namespace Particular.ServiceInsight.Desktop.LogWindow
{
    using System;

    public partial class LogWindowView
    {
        public LogWindowView()
        {
            InitializeComponent();

            DataContextChanged += LogWindowView_DataContextChanged;
        }

        void LogWindowView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = DataContext as LogWindowViewModel;
            if (vm == null)
                return;

            vm.Logs.ItemsAdded.Subscribe(_ => scroll.ScrollToEnd());
        }
    }
}