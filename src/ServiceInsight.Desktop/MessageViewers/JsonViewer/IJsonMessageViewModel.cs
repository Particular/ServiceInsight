namespace Particular.ServiceInsight.Desktop.MessageViewers.JsonViewer
{
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Caliburn.PresentationFramework.Views;
    using Events;
    using Models;

    public interface IJsonMessageViewModel :
        IViewAware,
        IScreen,
        IHandle<SelectedMessageChanged>
    {
        MessageBody SelectedMessage { get; set; }
    }
}