namespace ServiceInsight.Saga
{
    using System.Windows.Input;
    using Caliburn.Micro;
    using Pirac;

    public class SagaUpdatedValue : PropertyChangedBase
    {
        public const byte MaxValueLength = 30;

        public SagaUpdatedValue(string sagaName, string propertyName, string propertyValue)
        {
            SagaName = sagaName;
            Name = propertyName;
            NewValue = propertyValue;
            ShowEntireContentCommand = Command.Create(ShowEntireContent);
        }

        public string SagaName { get; }

        public string Name { get; }

        public string NewValue { get; }

        public string OldValue { get; private set; }

        public string EffectiveValue => NewValue ?? OldValue;

        public ICommand ShowEntireContentCommand { get; set; }

        void ShowEntireContent()
        {
            MessageContentVisible = true;
        }

        public bool MessageContentVisible { get; set; }

        public string Label => string.Format("{0}{1}", Name, IsValueNew ? " (new)" : "");

        public string OldValueLink => string.Format("{0} byte(s)", OldValue != null ? OldValue.Length : 0);

        public string NewValueLink => string.Format("{0} byte(s)", NewValue != null ? NewValue.Length : 0);

        public bool ShouldDisplayOldValueLink => !string.IsNullOrWhiteSpace(OldValue) && OldValue.Length > MaxValueLength;

        public bool ShouldDisplayNewValueLink => !string.IsNullOrWhiteSpace(NewValue) && NewValue.Length > MaxValueLength;

        public bool IsValueChanged => !string.IsNullOrEmpty(OldValue) && NewValue != OldValue;

        public bool IsValueNew => string.IsNullOrEmpty(OldValue);

        public bool IsValueNotUpdated => !IsValueChanged && !IsValueNew;

        public void UpdateOldValue(SagaUpdatedValue oldValueHolder)
        {
            OldValue = oldValueHolder != null ? oldValueHolder.NewValue : string.Empty;
        }
    }
}