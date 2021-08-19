﻿namespace ServiceInsight.Framework.UI.ScreenManager
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using Caliburn.Micro;
    using ServiceInsight.Framework.Commands;

    public class ServiceInsightWindowManager : WindowManager, IServiceInsightWindowManager
    {
        static IDictionary<MessageChoice, MessageBoxResult> messageOptionsMaps;
        static IDictionary<MessageBoxImage, MessageIcon> messageIconsMaps;
        static IDictionary<DialogResult, bool?> dialogResultMaps;

        static ServiceInsightWindowManager()
        {
            messageOptionsMaps = new Dictionary<MessageChoice, MessageBoxResult>
            {
                { MessageChoice.OK, MessageBoxResult.OK },
                { MessageChoice.Cancel, MessageBoxResult.Cancel },
                { MessageChoice.Yes, MessageBoxResult.Yes },
                { MessageChoice.No, MessageBoxResult.No }
            };
            messageIconsMaps = new Dictionary<MessageBoxImage, MessageIcon>
            {
                { MessageBoxImage.None, MessageIcon.None },
                { MessageBoxImage.Exclamation, MessageIcon.Warning },
                { MessageBoxImage.Asterisk, MessageIcon.Information },
                { MessageBoxImage.Hand, MessageIcon.Error },
                { MessageBoxImage.Question, MessageIcon.Question }
            };
            dialogResultMaps = new Dictionary<DialogResult, bool?>
            {
                { DialogResult.Abort,  null },
                { DialogResult.Cancel, null },
                { DialogResult.Ignore, null },
                { DialogResult.None,   null },
                { DialogResult.Retry,  null },
                { DialogResult.Yes,    true },
                { DialogResult.OK,     true },
                { DialogResult.No,     false },
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
                var result = new FileDialogResult(dialogResultMaps[dialogResult], dialog.FileNames);

                return result;
            }
        }

        public MessageBoxResult ShowMessageBox(string message, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None, bool enableDontAsk = false, string help = "", MessageChoice defaultChoice = MessageChoice.OK)
        {
            var parent = Dialog.ActiveModalWindow;
            var choices = GetMessageChoice(button);
            var icon = messageIconsMaps[image];
            var result = DialogViewModel.Show(parent, icon, caption, message, choices, help, enableDontAsk, defaultChoice);

            if (messageOptionsMaps.ContainsKey(result))
            {
                return messageOptionsMaps[result];
            }

            return MessageBoxResult.None;
        }

        protected override Window CreateWindow(object rootModel, bool isDialog, object context, IDictionary<string, object> settings)
        {
            var window = base.CreateWindow(rootModel, isDialog, context, settings);

            if (isDialog)
            {
                window.InputBindings.Add(new KeyBinding(new CloseWindowCommand(window), new KeyGesture(Key.Escape)));
            }

            return window;
        }

        public bool? ShowDialog<T>() where T : class
        {
            var screen = IoC.Get<T>(); // Yick!
            return ShowDialog(screen);
        }

        public bool? ShowModalDialog<T>(T model) where T : class
        {
            var settings = new Dictionary<string, object>
            {
                { "ResizeMode", ResizeMode.NoResize }
            };

            return ShowDialog(model, null, settings);
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
