namespace Particular.ServiceInsight.Desktop.Saga
{
    using System.Windows;
    using System.Windows.Input;

    public partial class SagaUpdateControl
    {
        public static readonly RoutedEvent TimeoutClickEvent = EventManager.RegisterRoutedEvent("TimeoutClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SagaUpdateControl));

        public static readonly DependencyProperty SelectedMessageProperty = DependencyProperty.Register("SelectedMessage", typeof(SagaMessage), typeof(SagaUpdateControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public SagaUpdateControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler TimeoutClick
        {
            add { AddHandler(TimeoutClickEvent, value); }
            remove { RemoveHandler(TimeoutClickEvent, value); }
        }

        public SagaMessage SelectedMessage
        {
            get { return (SagaMessage)GetValue(SelectedMessageProperty); }
            set { SetValue(SelectedMessageProperty, value); }
        }

        void RootGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectedMessage = ((FrameworkElement)sender).DataContext as SagaMessage;
        }

        void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var newEventArgs = new RoutedEventArgs(TimeoutClickEvent, e.Source);
            RaiseEvent(newEventArgs);
        }

        void ExpandData_Click(object sender, RoutedEventArgs e)
        {
            var newEventArgs = new RoutedEventArgs(TimeoutClickEvent, e.Source);
            RaiseEvent(newEventArgs);
        }
    }
}