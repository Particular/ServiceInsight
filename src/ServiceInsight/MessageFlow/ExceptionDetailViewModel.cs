namespace ServiceInsight.MessageFlow
{
    using Caliburn.Micro;
    using Models;
    using ServiceInsight.Framework.Settings;
    using Shell;

    public class ExceptionDetailViewModel : Screen
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

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            View = (IPersistableLayout)view;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
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

        public string FormattedSource => string.Format("{0} (@{1})", Exception.ExceptionType, Exception.Source);
    }
}