using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Caliburn.PresentationFramework.Screens;

namespace NServiceBus.Profiler.Desktop.ScreenManager
{
    public class DialogViewModel : Screen
    {
        private Dialog _view;

        internal static MessageChoice Show(Window parent, MessageIcon icon, string title, string content, MessageChoice choices, string help, bool enableDontAsk)
        {
            var window = new DialogViewModel();
            window.ShowDialog(parent, icon, title, content, choices, help, enableDontAsk);
            return window.Result;
        }

        private void ShowDialog(Window parent, MessageIcon icon, string title, string content, MessageChoice choices, string help, bool enableDontAsk)
        {
            if (IsSet(choices, MessageChoice.Yes | MessageChoice.OK) || (choices == MessageChoice.Help))
                throw new ArgumentException();

            _view = new Dialog { Owner = parent, DataContext = this };

            Icon = icon;
            Title = title;
            Content = content;
            Choices = new List<ICommand>();
            EnableDontAskAgain = enableDontAsk;
            Result = MessageChoice.None;

            if (!string.IsNullOrEmpty(help))
                HelpMessage = help;

            if (IsSet(choices, MessageChoice.Yes))
                Choices.Add(new ChoiceCommand(CloseCommand, true, false, "Yes", MessageChoice.Yes));

            if (IsSet(choices, MessageChoice.No))
                Choices.Add(new ChoiceCommand(CloseCommand, false, !IsSet(choices, MessageChoice.Cancel), "No", MessageChoice.No));

            if (IsSet(choices, MessageChoice.OK))
                Choices.Add(new ChoiceCommand(CloseCommand, true, !IsSet(choices, MessageChoice.Cancel), "OK", MessageChoice.OK));

            if (IsSet(choices, MessageChoice.Cancel))
                Choices.Add(new ChoiceCommand(CloseCommand, choices == MessageChoice.Cancel, true, "Cancel", MessageChoice.Cancel));

            if (IsSet(choices, MessageChoice.Help))
                Choices.Add(new ChoiceCommand(HelpCommand, choices == MessageChoice.Help, false, "Help", MessageChoice.Help));

            _view.ShowDialog();
        }

        private static bool IsSet(MessageChoice choices, MessageChoice bits)
        {
            return ((choices & bits) == bits);
        }

        private void HelpCommand(object target)
        {
            Show(_view, MessageIcon.None, Title, HelpMessage, MessageChoice.OK, null, false);
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
            _view.Close();
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