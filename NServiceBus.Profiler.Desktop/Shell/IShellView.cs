using DevExpress.Xpf.Bars;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public interface IShellView : IPersistableLayout
    {
        void ChangeTheme(string name);
        BarManager GetMenuManager();
    }
}