using System.ComponentModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Core.Settings;
using NServiceBus.Profiler.Desktop.Settings;

namespace NServiceBus.Profiler.Desktop.Options
{
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