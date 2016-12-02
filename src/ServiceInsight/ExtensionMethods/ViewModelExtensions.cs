namespace ServiceInsight.ExtensionMethods
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using System.Windows.Input;
    using Framework.Commands;

    public static class ViewModelExtensions
    {
        // ReSharper disable UnusedParameter.Global

        public static ICommand CreateCommand<TViewModel>(this TViewModel viewModel, Action executeAction, IObservable<bool> canExecuteObservable) where TViewModel : class
        {
            return new ObservableCommand(canExecuteObservable, _ => executeAction());
        }

        public static ICommand CreateCommand<TViewModel>(this TViewModel viewModel, Action executeAction, Func<TViewModel, bool> canExecuteSelector) where TViewModel : class, INotifyPropertyChanged
        {
            return CreateCommand(viewModel, executeAction, viewModel.Changed().Select(_ => canExecuteSelector(viewModel)));
        }

        public static ICommand CreateCommand<TViewModel>(this TViewModel viewModel, Action executeAction) where TViewModel : class
        {
            return CreateCommand(viewModel, _ => executeAction());
        }

        public static ICommand CreateCommand<TViewModel>(this TViewModel viewModel, Action<object> executeAction) where TViewModel : class
        {
            return new DelegateCommand(executeAction);
        }

        // ReSharper restore UnusedParameter.Global
    }
}