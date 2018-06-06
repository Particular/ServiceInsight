namespace ServiceInsight.Framework.Commands
{
    using MessageFlow;
    using Models;
    using Settings;
    using UI.ScreenManager;

    public class ShowExceptionCommand : BaseCommand
    {
        IWindowManagerEx windowManager;
        readonly ISettingsProvider settingsProvider;

        public ShowExceptionCommand(IWindowManagerEx windowManager, ISettingsProvider settingsProvider)
        {
            this.windowManager = windowManager;
            this.settingsProvider = settingsProvider;
        }

        public override void Execute(object parameter)
        {
            var selectedMessage = (StoredMessage)parameter;
            var model = new ExceptionDetailViewModel(settingsProvider, new ExceptionDetails(selectedMessage));

            windowManager.ShowModalDialog(model, true);
        }
    }
}