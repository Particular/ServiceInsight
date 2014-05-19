namespace Particular.ServiceInsight.Desktop.Options
{
    using System.ComponentModel;
    using Caliburn.Micro;
    using Core.Settings;
    using Settings;

    public class OptionsViewModel : Screen
    {
        ISettingsProvider settingsProvider;

        public OptionsViewModel(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;

            DisplayName = "Options";
        }

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
            Application.PropertyChanged += OnSettingChanged;
        }

        void OnSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            IsModified = true;
        }

        public ProfilerSettings Application
        {
            get;
            set;
        }

        public ReportingSettings UsageReporting
        {
            get;
            set;
        }

        public bool IsModified
        {
            get;
            set;
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
    }
}