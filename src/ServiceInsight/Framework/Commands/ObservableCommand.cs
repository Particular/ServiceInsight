namespace ServiceInsight.Framework.Commands
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Windows.Input;

    class ObservableCommand : ICommand
    {
        private Action<object> action;
        private IDisposable canExecuteSubscription;
        private bool latest;
        private bool isExecuting;

        public ObservableCommand(IObservable<bool> canExecuteObservable, Action<object> action)
        {
            this.action = action;

            canExecuteSubscription = canExecuteObservable
                .Subscribe(b =>
                {
                    latest = b;
                    RaiseCanExecuteChanged();
                });
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => !isExecuting && latest;

        public void Execute(object parameter)
        {
            using (StartExecuting())
            {
                action(parameter);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            canExecuteSubscription?.Dispose();
            canExecuteSubscription = null;
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