namespace ServiceInsight.Framework.UI.ScreenManager
{
    using System.Collections.Generic;
    using System.Linq;

    public class FileDialogResult
    {
        public FileDialogResult(bool? result)
            : this(result, Enumerable.Empty<string>())
        {
        }

        public FileDialogResult(bool? result, IEnumerable<string> fileNames)
        {
            Result = result;
            FileNames = fileNames;
        }

        public bool? Result { get; private set; }
        public IEnumerable<string> FileNames { get; private set; }
        public string FileName { get { return FileNames.FirstOrDefault(); } }

        public static implicit operator bool?(FileDialogResult result)
        {
            return result.Result;
        }
    }
}