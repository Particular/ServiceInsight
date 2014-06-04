namespace Particular.ServiceInsight.Desktop.Saga
{
    using System.Windows.Input;

    public partial class SagaContentViewer
    {
        public SagaContentViewer()
        {
            InitializeComponent();
        }

        SagaUpdatedValue Model
        {
            get { return (SagaUpdatedValue)DataContext; }
        }

        void OnCloseGlyphClicked(object sender, MouseButtonEventArgs e)
        {
            Model.MessageContentVisible = false;
        }
    }
}