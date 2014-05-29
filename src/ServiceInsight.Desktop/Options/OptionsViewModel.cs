namespace Particular.ServiceInsight.Desktop.Options
{
    using System.ComponentModel;
    using Caliburn.Micro;
    using Core.Settings;
    using Settings;

    public class OptionsViewModel : Screen
    {
        public OptionsViewModel(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;

            DisplayName = "Options";
        }

        public ProfilerSettings Application { get; set; }

        public ReportingSettings UsageReporting { get; set; }

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
            UsageReporting = settingsProvider.GetSettings<ReportingSettings>();

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
            settingsProvider.SaveSettings(UsageReporting);
        }

        public void Exit()
        {
            TryClose(false);
        }

        ISettingsProvider settingsProvider;
    }
}