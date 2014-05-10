namespace Particular.ServiceInsight.Desktop.MessageList
{
    using Caliburn.PresentationFramework;

    public interface ITableViewModel<T>
    {
        IObservableCollection<T> Rows { get; } 
        T FocusedRow { get; set; }
    }
}