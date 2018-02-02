namespace ServiceInsight.Framework.Commands
{
    using System;
    using System.Reactive.Disposables;
    using System.Windows.Input;

    class ObservableCommand : ICommand
    {
        private Action<object> action;
        private bool latest;
        private bool isExecuting;

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