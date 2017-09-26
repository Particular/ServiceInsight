namespace ServiceInsight.Framework.Commands
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    public class CloseWindowCommand : ICommand
    {
        readonly Window target;

        public CloseWindowCommand(Window target) => this.target = target;

        public bool CanExecute(object parameter) => target != null;

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                target.Close();
            }
        }

#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067
    }
}