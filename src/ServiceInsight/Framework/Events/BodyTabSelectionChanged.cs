namespace ServiceInsight.Framework.Events
{
    public class BodyTabSelectionChanged
    {
        public bool IsSelected { get; }

        public BodyTabSelectionChanged(bool isSelected)
        {
            IsSelected = isSelected;
        }
    }
}