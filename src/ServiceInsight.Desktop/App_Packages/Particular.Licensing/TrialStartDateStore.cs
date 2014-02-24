namespace Particular.Licensing
{
    using System;
    using System.Globalization;
    using Microsoft.Win32;

    static class TrialStartDateStore
    {
        public static DateTime GetTrialStartDate()
        {
            using (var registryKey = Registry.CurrentUser.CreateSubKey(StorageLocation))
            {
                // ReSharper disable PossibleNullReferenceException
                //CreateSubKey does not return null http://stackoverflow.com/questions/19849870/under-what-circumstances-will-registrykey-createsubkeystring-return-null
                var trialStartDateString = (string)registryKey.GetValue("TrialStart", null);
                // ReSharper restore PossibleNullReferenceException
                if (trialStartDateString == null)
                {
                    var trialStart = DateTime.UtcNow;
                    trialStartDateString = trialStart.ToString("yyyy-MM-dd");
                    registryKey.SetValue("TrialStart", trialStartDateString, RegistryValueKind.String);

                    return trialStart;
                }
                else
                {
                    var trialStartDate = DateTimeOffset.ParseExact(trialStartDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

                    return trialStartDate.Date;

                }
            }
        }

        public static string StorageLocation = @"SOFTWARE\ParticularSoftware";
    }
}