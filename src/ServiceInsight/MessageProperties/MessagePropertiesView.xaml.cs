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
            var data = e.Item.DataContext as RowData;
            if (data != null && data.Value != null)
            {
                var propertyProvider = data.Value as IPropertyDataProvider;
                var valueToCopy = propertyProvider != null ? propertyProvider.DisplayName : data.Value;

                Model.CopyPropertyValue(valueToCopy);
            }
        }

        MessagePropertiesViewModel Model => (MessagePropertiesViewModel)DataContext;
    }

    public interface IMessagePropertiesView
    {
    }
}
