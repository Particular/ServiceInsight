namespace Particular.ServiceInsight.Desktop.Core.UI.ScreenManager
{
    using System;
    using System.Windows.Input;
    using Caliburn.PresentationFramework;

    public delegate void ButtonCommandHandler(object target);

    public class ChoiceCommand : PropertyChangedBase, ICommand
    {
        public event EventHandler CanExecuteChanged = delegate { };

        private readonly ButtonCommandHandler _commandHandler;

        public ChoiceCommand(ButtonCommandHandler commandHandler, bool isDefault, bool isCancel, string label, MessageChoice result)
        {
            _commandHandler = (ButtonCommandHandler)Delegate.Combine(_commandHandler, commandHandler);
            IsDefault = isDefault;
            IsCancel = isCancel;
            Label = label;
            Result = result;
        }

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public virtual void Execute(object parameter)
        {
            _commandHandler(this);
        }

        public virtual bool IsCancel
        {
            get; set;
        }

        public virtual bool IsDefault
        {
            get; set;
        }

        public virtual string Label
        {
            get; set;
        }

        public virtual MessageChoice Result
        {
            get; private set;
        }
    }
}