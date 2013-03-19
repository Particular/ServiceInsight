using System.Collections.Generic;
using System.Windows;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.ViewModels;
using Caliburn.PresentationFramework.Views;

namespace NServiceBus.Profiler.Desktop.ScreenManager
{
    public interface IWindowManagerEx : IWindowManager
    {
        MessageBoxResult ShowMessageBox(string message, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None, bool enableDontAsk = false, string help = "", MessageChoice defaultChoice = MessageChoice.OK);
    }

    public class WindowManagerEx : DefaultWindowManager, IWindowManagerEx
    {
        private static readonly IDictionary<MessageChoice, MessageBoxResult> MessageOptionsMaps;
        private static readonly IDictionary<MessageBoxImage, MessageIcon> MessageIconsMaps;

        static WindowManagerEx()
        {
            MessageOptionsMaps = new Dictionary<MessageChoice, MessageBoxResult>
            {
                {MessageChoice.OK, MessageBoxResult.OK},
                {MessageChoice.Cancel, MessageBoxResult.Cancel},
                {MessageChoice.Yes, MessageBoxResult.Yes},
                {MessageChoice.No, MessageBoxResult.No}
            };
            MessageIconsMaps = new Dictionary<MessageBoxImage, MessageIcon>
            {
                {MessageBoxImage.Exclamation, MessageIcon.Warning},
                {MessageBoxImage.Asterisk, MessageIcon.Information},
                {MessageBoxImage.Hand, MessageIcon.Error},
                {MessageBoxImage.Question, MessageIcon.Question}
            };
        }

        public WindowManagerEx(
            IViewLocator viewLocator, 
            IViewModelBinder viewModelBinder) : base(viewLocator, viewModelBinder)
        {
        }

        public MessageBoxResult ShowMessageBox(string message, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None, bool enableDontAsk = false, string help = "", MessageChoice defaultChoice = MessageChoice.OK)
        {
            var parent = Dialog.ActiveModalWindow;
            var choices = GetMessageChoice(button);
            var icon = MessageIconsMaps[image];
            var result = DialogViewModel.Show(parent, icon, caption, message, choices, help, enableDontAsk, defaultChoice);

            if (MessageOptionsMaps.ContainsKey(result))
                return MessageOptionsMaps[result];

            return MessageBoxResult.None;
        }

        private static MessageChoice GetMessageChoice(MessageBoxButton button)
        {
            MessageChoice choices;
            switch (button)
            {
                case MessageBoxButton.OK:
                    choices = MessageChoice.OK;
                    break;

                case MessageBoxButton.OKCancel:
                    choices = MessageChoice.Cancel | MessageChoice.OK;
                    break;

                case MessageBoxButton.YesNoCancel:
                    choices = MessageChoice.No | MessageChoice.Yes | MessageChoice.Cancel;
                    break;

                case MessageBoxButton.YesNo:
                    choices = MessageChoice.No | MessageChoice.Yes;
                    break;

                default:
                    choices = MessageChoice.None;
                    break;
            }
            return choices;
        }
    }
}