namespace Particular.ServiceInsight.Desktop.MessageHeaders
{
    using System;
    using System.Reactive.Linq;
    using System.Windows;
    using DevExpress.Xpf.Bars;
    using ReactiveUI;

    public partial class MessageHeadersView
    {
        IDisposable kvSubscription;

        public MessageHeadersView()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var vm = DataContext as MessageHeadersViewModel;
            if (vm == null)
                return;

            if (kvSubscription != null)
                kvSubscription.Dispose();

            kvSubscription = vm.KeyValues.Changed
                .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler) // Just to ignore multiple adds from a loop
                .Subscribe(_ => gridView.BestFitColumn(KeyColumn));
        }

        void CopyToClipboard_OnItemClick(object sender, ItemClickEventArgs e)
        {
            gridView.DataControl.BeginSelection();
            gridView.DataControl.SelectAll();
            gridView.DataControl.CopySelectedItemsToClipboard();
            gridView.DataControl.UnselectAll();
            gridView.DataControl.EndSelection();
        }
    }
}