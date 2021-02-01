namespace ServiceInsight.Framework.Commands
{
    using System;
    using System.Reactive.Disposables;
    using System.Windows.Input;

    class ObservableCommand : ICommand
    {
        Action<object> action;
        bool latest;
        bool isExecuting;

        public ObservableCommand(IObservable<bool> canExecuteObservable, Action<object> action)
        {
            this.action = action;

            canExecuteObservable
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

        void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        IDisposable StartExecuting()
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