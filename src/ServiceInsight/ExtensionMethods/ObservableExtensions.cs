namespace ServiceInsight.ExtensionMethods
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive;
    using System.Reactive.Linq;

    static class ObservableExtensions
    {
        public static IObservable<EventPattern<PropertyChangedEventArgs>> Changed(this INotifyPropertyChanged item)
            => Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                   h => item.PropertyChanged += h,
                   h => item.PropertyChanged -= h);

        public static IObservable<PropertyChangedEventArgs> ObservableForProperty(this INotifyPropertyChanged item, string propertyName)
            => Changed(item)
                .Where(e => e.EventArgs.PropertyName == propertyName)
                .Select(e => e.EventArgs);

        public static IObservable<EventPattern<NotifyCollectionChangedEventArgs>> ChangedCollection(this INotifyCollectionChanged collection)
            => Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => collection.CollectionChanged += h,
                h => collection.CollectionChanged -= h);
    }
}