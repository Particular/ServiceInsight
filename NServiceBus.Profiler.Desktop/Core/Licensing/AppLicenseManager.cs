using System;
using System.Collections.Generic;
using System.Globalization;
using log4net;
using NServiceBus.Profiler.Desktop.Core.Settings;
using NServiceBus.Profiler.Desktop.Settings;
using Rhino.Licensing;

namespace NServiceBus.Profiler.Desktop.Core.Licensing
{
    public class AppLicenseManager : ILicenseManager
    {
        private const string InvalidLicenseVersionMessage = "Your license is valid for an older version of NServiceBus Profiler. If you are still within the 1 year upgrade protection period of your original license, you should have already received a new license and if you haven’t, please contact customer.care@nservicebus.com. If your upgrade protection has lapsed, you can renew it at http://www.nservicebus.com/PurchaseSupport.aspx.";
        private const string InvalidTrialPeriodMessage = "Trial period is not valid; Please contact Particular Software support for assistance";
        private const string LicenseTypeKey = "LicenseType";
        private const string LicenseVersionKey = "LicenseVersion";
        private const string DateFormat = "M/d/yyyy"; 
        private const int TrialDays = 30;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(ILicenseManager));
        private readonly LicenseSettings _licenseSettings;
        private readonly ISettingsProvider _settingsProvider;
        private readonly ICryptoService _cryptoService;
        private AbstractLicenseValidator _validator;

        public AppLicenseManager(
            ISettingsProvider settingsProvider, 
            ICryptoService cryptoService)
        {
            _settingsProvider = settingsProvider;
            _cryptoService = cryptoService;
            _licenseSettings = settingsProvider.GetSettings<LicenseSettings>();
            Initialize();
        }

        public void Initialize(string license = null)
        {
            _validator = CreateValidator(license);
            Validate(license);
        }

        public ProfilerLicense CurrentLicense { get; private set; }

        public bool TrialExpired { get; private set; }

        private void Validate(string license)
        {
            if (_validator != null)
            {
                try
                {
                    _validator.AssertValidLicense();

                    Logger.InfoFormat("Found a {0} license.", _validator.LicenseType);
                    Logger.InfoFormat("Registered to {0}", _validator.Name);
                    Logger.InfoFormat("Expires on {0}", _validator.ExpirationDate);
                    if ((_validator.LicenseAttributes != null) && (_validator.LicenseAttributes.Count > 0))
                        foreach (var licenseAttribute in _validator.LicenseAttributes)
                            Logger.InfoFormat("[{0}]: [{1}]", licenseAttribute.Key, licenseAttribute.Value);

                    ValidateLicenseVersion();
                    CreateLicense();
                    StoreLicense(license);
                }
                catch (LicenseExpiredException)
                {
                    TrialExpired = true;
                    Logger.Info("License has expired.");
                }
                catch (LicenseNotFoundException)
                {
                    Logger.Info("License could not be loaded.");
                }
                catch (LicenseFileNotFoundException)
                {
                    Logger.Info("License could not be loaded.");
                }
            }

            if (CurrentLicense == null)
            {
                Logger.Info("No valid license found.");
                CreateTrialLicense();
                ValidateTrialStartDate();
            }
        }

        private void StoreLicense(string license)
        {
            if (!string.IsNullOrEmpty(license))
            {
                LicenseDescriptor.License = license;
            }
        }

        private void StoreTrialStart(DateTime trialStart)
        {
            if (string.IsNullOrEmpty(_licenseSettings.HashedStartTrial))
            {
                _licenseSettings.HashedStartTrial = _cryptoService.Encrypt(trialStart.ToString(DateFormat));
                _settingsProvider.SaveSettings(_licenseSettings);
            }
        }

        private void ValidateTrialStartDate()
        {
            if(TrialExpired)
                return;

            if (string.IsNullOrEmpty(LicenseDescriptor.TrialStart))
                throw new LicenseExpiredException(InvalidTrialPeriodMessage);
        }

        private void CreateTrialLicense()
        {
            var trialExpirationDate = TryGetExpirationDate();

            if (trialExpirationDate.HasValue && trialExpirationDate.Value > DateTime.UtcNow.Date)
            {
                Logger.InfoFormat("Trial for NServiceBus Profiler v{0} is still active, trial expires on {1}.",
                                   LicenseDescriptor.SoftwareVersion, 
                                   trialExpirationDate.Value.ToLocalTime().ToShortDateString());

                Logger.InfoFormat("Configuring NServiceBus Profiler to run in trial mode.");

                CurrentLicense = new ProfilerLicense
                {
                    LicenseType = ProfilerLicenseTypes.Trial,
                    ExpirationDate = trialExpirationDate.Value,
                    Version = LicenseDescriptor.SoftwareVersion,
                    RegisteredTo = ProfilerLicense.UnRegisteredUser
                };

                StoreTrialStart(ParseDateString(LicenseDescriptor.TrialStart));
            }
            else
            {
                Logger.InfoFormat("Trial for NServiceBus Profiler v{0} has expired.", LicenseDescriptor.SoftwareVersion);
                Logger.Warn("Falling back to run in Trial license mode.");

                TrialExpired = true;
            }
        }

        private DateTime? TryGetExpirationDate()
        {
            try
            {
                return GetTrialExpirationDate();
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Error("Could not read from the registry. Because we didn't find a license file we assume the trial has expired.", ex);
                return null;
            }
        }

        private DateTime GetTrialExpirationDate()
        {
            var trialExpirationDate = DateTime.UtcNow.Date;

            if (LicenseDescriptor.TrialStart == null)
            {
                TrialExpired = true;
            }
            else
            {
                var trialStartDate = ParseDateString(LicenseDescriptor.TrialStart);
                trialExpirationDate = trialStartDate.Date.AddDays(TrialDays);
            }

            return trialExpirationDate;
        }

        private static DateTime ParseDateString(string date)
        {
            try
            {
                return DateTime.ParseExact(date, DateFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new InvalidLicenseException(InvalidTrialPeriodMessage);
            }
        }

        public int GetRemainingTrialDays()
        {
            var now = DateTime.UtcNow.Date;
            var expiration = TryGetExpirationDate();

            if (!expiration.HasValue)
                return 0;

            var remainingDays = (expiration - now).Value.Days;

            return remainingDays > 0 ? remainingDays : 0;
        }

        private AbstractLicenseValidator CreateValidator(string license = null)
        {
            if (license == null && !string.IsNullOrEmpty(LicenseDescriptor.License))
            {
                Logger.InfoFormat(@"Using embeded license found in registry [{0}\License].", LicenseDescriptor.RegistryKey);
                license = LicenseDescriptor.License;
            }

            return new StringLicenseValidator(LicenseDescriptor.PublicKey, license);
        }

        private void ValidateLicenseVersion()
        {
            if (_validator.LicenseType == LicenseType.None)
                return;

            if (_validator.LicenseAttributes.ContainsKey(LicenseVersionKey))
            {
                try
                {
                    var semver = LicenseDescriptor.ApplicationVersion;
                    var licenseVersion = Version.Parse(_validator.LicenseAttributes[LicenseVersionKey]);
                    if (licenseVersion >= semver)
                        return;
                }
                catch (Exception exception)
                {
                    throw new InvalidLicenseException(InvalidLicenseVersionMessage, exception);
                }
            }

            throw new InvalidLicenseException(InvalidLicenseVersionMessage);
        }

        private void CreateLicense()
        {
            CurrentLicense = new ProfilerLicense();

            switch (_validator.LicenseType)
            {
                case LicenseType.None:
                    CurrentLicense.LicenseType = ProfilerLicenseTypes.Trial;
                    break;
                case LicenseType.Standard:
                    SetLicenseType(ProfilerLicenseTypes.Standard);
                    break;
                case LicenseType.Trial:
                    SetLicenseType(ProfilerLicenseTypes.Trial);
                    break;
                default:
                    Logger.Error(string.Format("Got unexpected license type [{0}], setting Basic1 free license type.", _validator.LicenseType), null);
                    CurrentLicense.LicenseType = ProfilerLicenseTypes.Trial;
                    break;
            }

            CurrentLicense.ExpirationDate = _validator.ExpirationDate;
            ConfigureLicenseBasedOnAttribute(_validator.LicenseAttributes);
        }

        private void ConfigureLicenseBasedOnAttribute(IDictionary<string, string> attributes)
        {
            CurrentLicense.Version = attributes[LicenseVersionKey];
            CurrentLicense.RegisteredTo = _validator.Name;
        }

        private void SetLicenseType(string defaultLicenseType)
        {
            if ((_validator.LicenseAttributes == null) ||
                (!_validator.LicenseAttributes.ContainsKey(LicenseTypeKey)) ||
                (string.IsNullOrEmpty(_validator.LicenseAttributes[LicenseTypeKey])))
            {
                CurrentLicense.LicenseType = defaultLicenseType;
            }
            else
            {
                CurrentLicense.LicenseType = _validator.LicenseAttributes[LicenseTypeKey];
            }
        }
    }
}