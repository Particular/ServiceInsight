namespace Particular.ServiceInsight.Desktop.Framework.Rx
{
    using System;
    using System.Linq.Expressions;
    using System.Windows.Input;
    using ReactiveUI;

    public static class ViewModelExtensions
    {
        public static ICommand CreateCommand<TViewModel>(this TViewModel viewModel, Expression<Func<TViewModel, bool>> canExecuteSelector, Action executAction) where TViewModel : class
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
    }
}