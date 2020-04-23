namespace Particular.Licensing
{
    using System;
    using System.IO;

    using static System.Environment;

    static class LicenseFileLocationResolver
    {
        public static string ApplicationFolderLicenseFile { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "license.xml");

        public static string GetPathFor(SpecialFolder specialFolder, string licenseFileName = "license.xml")
        {
            var specialFolderPath = GetSpecialFolderPath(specialFolder);

            if (specialFolderPath == string.Empty)
            {
                specialFolderPath = AppDomain.CurrentDomain.BaseDirectory;
            }

            return Path.Combine(specialFolderPath, companyFolder, licenseFileName ?? string.Empty);
        }

        static string GetSpecialFolderPath(SpecialFolder specialFolder)
        {
            var path = GetFolderPath(specialFolder, SpecialFolderOption.DoNotVerify);

            if (path == string.Empty)
            {
                if (specialFolder == SpecialFolder.CommonApplicationData)
                {
                    path = GetEnvironmentVariable("PROGRAMDATA");
                }
                else if (specialFolder == SpecialFolder.LocalApplicationData)
                {
                    path = GetEnvironmentVariable("LOCALAPPDATA");
                }
            }

            return path ?? string.Empty;
        }

        const string companyFolder = "ParticularSoftware";
    }
}