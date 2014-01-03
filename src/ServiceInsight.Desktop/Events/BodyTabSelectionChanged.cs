namespace NServiceBus.Profiler.Desktop.Events
{
    public class BodyTabSelectionChanged
    {
        public bool IsSelected { get; private set; }

        public BodyTabSelectionChanged(bool isSelected)
        {
            IsSelected = isSelected;
        }
    }
}