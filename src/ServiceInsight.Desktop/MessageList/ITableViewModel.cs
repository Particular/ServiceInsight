namespace Particular.ServiceInsight.Desktop.MessageList
{
    using Caliburn.Micro;

    public interface ITableViewModel<T>
    {
        IObservableCollection<T> Rows { get; }

        T FocusedRow { get; set; }
    }
}