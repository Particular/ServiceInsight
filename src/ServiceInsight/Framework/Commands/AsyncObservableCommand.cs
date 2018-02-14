namespace ServiceInsight.Framework.Commands
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    class AsyncObservableCommand : ICommand
    {
        private Func<object, Task> action;
        private bool latest;
        private bool isExecuting;

        public AsyncObservableCommand(IObservable<bool> canExecuteObservable, Func<object, Task> action)
        {
            this.action = action;

            canExecuteObservable
                .ObserveOnDispatcher()
                .Subscribe(b =>
                {
                    latest = b;
                    RaiseCanExecuteChanged();
                });
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => !isExecuting && latest;

        public async void Execute(object parameter)
        {
            using (StartExecuting())
            {
                await action(parameter);
            }
        }

        private void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private IDisposable StartExecuting()
        {
            isExecuting = true;
            RaiseCanExecuteChanged();

            return Disposable.Create(() =>
            {
                isExecuting = false;
                RaiseCanExecuteChanged();
            });
        }
    }
}