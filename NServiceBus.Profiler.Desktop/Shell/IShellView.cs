using DevExpress.Xpf.Bars;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public interface IShellView
    {
        void ChangeTheme(string name);
        BarManager GetMenuManager();
    }
}