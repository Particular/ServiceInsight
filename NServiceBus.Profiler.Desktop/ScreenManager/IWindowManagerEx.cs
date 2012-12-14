using System.Windows;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.ViewModels;
using Caliburn.PresentationFramework.Views;

namespace NServiceBus.Profiler.Desktop.ScreenManager
{
    public interface IWindowManagerEx : IWindowManager
    {
        MessageBoxResult ShowMessageBox(string message, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None, bool enableDontAsk = false, string help = "");
    }

    public class WindowManagerEx : DefaultWindowManager, IWindowManagerEx
    {
        public WindowManagerEx(
            IViewLocator viewLocator, 
            IViewModelBinder viewModelBinder) : base(viewLocator, viewModelBinder)
        {
        }

        public MessageBoxResult ShowMessageBox(string message, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None, bool enableDontAsk = false, string help = "")
        {
            var parent = Dialog.ActiveModalWindow;
            var choices = GetMessageChoice(button);
            var icon = GetMessageIcon(image);
            var result = DialogViewModel.Show(parent, icon, caption, message, choices, help, enableDontAsk);

            switch (result)
            {
                case MessageChoice.OK:
                    return MessageBoxResult.OK;

                case MessageChoice.Cancel:
                    return MessageBoxResult.Cancel;

                case MessageChoice.Yes:
                    return MessageBoxResult.Yes;

                case MessageChoice.No:
                    return MessageBoxResult.No;
            }

            return MessageBoxResult.None;
        }

        private static MessageIcon GetMessageIcon(MessageBoxImage image)
        {
            MessageIcon icon;
            switch (image)
            {
                case MessageBoxImage.Exclamation:
                    icon = MessageIcon.Warning;
                    break;

                case MessageBoxImage.Asterisk:
                    icon = MessageIcon.Information;
                    break;

                case MessageBoxImage.Hand:
                    icon = MessageIcon.Error;
                    break;

                case MessageBoxImage.Question:
                    icon = MessageIcon.Question;
                    break;

                default:
                    icon = MessageIcon.None;
                    break;
            }
            return icon;
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