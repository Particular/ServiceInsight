namespace ServiceInsight.ExtensionMethods
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;

    static class ObservableExtensions
    {
        public static IObservable<PropertyChangedEventArgs> ChangedProperty(this INotifyPropertyChanged item) =>
            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => item.PropertyChanged += h,
                h => item.PropertyChanged -= h)
            .Select(e => e.EventArgs);

        public static IObservable<PropertyChangedEventArgs> ChangedProperty(this INotifyPropertyChanged item, string propertyName) =>
            ChangedProperty(item).Where(e => e.PropertyName == propertyName);

        public static IObservable<NotifyCollectionChangedEventArgs> ChangedCollection(this INotifyCollectionChanged collection) =>
            Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => collection.CollectionChanged += h,
                h => collection.CollectionChanged -= h)
            .Select(e => e.EventArgs);
    }
}