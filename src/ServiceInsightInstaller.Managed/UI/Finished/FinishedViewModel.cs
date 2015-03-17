namespace ServiceInsightInstaller.Managed.UI.Finished
{
    using System.Windows;
    using Caliburn.Micro;

    class FinishedViewModel : Screen
    {
        public void Finish()
        {
            Application.Current.Shutdown();
        }
    }
}