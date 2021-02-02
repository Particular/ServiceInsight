namespace ServiceInsight.Saga
{
    using System;
    using Caliburn.Micro;
    using Humanizer;

    public class SagaUpdatedValue : PropertyChangedBase
    {
        public const byte MaxValueLength = 30;

        public SagaUpdatedValue(string sagaName, string propertyName, string propertyValue)
        {
            SagaName = sagaName;
            Name = propertyName;
            NewValue = propertyValue;
        }

        public string SagaName { get; }

        public string Name { get; }

        public string NewValue { get; }

        public string OldValue { get; private set; }

        public string EffectiveValue => NewValue ?? OldValue;

        public string Label => $"{Name}{(IsValueNew ? " (new)" : "")}";

        public string OldValueLink => $"({"byte".ToQuantity(OldValue?.Length ?? 0)})";

        public string NewValueLink => $"({"byte".ToQuantity(NewValue?.Length ?? 0)})";

        public bool ShouldDisplayOldValueLink => ShouldDisplayAsLink(OldValue);

        bool ShouldDisplayAsLink(string value)
        {
            if (value != null)
            {
                if (Guid.TryParse(value, out _))
                {
                    return false;
                }
            }
            return !string.IsNullOrWhiteSpace(value) && value.Length > MaxValueLength;
        }

        public bool ShouldDisplayNewValueLink => ShouldDisplayAsLink(NewValue);

        public bool IsValueChanged => !string.IsNullOrEmpty(OldValue) && NewValue != OldValue;

        public bool IsValueNew => string.IsNullOrEmpty(OldValue);

        public bool IsValueNotUpdated => !IsValueChanged && !IsValueNew;

        public void UpdateOldValue(SagaUpdatedValue oldValueHolder)
        {
            OldValue = oldValueHolder != null ? oldValueHolder.NewValue : string.Empty;
        }
    }
}