namespace ServiceInsight.MessagePayloadViewer
{
    using Caliburn.Micro;
    using ServiceInsight.Framework.Settings;
    using ServiceInsight.Shell;

    public class MessagePayloadViewModel : Screen
    {
        ISettingsProvider settingsProvider;

        public string Content { get; }

        public virtual IPersistableLayout View { get; private set; }

        public MessagePayloadViewModel(ISettingsProvider settingsProvider, string content)
        {
            this.settingsProvider = settingsProvider;
            Content = content;
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
    }
}