namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    public interface IMessageHeadersView
    {
        void AutoFit();
    }

    public partial class MessageHeadersView : IMessageHeadersView
    {
        public MessageHeadersView()
        {
            InitializeComponent();
        }

        public void AutoFit()
        {
            gridView.BestFitColumn(KeyColumn);
        }
    }
}
