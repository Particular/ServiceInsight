﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.ViewModels;
using Caliburn.PresentationFramework.Views;
using Application = System.Windows.Application;

namespace Particular.ServiceInsight.Desktop.ScreenManager
{
    public interface IDialogManager
    {
        FileDialogResult OpenFileDialog(FileDialogModel model);
    }

    public interface IWindowManagerEx : IWindowManager
    {
        MessageBoxResult ShowMessageBox(string message, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None, bool enableDontAsk = false, string help = "", MessageChoice defaultChoice = MessageChoice.OK);
        bool? ShowDialog<T>() where T : IScreen;
    }

    public class WindowManagerEx : DefaultWindowManager, IWindowManagerEx, IDialogManager
    {
        private readonly IScreenFactory _screenFactory;
        private static readonly IDictionary<MessageChoice, MessageBoxResult> MessageOptionsMaps;
        private static readonly IDictionary<MessageBoxImage, MessageIcon> MessageIconsMaps;
        private static readonly IDictionary<DialogResult, bool?> DialogResultMaps;

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

        public WindowManagerEx(
            IViewLocator viewLocator, 
            IViewModelBinder viewModelBinder,
            IScreenFactory screenFactory) : base(viewLocator, viewModelBinder)
        {
            _screenFactory = screenFactory;
        }

        public FileDialogResult OpenFileDialog(FileDialogModel model)
        {
            using(var dialog = new OpenFileDialog())
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

        public bool? ShowDialog<T>() where T : IScreen
        {
            var screen = _screenFactory.CreateScreen<T>();
            return base.ShowDialog(screen, null);
        }

        protected override Window EnsureWindow(object model, object view, bool isDialog)
        {
            var window = base.EnsureWindow(model, view, isDialog);
            window.ResizeMode = ResizeMode.NoResize;

            SetParentToMain(window);

            if (window.Owner == null)
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            return window;
        }

        private void SetParentToMain(Window window)
        {
            if (window.Owner == null &&
                Application.Current != null &&
                Application.Current.MainWindow != null)
            {
                window.Owner = Application.Current.MainWindow;
            }
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