namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    using System.Linq;

    public interface IMessageHeadersView
    {
        void AutoFit();
        void CopyRowsToClipboard();
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

        public void CopyRowsToClipboard()
        {
            gridView.DataControl.BeginSelection();
            gridView.DataControl.SelectAll();
            gridView.DataControl.CopySelectedItemsToClipboard();
            gridView.DataControl.UnselectAll();
            gridView.DataControl.EndSelection();
        }
    }
}
