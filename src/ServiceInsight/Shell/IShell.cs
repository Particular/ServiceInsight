namespace ServiceInsight.Shell
{
    public interface IShell : IPersistableLayout
    {
        void ChangeTheme(string name);

        void SelectTab(string name);
    }
}