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
            var message = ((Hyperlink)e.OriginalSource).DataContext as SagaTimeoutMessage;
            if (message != null)
            {
                var steps = (ItemsControl)FindName("Steps");
                for (var i = 0; i < steps.Items.Count; i++)
                {
                    var update = steps.Items[i] as SagaUpdate;
                    if (update != null && update.InitiatingMessage.MessageId == message.MessageId)
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
    }

    public interface ISagaWindowView
    {
    }
}