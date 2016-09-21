namespace ServiceInsight.Framework.Rx
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Caliburn.Micro;
    using Pirac;

    public class RxConductor<T> : Conductor<T>, IObservablePropertyChanged where T : class
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

        public class RxCollection
        {
            public class AllActive : Collection.AllActive, IObservablePropertyChanged
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

            public class OneActive : Collection.OneActive, IObservablePropertyChanged
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
    }
}