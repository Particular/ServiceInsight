using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using Particular.ServiceInsight.Desktop.Events;

namespace Particular.ServiceInsight.Desktop.MessageViewers.HexViewer
{
    public interface IHexContentViewModel : 
        IScreen,
        IViewAware,
        IHandle<MessageBodyLoaded>,
        IHandle<SelectedMessageChanged>
    {
        byte[] CurrentContent { get; set; }
        IObservableCollection<HexPart> HexParts { get; }
    }
}