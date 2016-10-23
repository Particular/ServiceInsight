namespace ServiceInsight.Framework.Rx
{
    using System;
    using Pirac;

    public class RxScreen : ViewModelBase
    {
        public string DisplayName { get; set; }

        [Obsolete("Old CM Method")]
        protected void NotifyOfPropertyChange(string propertyName)
        {
            OnPropertyChanging(propertyName, null);
            OnPropertyChanged(propertyName, null, null);
        }
    }
}