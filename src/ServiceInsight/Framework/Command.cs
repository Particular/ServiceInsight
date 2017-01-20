namespace ServiceInsight.Framework
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using System.Windows.Input;
    using Commands;
    using ExtensionMethods;

    public static class Command
    {
        public static ICommand Create(Action executeAction, IObservable<bool> canExecuteObservable) =>
            new ObservableCommand(canExecuteObservable, _ => executeAction());

        public static ICommand Create<TPropertyChanged>(TPropertyChanged propertyChanged, Action executeAction, Func<TPropertyChanged, bool> canExecuteSelector) where TPropertyChanged : INotifyPropertyChanged =>
            Create(executeAction, propertyChanged.ChangedProperty().Select(_ => canExecuteSelector(propertyChanged)));

        public static ICommand Create(Action executeAction) =>
            Create(_ => executeAction());

        public static ICommand Create(Action<object> executeAction) =>
            new DelegateCommand(executeAction);
    }
}