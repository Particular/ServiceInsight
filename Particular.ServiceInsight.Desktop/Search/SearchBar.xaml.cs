using System.Windows.Input;

namespace Particular.ServiceInsight.Desktop.Search
{
    /// <summary>
    /// Interaction logic for SearchBar.xaml
    /// </summary>
    public partial class SearchBar : ISearchBarView
    {
        public SearchBar()
        {
            InitializeComponent();
        }

        private void OnSearchKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Return || e.Key == Key.Enter) && Model != null)
            {
                Model.Search();
            }
        }

        private ISearchBarViewModel Model
        {
            get { return DataContext as ISearchBarViewModel; }
        }
    }
}
