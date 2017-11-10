namespace ServiceInsight.Framework.Settings
{
    using System;
    using System.Configuration;

    public static class ApplicationConfiguration
    {
        public static bool SkipCertificateValidation { get; private set; }

        public static void Initialize()
        {
            SkipCertificateValidation = GetSkipCertificateValidation();
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
