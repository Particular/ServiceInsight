namespace ServiceInsight.Saga
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    public partial class SagaWindowView : ISagaWindowView
    {
        public SagaWindowView()
        {
            InitializeComponent();
        }

        void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            //var model = DataContext as ISagaWindowViewModel;
            if (((Hyperlink)e.OriginalSource).DataContext is SagaTimeoutMessage message)
            {
                var steps = (ItemsControl)FindName("Steps");
                for (var i = 0; i < steps.Items.Count; i++)
                {
                    if (steps.Items[i] is SagaUpdate update && update.InitiatingMessage.MessageId == message.MessageId)
                    {
                        ScrollIntoView(steps, i);
                    }
                }
            }
        }

        static void ScrollIntoView(ItemsControl steps, int i)
        {
            var stepsContainer = steps.ItemContainerGenerator.ContainerFromIndex(i);
            if (stepsContainer != null && VisualTreeHelper.GetChildrenCount(stepsContainer) > 0)
            {
                var item = (FrameworkElement)VisualTreeHelper.GetChild(stepsContainer, 0);
                item.BringIntoView();
            }
        }

        public SagaWindowViewModel Model => DataContext as SagaWindowViewModel;
    }

    public interface ISagaWindowView
    {
    }
}