namespace ServiceInsight.Framework.Settings
{
    using System;
    using System.Configuration;

    public class ApplicationConfiguration
    {
        public static bool SkipCertificateValidation { get; private set; }

        public static void Initialize()
        {
            SkipCertificateValidation = GetValue<bool>("SkipCertificateValidation");
        }

        private static T GetValue<T>(string key)
        {
            try
            {
                return (T)Convert.ChangeType(ConfigurationManager.AppSettings[key], typeof(T));
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"The value of '{key}' in app.config is not a valid {typeof(T).Name} type.", ex);
            }
        }
    }
}