namespace ServiceInsight.Framework.Settings
{
    using System;
    using System.Configuration;

    public static class ApplicationConfiguration
    {
        public static bool SkipCertificateValidation { get; private set; }
        public static int ConversationPageSize { get; private set; }

        public static void Initialize()
        {
            SkipCertificateValidation = GetValue(nameof(SkipCertificateValidation), Convert.ToBoolean);
            ConversationPageSize = GetValue(nameof(ConversationPageSize), Convert.ToInt32);
        }

        static T GetValue<T>(string key, Func<object, T> convert)
        {
            try
            {
                var value = ConfigurationManager.AppSettings[key];
                return convert(value);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException($"Application setting '{key}' cannot be converted to type '{typeof(T)}'.", ex);
            }
        }
    }
}
