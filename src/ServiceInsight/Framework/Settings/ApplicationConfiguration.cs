namespace ServiceInsight.Framework.Settings
{
    using System;
    using System.Configuration;

    public class ApplicationConfiguration
    {
        public static bool SkipCertificateValidation => Convert.ToBoolean(ConfigurationManager.AppSettings["SkipCertificateValidation"]);
    }
}