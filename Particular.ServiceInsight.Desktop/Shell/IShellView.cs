using DevExpress.Xpf.Bars;

namespace Particular.ServiceInsight.Desktop.Shell
{
    public interface IShellView : IPersistableLayout
    {
        void ChangeTheme(string name);
        BarManager GetMenuManager();
    }
}