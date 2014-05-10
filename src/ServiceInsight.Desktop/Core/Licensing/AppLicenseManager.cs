namespace Particular.ServiceInsight.Desktop.Core.Licensing
{
    using System;
    using App_Packages.Particular.Licensing;
    using log4net;

    public class AppLicenseManager
    {
        public AppLicenseManager()
        {
            string licenseText;

            if (licenseStore.TryReadLicense(out licenseText))
            {
                Exception ex;
                
                if (LicenseVerifier.TryVerify(licenseText, out ex))
                {
                    CurrentLicense = LicenseDeserializer.Deserialize(licenseText);

                    return;
                }
            }

            CurrentLicense = CreateTrialLicense();
        }

        public bool TryInstallLicense(string licenseText)
        {
            try
            {
                Exception verificationException;

                if (!LicenseVerifier.TryVerify(licenseText, out verificationException))
                {
                    return false;
                }

                CurrentLicense = LicenseDeserializer.Deserialize(licenseText);

                licenseStore.StoreLicense(licenseText);

                return true;
            }
            catch (Exception ex)
            {
                Logger.WarnFormat("Can't install license: {0}", ex);
                return false;
            }
        }

        public License CurrentLicense { get; private set; }

        public int GetRemainingTrialDays()
        {
            var now = DateTime.UtcNow.Date;

            var expiration = (CurrentLicense == null || CurrentLicense.ExpirationDate == null) ? 
                TrialStartDateStore.GetTrialStartDate().AddDays(14) : CurrentLicense.ExpirationDate.Value;

            var remainingDays = (expiration - now).Days;

            return remainingDays > 0 ? remainingDays : 0;
        }

        License CreateTrialLicense()
        {
            var trialStartDate = TrialStartDateStore.GetTrialStartDate();

            Logger.InfoFormat("Configuring ServiceInsight to run in trial mode.");

            return License.TrialLicense(trialStartDate);
        }


        public bool IsLicenseExpired()
        {
            return LicenseExpirationChecker.HasLicenseExpired(CurrentLicense);
        }

        RegistryLicenseStore licenseStore = new RegistryLicenseStore();

     
        static readonly ILog Logger = LogManager.GetLogger(typeof(AppLicenseManager));


    }
}