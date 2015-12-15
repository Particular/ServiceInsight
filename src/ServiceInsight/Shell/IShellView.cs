namespace ServiceInsight.Shell
{
    public interface IShellView : IPersistableLayout
    {
        void ChangeTheme(string name);
        void SelectTab(string name);
    }
}