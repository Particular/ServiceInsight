namespace Particular.ServiceInsight.Desktop.Options
{
    using System.ComponentModel;
    using Caliburn.PresentationFramework.Screens;
    using Core.Settings;
    using Settings;

    public class OptionsViewModel : Screen
    {
        private readonly ISettingsProvider _settingsProvider;

        public OptionsViewModel(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            DisplayName = "Options";
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            LoadSettings();
            IsModified = false;
        }

        private void LoadSettings()
        {
            Application = _settingsProvider.GetSettings<ProfilerSettings>();
            UsageReporting = _settingsProvider.GetSettings<ReportingSettings>();
            Application.PropertyChanged += OnSettingChanged;
        }

        private void OnSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            IsModified = true;
        }

        public ProfilerSettings Application
        {
            get; set;
        }

        public ReportingSettings UsageReporting
        {
            get; set;
        }

        public bool IsModified
        {
            get; set;
        }

        public void Save()
        {
            _settingsProvider.SaveSettings(Application);
            _settingsProvider.SaveSettings(UsageReporting);
        }

        public void Exit()
        {
            TryClose(false);
        }
    }
}