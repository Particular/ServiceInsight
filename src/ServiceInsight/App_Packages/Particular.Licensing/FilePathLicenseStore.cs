namespace Particular.Licensing
{
    using System.IO;

    class FilePathLicenseStore
    {
        public void StoreLicense(string filePath, string license)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (directory != null)
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, license);
        }
    }
}