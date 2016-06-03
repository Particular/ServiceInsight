namespace ServiceInsight.Framework.UI.ScreenManager
{
    using System;
    using System.Windows.Input;
    using Caliburn.Micro;

    public delegate void ButtonCommandHandler(object target);

    public class ChoiceCommand : PropertyChangedBase, ICommand
    {
        public event EventHandler CanExecuteChanged = (s, e) => { };

        ButtonCommandHandler commandHandler;

        public ChoiceCommand(ButtonCommandHandler commandHandler, bool isDefault, bool isCancel, string label, MessageChoice result)
        {
            this.commandHandler = (ButtonCommandHandler)Delegate.Combine(this.commandHandler, commandHandler);
            IsDefault = isDefault;
            IsCancel = isCancel;
            Label = label;
            Result = result;
        }

        public virtual bool CanExecute(object parameter) => true;

        public virtual void Execute(object parameter)
        {
            commandHandler(this);
        }

        public virtual bool IsCancel
        {
            get;
            set;
        }

        public virtual bool IsDefault
        {
            get;
            set;
        }

        public virtual string Label
        {
            get;
            set;
        }

        public virtual MessageChoice Result
        {
            get;
        }
    }
}