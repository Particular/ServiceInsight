namespace ServiceInsight.MessageProperties
{
    using DevExpress.Xpf.Bars;
    using DevExpress.Xpf.PropertyGrid;

    public partial class MessagePropertiesView : IMessagePropertiesView
    {
        public MessagePropertiesView()
        {
            InitializeComponent();
        }

        void OnPropertyContentCopy(object sender, ItemClickEventArgs e)
        {
            if (e.Item.DataContext is RowData data && data.Value != null)
            {
                var valueToCopy = data.Value is IPropertyDataProvider propertyProvider ? propertyProvider.DisplayName : data.Value;

                Model.CopyPropertyValue(valueToCopy);
            }
        }

        MessagePropertiesViewModel Model => (MessagePropertiesViewModel)DataContext;
    }

    public interface IMessagePropertiesView
    {
    }
}
