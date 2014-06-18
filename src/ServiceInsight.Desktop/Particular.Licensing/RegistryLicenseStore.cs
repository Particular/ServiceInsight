// ReSharper disable once CheckNamespace
namespace Particular.Licensing
{
    using System;
    using System.Linq;
    using System.Security;
    using Microsoft.Win32;

    public class RegistryLicenseStore
    {
        public bool TryReadLicense(out string license)
        {
            try
            {
                using (var registryKey = FindLicenceRegistryKey())
                {
                    if (registryKey == null)
                    {
                        license = null;
                        return false;
                    }

                    var licenseValue = registryKey.GetValue("License", null);

                    var licenseValues = licenseValue as string[];
                    if (licenseValues != null)
                    {
                        license = string.Join(" ", licenseValues);
                    }
                    else
                    {
                        license = (string)licenseValue;
                    }

                    return !string.IsNullOrEmpty(license);
                }
            }
            catch (SecurityException exception)
            {
                throw new Exception(string.Format("Failed to access '{0} : {1} : {2}'. Do you have permission to read this key?", Registry.CurrentUser, DefaultKeyPath, DefaultKeyName), exception);
            }
        }

        public void StoreLicense(string license)
        {
            try
            {
                using (var registryKey = Registry.CurrentUser.CreateSubKey(DefaultKeyPath))
                {
                    if (registryKey == null)
                    {
                        throw new Exception(string.Format("CreateSubKey for '{0}' returned null. Do you have permission to write to this key", DefaultKeyPath));
                    }

                    registryKey.SetValue("License", license, RegistryValueKind.String);
                }
            }
            catch (UnauthorizedAccessException exception)
            {
                throw new Exception(string.Format("Failed to access '{0} : {1} : {2}'. Do you have permission to write to this key?", Registry.CurrentUser, DefaultKeyPath, DefaultKeyName), exception);
            }
        }

        static RegistryKey FindLicenceRegistryKey()
        {
            var hklm = RegistryKey.OpenBaseKey(
                            RegistryHive.LocalMachine,
                            Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);

            // 4.5
            var key = FindLocalMachineOrCurrentUserKey(hklm, @"SOFTWARE\ParticularSoftware");
            if (key != null) return key;

            // 4.0
            key = FindLocalMachineOrCurrentUserKey(hklm, @"SOFTWARE\NServiceBus\4.0");
            if (key != null) return key;

            // 4.1
            key = FindLocalMachineOrCurrentUserKey(hklm, @"SOFTWARE\NServiceBus\4.1");
            if (key != null) return key;

            // 4.2
            key = FindLocalMachineOrCurrentUserKey(hklm, @"SOFTWARE\NServiceBus\4.2");
            if (key != null) return key;

            // 4.3-4.4
            key = FindLocalMachineOrCurrentUserKey(hklm, @"SOFTWARE\ParticularSoftware\NServiceBus");
            if (key != null) return key;

            // 3.3
            key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\NServiceBus\3.3");
            if (key != null)
            {
                if (key.GetValueNames().Contains("License"))
                    return key;

                key.Dispose();
            }

            return null;
        }

        static RegistryKey FindLocalMachineOrCurrentUserKey(RegistryKey hklm, string name)
        {
            var key = hklm.OpenSubKey(name);
            if (key != null)
            {
                if (key.GetValueNames().Contains("License"))
                    return key;

                key.Dispose();
            }

            key = Registry.CurrentUser.OpenSubKey(name);
            if (key != null)
            {
                if (key.GetValueNames().Contains("License"))
                    return key;

                key.Dispose();
            }

            return null;
        }

        const string DefaultKeyPath = @"SOFTWARE\ParticularSoftware";
        const string DefaultKeyName = "License";
    }
}