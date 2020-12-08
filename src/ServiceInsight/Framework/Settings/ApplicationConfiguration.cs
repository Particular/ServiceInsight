namespace ServiceInsight.Framework.Settings
{
    using System;
    using System.Configuration;

    public static class ApplicationConfiguration
    {
        public static bool SkipCertificateValidation { get; private set; }
        public static int ConfigListenerPort { get; private set; }

        public static void Initialize()
        {
            SkipCertificateValidation = GetSkipCertificateValidation();
            ConfigListenerPort = GetConfigListenerPort();
        }

        private static int GetConfigListenerPort()
        {
            var port = ConfigurationManager.AppSettings["ConfigListenerPort"];

            try
            {
                return Convert.ToInt32(port);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException("Application setting 'ConfigListenerPort' cannot be converted to type Int32.", ex);
            }
        }

        private static bool GetSkipCertificateValidation()
        {
            var skipCertificateValidationValue = ConfigurationManager.AppSettings["SkipCertificateValidation"];

            try
            {
                return Convert.ToBoolean(skipCertificateValidationValue);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException("Application setting 'SkipCertificateValidation' cannot be converted to type Boolean.", ex);
            }
        }
    }
}
