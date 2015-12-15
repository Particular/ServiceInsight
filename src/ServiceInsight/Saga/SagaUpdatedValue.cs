namespace ServiceInsight.Saga
{
    using System.Windows.Input;
    using Caliburn.Micro;
    using ExtensionMethods;

    public class SagaUpdatedValue : PropertyChangedBase
    {
        public const byte MaxValueLength = 30;

        public SagaUpdatedValue(string sagaName, string propertyName, string propertyValue)
        {
            SagaName = sagaName;
            Name = propertyName;
            NewValue = propertyValue;
            ShowEntireContentCommand = this.CreateCommand(ShowEntireContent);
        }

        public string SagaName { get; private set; }
        public string Name { get; private set; }
        public string NewValue { get; private set; }
        public string OldValue { get; private set; }

        public string EffectiveValue
        {
            get { return NewValue ?? OldValue; }
        }

        public ICommand ShowEntireContentCommand { get; set; }

        private void ShowEntireContent()
        {
            MessageContentVisible = true;
        }

        public bool MessageContentVisible { get; set; }

        public string Label
        {
            get
            {
                return string.Format("{0}{1}", Name, IsValueNew ? " (new)" : "");
            }
        }

        public string OldValueLink
        {
            get { return string.Format("{0} byte(s)", OldValue != null ? OldValue.Length : 0); }
        }

        public string NewValueLink
        {
            get { return string.Format("{0} byte(s)", NewValue != null ? NewValue.Length : 0); }
        }

        public bool ShouldDisplayOldValueLink
        {
            get { return !string.IsNullOrWhiteSpace(OldValue) && OldValue.Length > MaxValueLength; }
        }

        public bool ShouldDisplayNewValueLink
        {
            get { return !string.IsNullOrWhiteSpace(NewValue) && NewValue.Length > MaxValueLength; }
        }

        public bool IsValueChanged
        {
            get
            {
                return !string.IsNullOrEmpty(OldValue) && NewValue != OldValue;
            }
        }

        public bool IsValueNew
        {
            get
            {
                return string.IsNullOrEmpty(OldValue);
            }
        }

        public bool IsValueNotUpdated
        {
            get
            {
                return !IsValueChanged && !IsValueNew;
            }
        }

        public void UpdateOldValue(SagaUpdatedValue oldValueHolder)
        {
            OldValue = oldValueHolder != null ? oldValueHolder.NewValue : string.Empty;
        }
    }
}