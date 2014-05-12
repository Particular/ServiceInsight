namespace Particular.ServiceInsight.Desktop.Search
{
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for SearchBar.xaml
    /// </summary>
    public partial class SearchBar : ISearchBarView
    {
        public SearchBar()
        {
            InitializeComponent();
        }

        void OnSearchKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Return || e.Key == Key.Enter) && Model != null)
            {
                Model.Search();
            }
        }

        ISearchBarViewModel Model
        {
            get { return DataContext as ISearchBarViewModel; }
        }
    }
}
