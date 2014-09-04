namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System.Windows;

    public partial class SequenceDiagramMessage
    {
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