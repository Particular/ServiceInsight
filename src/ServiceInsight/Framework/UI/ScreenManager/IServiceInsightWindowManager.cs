namespace ServiceInsight.Framework.UI.ScreenManager
{
    using System.Windows;
    using Caliburn.Micro;

    public interface IServiceInsightWindowManager : IWindowManager
    {
        FileDialogResult OpenFileDialog(FileDialogModel model);

        MessageBoxResult ShowMessageBox(string message, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None, bool enableDontAsk = false, string help = "", MessageChoice defaultChoice = MessageChoice.OK);

        bool? ShowDialog<T>() where T : class;

        bool? ShowModalDialog<T>(T model) where T : class;
    }
}