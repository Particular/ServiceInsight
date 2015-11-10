namespace Particular.ServiceInsight.Desktop.Framework.Commands
{
    using Settings;
    using UI.ScreenManager;
    using MessageFlow;
    using Models;

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
            
            windowManager.ShowDialog(model);
        }
    }
}