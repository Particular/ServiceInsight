using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Particular.ServiceInsight.Desktop.MessageHeaders
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

        public PropertyGrid PropertControl { get { return PropertyGrid; } }
    }

    public interface IMessagePropertiesView
    {
        PropertyGrid PropertControl { get; }
    }
}
