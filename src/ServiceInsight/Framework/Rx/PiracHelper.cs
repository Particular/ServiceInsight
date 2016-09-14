namespace ServiceInsight.Framework.Rx
{
    using Pirac;

    class PiracHelper : BindableObject
    {
        public void PropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName, null, null);
        }
    }
}