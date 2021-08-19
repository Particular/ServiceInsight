namespace ServiceInsight.Framework.Rx
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Caliburn.Micro;
    using ObservablePropertyChanged;

    public class RxConductor<T> : Conductor<T>, IObservablePropertyChanged where T : class
    {
        ObservablePropertyChangeHelper helper = new ObservablePropertyChangeHelper();

        public IObservable<PropertyChangeData> Changed => helper.ChangedObservable;

        public void Dispose()
        {
            Interlocked.Exchange(ref helper, null)?.Dispose();
        }

        public override void NotifyOfPropertyChange([CallerMemberName] string propertyName = null)
        {
            base.NotifyOfPropertyChange(propertyName);
            helper.PropertyChanged(this, propertyName);
        }
    }
}