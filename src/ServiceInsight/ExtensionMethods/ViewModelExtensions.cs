namespace Particular.ServiceInsight.Desktop.ExtensionMethods
{
    using System;
    using System.Linq.Expressions;
    using System.Windows.Input;
    using ReactiveUI;

    public static class ViewModelExtensions
    {
        // ReSharper disable UnusedParameter.Global
        public static ICommand CreateCommand<TViewModel>(this TViewModel viewModel, Action executAction, IObservable<bool> canExecuteObservable) where TViewModel : class
        // ReSharper restore UnusedParameter.Global
        {
            var command = new ReactiveCommand(canExecuteObservable);
            command.Subscribe(_ => executAction());
            return command;
        }

        public static ICommand CreateCommand<TViewModel>(this TViewModel viewModel, Action executAction, Expression<Func<TViewModel, bool>> canExecuteSelector) where TViewModel : class
        {
            var command = new ReactiveCommand(viewModel.ObservableForProperty(canExecuteSelector, b => b));
            command.Subscribe(_ => executAction());
            return command;
        }

        // ReSharper disable UnusedParameter.Global
        public static ICommand CreateCommand<TViewModel>(this TViewModel viewModel, Action executAction) where TViewModel : class
        // ReSharper restore UnusedParameter.Global
        {
            var command = new ReactiveCommand();
            command.Subscribe(_ => executAction());
            return command;
        }

        // ReSharper disable UnusedParameter.Global
        public static ICommand CreateCommand<TViewModel>(this TViewModel viewModel, Action<object> executAction) where TViewModel : class
        // ReSharper restore UnusedParameter.Global
        {
            var command = new ReactiveCommand();
            command.Subscribe(arg => executAction(arg));
            return command;
        }
    }
}