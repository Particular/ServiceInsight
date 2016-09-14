namespace ServiceInsight.Framework.Rx
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Pirac;

    public class RxScreen : Caliburn.Micro.Screen, IObservablePropertyChanged
    {
        PiracHelper helper = new PiracHelper();

        public IObservable<PropertyChangedData> Changed => helper.Changed;

        public void Dispose()
        {
            Interlocked.Exchange(ref helper, null)?.Dispose();
        }

        public override void NotifyOfPropertyChange([CallerMemberName] string propertyName = null)
        {
            base.NotifyOfPropertyChange(propertyName);
            helper.PropertyChanged(propertyName);
        }
    }
}