namespace Particular.Licensing
{
    using System;
    using Microsoft.Win32;

    public class RegistryLicenseStore
    {
        public RegistryLicenseStore()
        {
            keyPath = @"SOFTWARE\ParticularSoftware";
            keyName = "License";
            regKey = Registry.CurrentUser;

        }

        public RegistryLicenseStore(string keyPath, string keyName, RegistryKey regKey)
        {
            this.keyPath = keyPath;
            this.keyName = keyName;
            this.regKey = regKey;
        }

        public bool TryReadLicense(out string license)
        {
            try
            {
                using (var registryKey = Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    if (registryKey == null)
                    {
                        throw new Exception(string.Format("CreateSubKey for '{0}' returned null. Do you have permission to write to this key", FullPath));
                    }

                    license = (string)registryKey.GetValue("License", null);

                    return !string.IsNullOrEmpty(license);
                }
            }
            catch (UnauthorizedAccessException exception)
            {
                throw new Exception(string.Format("Failed to access '{0}'. Do you have permission to write to this key?", FullPath), exception);
            }
        }


        public void StoreLicense(string license)
        {
            try
            {
                using (var registryKey = regKey.CreateSubKey(keyPath))
                {
                    if (registryKey == null)
                    {
                        throw new Exception( string.Format("CreateSubKey for '{0}' returned null. Do you have permission to write to this key", keyPath));
                    }

                    registryKey.SetValue(keyName, license, RegistryValueKind.String);
                }
            }
            catch (UnauthorizedAccessException exception)
            {
                throw new Exception(string.Format("Failed to access '{0}'. Do you have permission to write to this key?", FullPath), exception);
            }
        }

        string FullPath
        {
            get { return string.Format("{0} : {1} : {2}", regKey.Name, keyPath, keyName); }
        }

        string keyPath;
        string keyName;
        RegistryKey regKey;
    }
}