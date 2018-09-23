namespace ServiceInsight.Shell
{
    using System.Windows.Input;
    using Caliburn.Micro;
    using ServiceInsight.Framework;

    public class LicensePopupViewModel : Screen
    {
        public LicensePopupViewModel()
        {
            ContactUs = Command.Create(OnContactUsClicked);
            ManageLicense = Command.Create(OnManageLicense);
        }

        private void OnManageLicense()
        {
        }

        private void OnContactUsClicked()
        {
        }

        public ICommand ManageLicense { get; set; }

        public ICommand ContactUs { get; set; }
    }
}