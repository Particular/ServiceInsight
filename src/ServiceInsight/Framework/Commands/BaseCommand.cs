namespace ServiceInsight.Framework.Commands
{
    using System;
    using System.Windows.Input;

    public abstract class BaseCommand : ICommand
    {
        public virtual bool CanExecute(object parameter) => true;

        public event EventHandler CanExecuteChanged;

        public abstract void Execute(object parameter);

        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}