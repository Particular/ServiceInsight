namespace NServiceBus.Profiler.Desktop.MessageProperties
{
    /// <summary>
    /// Interaction logic for MessagePropertiesView.xaml
    /// </summary>
    public partial class MessagePropertiesView : IMessagePropertiesView
    {
        public MessagePropertiesView()
        {
            InitializeComponent();
        }
    }

    public interface IMessagePropertiesView
    {
    }
}
