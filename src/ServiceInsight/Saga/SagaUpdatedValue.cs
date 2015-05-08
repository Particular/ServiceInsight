namespace Particular.ServiceInsight.Desktop.Saga
{
    using System.Windows.Input;
    using Caliburn.Micro;
    using ExtensionMethods;

    public class ContentViewer : PropertyChangedBase
    {
        public ContentViewer()
        {
            SyntaxHighlighting = "JavaScript";
        }

        public bool Visible { get; set; }
        public string DisplayTitle { get; set; }
        public string Data { get; set; }
        public string SyntaxHighlighting { get; set; }
    }

    public class SagaUpdatedValue : PropertyChangedBase
    {
        const byte MaxValueLength = 30;

        public SagaUpdatedValue(string sagaName, string propertyName, string propertyValue)
        {
            Name = propertyName;
            NewValue = propertyValue;
            Viewer = new ContentViewer
            {
                DisplayTitle = sagaName,
            };
            ShowEntireNewContentCommand = this.CreateCommand(() =>
            {
                Viewer.Data = NewValue;
                Viewer.Visible = true;
            });
            ShowEntireOldContentCommand = this.CreateCommand(() =>
            {
                Viewer.Data = OldValue;
                Viewer.Visible = true;
            });
        }

        public string Name { get; private set; }
        public string NewValue { get; private set; }
        public string OldValue { get; private set; }
        public ContentViewer Viewer { get; private set; }

        public ICommand ShowEntireNewContentCommand { get; set; }
        public ICommand ShowEntireOldContentCommand { get; set; }

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