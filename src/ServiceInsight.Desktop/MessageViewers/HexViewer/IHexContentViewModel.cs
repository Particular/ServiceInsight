using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using NServiceBus.Profiler.Desktop.Events;

namespace NServiceBus.Profiler.Desktop.MessageViewers.HexViewer
{
    public interface IHexContentViewModel : 
        IScreen,
        IViewAware,
        IHandle<SelectedMessageChanged>
    {
        byte[] CurrentContent { get; set; }
        IObservableCollection<HexPart> HexParts { get; }
    }
}