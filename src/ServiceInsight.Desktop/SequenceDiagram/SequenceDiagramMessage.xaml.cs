namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System.Windows;
    using System.Windows.Media;

    public partial class SequenceDiagramMessage
    {
        public static readonly DependencyProperty LineProperty = DependencyProperty.Register(
            "Line", typeof(Brush), typeof(SequenceDiagramMessage), new PropertyMetadata(default(Brush)));

        public Brush Line
        {
            get { return (Brush)GetValue(LineProperty); }
            set { SetValue(LineProperty, value); }
        }

        public static readonly DependencyProperty EndpointBoxProperty = DependencyProperty.Register(
            "EndpointBox", typeof(Brush), typeof(SequenceDiagramMessage), new PropertyMetadata(default(Brush)));

        public Brush EndpointBox
        {
            get { return (Brush)GetValue(EndpointBoxProperty); }
            set { SetValue(EndpointBoxProperty, value); }
        }

        public static readonly DependencyProperty MessageBackgroundProperty = DependencyProperty.Register(
            "MessageBackground", typeof(Brush), typeof(SequenceDiagramMessage), new PropertyMetadata(default(Brush)));

        public Brush MessageBackground
        {
            get { return (Brush)GetValue(MessageBackgroundProperty); }
            set { SetValue(MessageBackgroundProperty, value); }
        }

        public static readonly DependencyProperty MessageForegroundProperty = DependencyProperty.Register(
            "MessageForeground", typeof(Brush), typeof(SequenceDiagramMessage), new PropertyMetadata(default(Brush)));

        public Brush MessageForeground
        {
            get { return (Brush)GetValue(MessageForegroundProperty); }
            set { SetValue(MessageForegroundProperty, value); }
        }

        public static readonly DependencyProperty SagaBackgroundProperty = DependencyProperty.Register(
            "SagaBackground", typeof(Brush), typeof(SequenceDiagramMessage), new PropertyMetadata(default(Brush)));

        public Brush SagaBackground
        {
            get { return (Brush)GetValue(SagaBackgroundProperty); }
            set { SetValue(SagaBackgroundProperty, value); }
        }

        public static readonly DependencyProperty SagaForegroundProperty = DependencyProperty.Register(
            "SagaForeground", typeof(Brush), typeof(SequenceDiagramMessage), new PropertyMetadata(default(Brush)));

        public Brush SagaForeground
        {
            get { return (Brush)GetValue(SagaForegroundProperty); }
            set { SetValue(SagaForegroundProperty, value); }
        }

        public SequenceDiagramMessage()
        {
            InitializeComponent();
        }

        void CloseFailedMessagePopup(object sender, RoutedEventArgs e)
        {
            FailedMessagePopup.IsOpen = false;
        }
    }
}