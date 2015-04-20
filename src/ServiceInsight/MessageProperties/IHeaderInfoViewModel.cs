namespace Particular.ServiceInsight.Desktop.MessageProperties
{
    using Caliburn.Micro;
    using Particular.ServiceInsight.Desktop.Framework.Events;

    public interface IHeaderInfoViewModel :
        IScreen,
        IHandle<SelectedMessageChanged>
    {
    }
}