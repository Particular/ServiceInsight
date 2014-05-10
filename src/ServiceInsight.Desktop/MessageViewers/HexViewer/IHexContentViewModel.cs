namespace Particular.ServiceInsight.Desktop.MessageViewers.HexViewer
{
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Caliburn.PresentationFramework.Views;
    using Events;

    public interface IHexContentViewModel : 
        IScreen,
        IViewAware,
        IHandle<SelectedMessageChanged>
    {
        byte[] SelectedMessage { get; set; }
        IObservableCollection<HexPart> HexParts { get; }
    }
}