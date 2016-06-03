namespace ServiceInsight.Options
{
    using System.Collections.ObjectModel;
    using System.Windows;

    public partial class EditableListBox
    {
        public EditableListBox()
        {
            InitializeComponent();
        }

        public static DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<string>), typeof(EditableListBox), new PropertyMetadata(OnItemsSourceChanged));

        static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs basevalue)
        {
            ((EditableListBox)d).OnItemsSourceChanged();
        }

        protected void OnItemsSourceChanged()
        {
            List.ItemsSource = ItemsSource;
        }

        public ObservableCollection<string> ItemsSource
        {
            get { return (ObservableCollection<string>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        void OnRemoveSelectedItem(object sender, RoutedEventArgs e)
        {
            var selectedItem = List.SelectedItem as string;
            if (selectedItem != null && ItemsSource.Contains(selectedItem))
            {
                ItemsSource.Remove(selectedItem);
            }
        }

        void OnAddNewItem(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewItem.Text))
            {
                ItemsSource.Add(NewItem.Text);
                NewItem.Clear();
            }
        }
    }
}
