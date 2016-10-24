namespace ServiceInsight.MessageFlow
{
    using System.Windows;
    using Framework.Rx;
    using Models;
    using ServiceInsight.Framework.Settings;
    using Shell;

    public class ExceptionDetailViewModel : RxScreen
    {
        ISettingsProvider settingsProvider;

        public virtual IPersistableLayout View { get; private set; }

        public ExceptionDetails Exception { get; set; }

        public ExceptionDetailViewModel(ISettingsProvider settingsProvider, ExceptionDetails exception = null)
        {
            this.settingsProvider = settingsProvider;
            DisplayName = "Exception Details";
            Exception = exception;
        }

        protected override void OnViewAttached(object view)
        {
            View = (IPersistableLayout)view;
        }

        protected override void OnViewLoaded(FrameworkElement view)
        {
            RestoreLayout();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            SaveLayout();
        }

        void SaveLayout()
        {
            View.OnSaveLayout(settingsProvider);
        }

        void RestoreLayout()
        {
            View.OnRestoreLayout(settingsProvider);
        }

        public string FormattedSource => $"{Exception.ExceptionType} (@{Exception.Source})";
    }
}