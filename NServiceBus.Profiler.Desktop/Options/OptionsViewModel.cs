using System.ComponentModel;
using Caliburn.PresentationFramework.Screens;
using Particular.ServiceInsight.Desktop.Core.Settings;
using Particular.ServiceInsight.Desktop.Settings;

namespace Particular.ServiceInsight.Desktop.Options
{
    public class OptionsViewModel : Screen
    {
        private readonly ISettingsProvider _settingsProvider;

        public OptionsViewModel(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            DisplayName = "Options...";
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            LoadSettings();
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
            TryClose(true);
        }

        public void Close()
        {
            TryClose(false);
        }
    }
}