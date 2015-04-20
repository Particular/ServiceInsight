namespace Particular.ServiceInsight.Desktop.Search
{
    using System.Windows.Input;

    public partial class SearchBarView 
    {
        public SearchBarView()
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

        SearchBarViewModel Model
        {
            get { return DataContext as SearchBarViewModel; }
        }
    }
}
