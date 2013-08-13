using System;
using System.Reflection;
using Microsoft.Win32;

namespace Particular.ServiceInsight.Desktop.Core.Licensing
{
    public class LicenseDescriptor
    {
        public static string RegistryKey
        {
            get { return string.Format(@"SOFTWARE\ParticularSoftware\Profiler\{0}", ApplicationVersion.ToString(2)); }
        }

        public static Version ApplicationVersion
        {
            get
            {
                var assembyVersion = Assembly.GetExecutingAssembly().GetName().Version;
                return new Version(assembyVersion.Major, assembyVersion.Minor);
            }
        }

        public static string TrialStart
        {
            get
            {
                using (var registryKey = Registry.CurrentUser.OpenSubKey(RegistryKey))
                {
                    if (registryKey != null)
                    {
                        return (string)registryKey.GetValue("TrialStart", null);
                    }
                }

                return null;
            }
        }

        public static string License
        {
            get
            {
                using (var registryKey = Registry.CurrentUser.OpenSubKey(RegistryKey))
                {
                    if (registryKey != null)
                    {
                        return (string) registryKey.GetValue("License", null);
                    }
                }

                return null;
            }
            set
            {
                using (var registryKey = Registry.CurrentUser.CreateSubKey(RegistryKey))
                {
                    if (registryKey != null)
                    {
                        registryKey.SetValue("License", value, RegistryValueKind.String);
                    }
                }
            }
        }

        public static string SoftwareVersion
        {
            get { return ApplicationVersion.ToString(2); }
        }

        public static string PublicKey
        {
            get
            {
                return @"<RSAKeyValue><Modulus>spGPDNj14Rim0Og5I1I+F3O2TVjWwDAtSHr54VzhbAg3a+2KJkjgXpZs+BKvzPiI+mscZDroF2ykEHGLSNEb0XOw8NpLFOeRrUuFzE7SOWn2fg5ZhY2u/8QrUl7yX8uIp4mxfvnvHOT/iB5cDipHvHjwE+1ZzBslMgSXecolO4E=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            }
        }
    }
}