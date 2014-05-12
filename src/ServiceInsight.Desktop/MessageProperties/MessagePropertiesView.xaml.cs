namespace Particular.ServiceInsight.Desktop.MessageProperties
{
    using DevExpress.Xpf.Bars;
    using DevExpress.Xpf.PropertyGrid;

    /// <summary>
    /// Interaction logic for MessagePropertiesView.xaml
    /// </summary>
    public partial class MessagePropertiesView : IMessagePropertiesView
    {
        public MessagePropertiesView()
        {
            InitializeComponent();
        }

        void OnPropertyContentCopy(object sender, ItemClickEventArgs e)
        {
            var data = e.Item.DataContext as RowData;
            if (data != null && data.Value != null)
            {
                var propertyProvider = data.Value as IPropertyDataProvider;
                var valueToCopy = propertyProvider != null ? propertyProvider.DisplayName : data.Value;
                
                Model.CopyPropertyValue(valueToCopy);
            }
        }

        IMessagePropertiesViewModel Model
        {
            get {  return (IMessagePropertiesViewModel)DataContext; }
        }
    }

    public interface IMessagePropertiesView
    {
    }
}
