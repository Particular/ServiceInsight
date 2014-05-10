namespace Particular.ServiceInsight.Desktop.MessageProperties
{
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Events;

    public interface IHeaderInfoViewModel : 
        IScreen,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<SelectedMessageChanged>
    {
    }
}