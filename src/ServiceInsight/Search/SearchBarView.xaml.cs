namespace ServiceInsight.Search
{
    using System.Windows.Input;

    public partial class SearchBarView
    {
        public SearchBarView()
        {
            InitializeComponent();
        }

        async void OnSearchKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Return || e.Key == Key.Enter) && Model != null)
            {
                await Model.Search();
            }
        }

        SearchBarViewModel Model => DataContext as SearchBarViewModel;
    }
}
