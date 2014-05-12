namespace Particular.ServiceInsight.Desktop.LogWindow
{
    using log4net;
    using log4net.Repository.Hierarchy;

    public interface ILogWindowView
    {
        void Initialize();
        void Clear();
        void Copy();
    }

    public partial class LogWindowView : ILogWindowView
    {
        RichTextBoxAppender appender;


        public LogWindowView()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            appender = new RichTextBoxAppender(LogContainer);
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.AddAppender(appender);
        }

        public void Clear()
        {
            appender.Clear();
        }

        public void Copy()
        {
            LogContainer.Copy();
        }
    }
}
