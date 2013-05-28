namespace NServiceBus.Profiler.Desktop.ScreenManager
{
    public class FileDialogModel
    {
        public FileDialogModel()
        {
            CheckFileExists = true;
            CheckPathExists = true;
            Multiselect = false;
            FilterIndex = 1;
        }

        public bool CheckFileExists { get; set; }
        public bool CheckPathExists { get; set; }
        public string DefaultExtension { get; set; }
        public string InitialDirectory { get; set; }
        public string Filter { get; set; }
        public string Title { get; set; }
        public bool Multiselect { get; set; }
        public int FilterIndex { get; set; }
    }
}