using System;
using System.Diagnostics;
using System.Windows.Input;

namespace NServiceBus.Profiler.Common
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The method to be called when the command is 
        /// invoked.</param>
        public RelayCommand(Action execute)
            : this(execute, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The method to be called when the command is 
        /// invoked.</param>
        /// <param name="canExecute">the method that determines whether the command 
        /// can execute in its current state.</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in 
        /// its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does 
        /// not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should 
        /// execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does 
        /// not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            _execute();
        }
    }
}