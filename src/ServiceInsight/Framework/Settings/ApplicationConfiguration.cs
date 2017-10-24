namespace ServiceInsight.Framework.Settings
{
    using System;
    using System.Configuration;

    public class ApplicationConfiguration
    {
        public static bool SkipCertificateValidation { get; private set; }

        public static void Initialize()
        {
            SkipCertificateValidation = GetSkipCertificateValidation();
        }

        private static bool GetSkipCertificateValidation()
        {
            try
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["SkipCertificateValidation"]);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Application setting 'SkipCertificateValidation' is either missing or cannot be converted to type boolean.", ex);
            }
        }
    }
}