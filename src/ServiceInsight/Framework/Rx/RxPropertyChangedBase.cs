namespace ServiceInsight.Framework.Rx
{
    using Caliburn.Micro;
    using ReactiveUI;

    public class RxPropertyChangedBase : ReactiveObject, INotifyPropertyChangedEx
    {
        public RxPropertyChangedBase()
        {
            IsNotifying = true;
        }

        public bool IsNotifying { get; set; }

        public void NotifyOfPropertyChange(string propertyName)
        {
            if (IsNotifying)
            {
                Execute.OnUIThread(() => { raisePropertyChanging(propertyName); raisePropertyChanged(propertyName); });
            }
        }

        public void Refresh()
        {
            NotifyOfPropertyChange(string.Empty);
        }
    }
}