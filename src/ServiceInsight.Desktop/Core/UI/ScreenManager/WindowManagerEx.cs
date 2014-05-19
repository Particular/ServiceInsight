namespace Particular.ServiceInsight.Desktop.Core.UI.ScreenManager
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Forms;
    using Caliburn.Micro;

    public class WindowManagerEx : WindowManager, IWindowManagerEx
    {
        static IDictionary<MessageChoice, MessageBoxResult> MessageOptionsMaps;
        static IDictionary<MessageBoxImage, MessageIcon> MessageIconsMaps;
        static IDictionary<DialogResult, bool?> DialogResultMaps;

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
            DialogResultMaps = new Dictionary<DialogResult, bool?>
            {
                {DialogResult.Abort,  null },
                {DialogResult.Cancel, null },
                {DialogResult.Ignore, null },
                {DialogResult.None,   null },
                {DialogResult.Retry,  null },
                {DialogResult.Yes,    true },
                {DialogResult.OK,     true },
                {DialogResult.No,     false},
            };
        }

        public FileDialogResult OpenFileDialog(FileDialogModel model)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.CheckFileExists = model.CheckFileExists;
                dialog.CheckPathExists = model.CheckPathExists;
                dialog.DefaultExt = model.DefaultExtension;
                dialog.Filter = model.Filter;
                dialog.FilterIndex = model.FilterIndex;
                dialog.Multiselect = model.Multiselect;
                dialog.Title = model.Title;

                var dialogResult = dialog.ShowDialog();
                var result = new FileDialogResult(DialogResultMaps[dialogResult], dialog.FileNames);

                return result;
            }
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

        public bool? ShowDialog<T>() where T : class
        {
            var screen = IoC.Get<T>(); // Yick!
            return base.ShowDialog(screen, null);
        }

        static MessageChoice GetMessageChoice(MessageBoxButton button)
        {
            //TODO: Use map to avoid switch/case
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