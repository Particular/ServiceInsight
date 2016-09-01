namespace ServiceInsight.LogWindow
{
    using System;
    using System.Collections.Specialized;
    using System.Reactive.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Interactivity;
    using ReactiveUI;

    public class LoggingRichTextBoxBehavior : Behavior<RichTextBox>
    {
        public static readonly DependencyProperty LogDataProperty = DependencyProperty.Register(
            "LogData", typeof(object), typeof(LoggingRichTextBoxBehavior), new FrameworkPropertyMetadata(default(object), LogDataChanged));

        static void LogDataChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((LoggingRichTextBoxBehavior)dependencyObject).OnLogDataChanged(dependencyPropertyChangedEventArgs);
        }

        private Paragraph paragraph;
        IDisposable logSubscription;

        public object LogData
        {
            get { return GetValue(LogDataProperty); }
            set { SetValue(LogDataProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject.Document == null)
            {
                AssociatedObject.Document = new FlowDocument();
            }

            paragraph = new Paragraph();

            AssociatedObject.Document.Blocks.Clear();
            AssociatedObject.Document.Blocks.Add(paragraph);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            paragraph = null;
        }

        void OnLogDataChanged(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var logs = dependencyPropertyChangedEventArgs.NewValue as ReactiveList<LogMessage>;
            if (logs == null)
            {
                paragraph.Inlines.Clear();
                return;
            }

            if (logSubscription != null)
            {
                logSubscription.Dispose();
                logSubscription = null;
            }

            paragraph.Inlines.Clear();
            foreach (var log in logs)
            {
                paragraph.Inlines.Add(new Run(log.Log) { Foreground = log.Brush, FontWeight = log.Weight });
            }

            logSubscription = logs.Changed
                .Where(e => e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Reset)
                .Subscribe(ProcessChange);
        }

        void ProcessChange(NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                var log = (LogMessage)args.NewItems[0];

                paragraph.Inlines.Add(new Run(log.Log) { Foreground = log.Brush, FontWeight = log.Weight });
            }

            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                paragraph.Inlines.Clear();
            }
        }
    }
}