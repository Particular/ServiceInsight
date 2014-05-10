namespace Particular.ServiceInsight.Desktop.MessageViewers.XmlViewer
{
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Caliburn.PresentationFramework.Views;
    using Events;
    using Models;

    public interface IXmlMessageViewModel : 
        IScreen,
        IViewAware,
        IHandle<SelectedMessageChanged>
    {
        MessageBody SelectedMessage { get; set; }
        void CopyMessageXml();
        bool CanCopyMessageXml();
    }
}