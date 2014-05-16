namespace Particular.ServiceInsight.Desktop.Core.UI
{
    using System;
    using System.Windows.Input;

    public class RelayCommand : ICommand
    {
        Func<bool> canExecute;
        Action execute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            if (execute == null) throw new ArgumentNullException("execute");
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this.canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (this.canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public virtual bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute();
        }

        public virtual void Execute(object parameter)
        {
            execute();
        }
    }
}