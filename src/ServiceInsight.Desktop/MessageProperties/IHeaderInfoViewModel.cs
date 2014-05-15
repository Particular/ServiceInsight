namespace Particular.ServiceInsight.Desktop.MessageProperties
{
    using Caliburn.Micro;
    using Events;

    public interface IHeaderInfoViewModel :
        IScreen,
        IHandle<SelectedMessageChanged>
    {
    }
}