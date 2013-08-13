using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using Particular.ServiceInsight.Desktop.Events;
using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.MessageViewers.XmlViewer
{
    public interface IXmlMessageViewModel : 
        IScreen,
        IViewAware,
        IHandle<MessageBodyLoaded>,
        IHandle<SelectedMessageChanged>
    {
        MessageBody SelectedMessage { get; set; }
        void CopyMessageXml();
        bool CanCopyMessageXml();
    }
}