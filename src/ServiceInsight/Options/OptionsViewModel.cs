namespace ServiceInsight.Options
{
    using System.ComponentModel;
    using Caliburn.Micro;
    using ServiceInsight.Framework.Settings;
    using Settings;

    public class OptionsViewModel : Screen
    {
        public OptionsViewModel(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;

            DisplayName = "Options";
        }

        public ProfilerSettings Application { get; set; }

        public bool IsModified { get; set; }

        protected override void OnActivate()
        {
            base.OnActivate();
            LoadSettings();
            IsModified = false;
        }

        void LoadSettings()
        {
            Application = settingsProvider.GetSettings<ProfilerSettings>();

            var managementConfig = settingsProvider.GetSettings<ServiceControlSettings>();
            Application.DefaultLastUsedServiceControl = string.Format("http://localhost:{0}/api", managementConfig.Port);

            Application.PropertyChanged += OnSettingChanged;
        }

        void OnSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            IsModified = true;
        }

        public void Save()
        {
            settingsProvider.SaveSettings(Application);
        }

        public void Exit()
        {
            TryClose(false);
        }

        ISettingsProvider settingsProvider;
    }
}