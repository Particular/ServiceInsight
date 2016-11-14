namespace ServiceInsight.ExtensionMethods
{
    using System;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using ReactiveUI;

    public static class ViewModelExtensions
    {
        public static ICommand CreateCommand<TViewModel>(this TViewModel viewModel, Action executAction, IObservable<bool> canExecuteObservable) where TViewModel : class
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

        public static ICommand CreateCommand<TViewModel>(this TViewModel viewModel, Action executAction) where TViewModel : class
        {
            var command = new ReactiveCommand();
            command.Subscribe(_ => executAction());
            return command;
        }

        public static ICommand CreateCommand<TViewModel>(this TViewModel viewModel, Action<object> executAction) where TViewModel : class
        {
            var command = new ReactiveCommand();
            command.Subscribe(arg => executAction(arg));
            return command;
        }

        public static ICommand CreateCommandAsync<TViewModel>(this TViewModel viewModel, Func<Task> executAction) where TViewModel : class
        {
            var command = new ReactiveCommand();
            command.SelectMany(async _ =>
            {
                await executAction();
                return Unit.Default;
            }).Subscribe();
            return command;
        }
    }
}