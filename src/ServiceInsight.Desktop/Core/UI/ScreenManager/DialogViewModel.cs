namespace Particular.ServiceInsight.Desktop.Core.UI.ScreenManager
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;
    using Caliburn.PresentationFramework.Screens;

    public class DialogViewModel : Screen
    {
        private Dialog view;

        internal static MessageChoice Show(Window parent, MessageIcon icon, string title, string content, MessageChoice choices, string help, bool enableDontAsk, MessageChoice defaultChoice)
        {
            var window = new DialogViewModel();
            window.ShowDialog(parent, icon, title, content, choices, help, enableDontAsk, defaultChoice);
            return window.Result;
        }

        private void ShowDialog(Window parent, MessageIcon icon, string title, string content, MessageChoice choices, string help, bool enableDontAsk, MessageChoice defaultChoice)
        {
            if (IsSet(choices, MessageChoice.Yes | MessageChoice.OK) || (choices == MessageChoice.Help))
                throw new ArgumentException();

            view = CreateWindow(parent);

            Icon = icon;
            Title = title;
            Content = content;
            Choices = new List<ICommand>();
            EnableDontAskAgain = enableDontAsk;
            Result = MessageChoice.None;

            if (!string.IsNullOrEmpty(help))
                HelpMessage = help;

            if (IsSet(choices, MessageChoice.Yes))
                Choices.Add(new ChoiceCommand(CloseCommand, defaultChoice == MessageChoice.Yes, false, "Yes", MessageChoice.Yes));

            if (IsSet(choices, MessageChoice.No))
                Choices.Add(new ChoiceCommand(CloseCommand, defaultChoice == MessageChoice.No, !IsSet(choices, MessageChoice.Cancel), "No", MessageChoice.No));

            if (IsSet(choices, MessageChoice.OK))
                Choices.Add(new ChoiceCommand(CloseCommand, choices == MessageChoice.OK || defaultChoice == MessageChoice.OK, !IsSet(choices, MessageChoice.Cancel), "OK", MessageChoice.OK));

            if (IsSet(choices, MessageChoice.Cancel))
                Choices.Add(new ChoiceCommand(CloseCommand, choices == MessageChoice.Cancel || defaultChoice == MessageChoice.Cancel, true, "Cancel", MessageChoice.Cancel));

            if (IsSet(choices, MessageChoice.Help))
                Choices.Add(new ChoiceCommand(HelpCommand, choices == MessageChoice.Help || defaultChoice == MessageChoice.Help, false, "Help", MessageChoice.Help));

            view.ShowDialog();
        }

        private Dialog CreateWindow(Window parent)
        {
            var dialog = new Dialog { Owner = parent, DataContext = this };

            if (parent == null)
            {
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            else
            {
                dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            return dialog;
        }

        private static bool IsSet(MessageChoice choices, MessageChoice bits)
        {
            return ((choices & bits) == bits);
        }

        private void HelpCommand(object target)
        {
            Show(view, MessageIcon.None, Title, HelpMessage, MessageChoice.OK, null, false, MessageChoice.Help);
        }

        private void CloseCommand(object target)
        {
            var command = target as ChoiceCommand;
            if (command != null)
                Close(command.Result);
        }

        public void Close(MessageChoice closeResult)
        {
            Result = closeResult;
            view.Close();
        }

        public virtual ICollection<ICommand> Choices
        {
            get; private set;
        }

        public virtual string Content
        {
            get; set;
        }

        public virtual bool DontAskAgain
        {
            get; set;
        }

        public virtual bool EnableDontAskAgain
        {
            get; private set;
        }

        public virtual string HelpMessage
        {
            get; private set;
        }

        public virtual MessageIcon Icon
        {
            get; private set;
        }

        public virtual MessageChoice Result
        {
            get; private set;
        }

        public virtual string Title
        {
            get; set;
        }

        public virtual bool ShowIcon
        {
            get { return Icon != MessageIcon.None; }
        }
    }
}