namespace NServiceBus.Profiler.Desktop.Options
{
    public interface IOptionsView
    {
    }

    public partial class OptionsView : IOptionsView
    {
        public OptionsView()
        {
            InitializeComponent();
        }
    }
}
