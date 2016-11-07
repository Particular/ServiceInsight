namespace ServiceInsight.MessageProperties
{
    using Caliburn.Micro;
    using ServiceInsight.Framework.Events;

    public interface IHeaderInfoViewModel :
        IScreen,
        IHandle<SelectedMessageChanged>
    {
    }
}