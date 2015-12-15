namespace ServiceInsight.Saga
{
    using System.Windows;
    using System.Windows.Input;

    public partial class SagaUpdateControl
    {
        public static readonly RoutedEvent TimeoutClickEvent = EventManager.RegisterRoutedEvent("TimeoutClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SagaUpdateControl));
        public static readonly DependencyProperty SelectedMessageIdProperty = DependencyProperty.Register("SelectedMessageId", typeof(string), typeof(SagaUpdateControl), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public SagaUpdateControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler TimeoutClick
        {
            add { AddHandler(TimeoutClickEvent, value); }
            remove { RemoveHandler(TimeoutClickEvent, value); }
        }

        public string SelectedMessageId
        {
            get { return (string)GetValue(SelectedMessageIdProperty); }
            set { SetValue(SelectedMessageIdProperty, value); }
        }

        void RootGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectedMessageId = ((SagaMessage)((FrameworkElement)sender).DataContext).MessageId;
        }

        void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var newEventArgs = new RoutedEventArgs(TimeoutClickEvent, e.Source);
            RaiseEvent(newEventArgs);
        }
    }
}