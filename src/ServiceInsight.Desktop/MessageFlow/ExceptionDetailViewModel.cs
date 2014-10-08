namespace Particular.ServiceInsight.Desktop.MessageFlow
{
    using Caliburn.Micro;
    using Core.Settings;
    using Models;
    using Shell;

    public class ExceptionDetailViewModel : Screen
    {
        ISettingsProvider settingsProvider;

        public virtual IPersistableLayout View { get; private set; }

        public ExceptionDetails Exception { get; set; }

        public ExceptionDetailViewModel(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
            DisplayName = "Exception Details";
        }

        public ExceptionDetailViewModel(ExceptionDetails exception)
        {
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

        public virtual void Deactivate(bool close)
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

        public string FormattedSource
        {
            get
            {
                return string.Format("{0} (@{1})", Exception.ExceptionType, Exception.Source);
            }
        }
    }
}