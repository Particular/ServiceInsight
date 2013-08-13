using System.Collections.ObjectModel;
using System.Windows;

namespace Particular.ServiceInsight.Desktop.Options
{
    public partial class EditableListBox
    {
        public EditableListBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<string>), typeof(EditableListBox), new PropertyMetadata(OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs basevalue)
        {
            ((EditableListBox) d).OnItemsSourceChanged();
        }

        protected void OnItemsSourceChanged()
        {
            List.ItemsSource = ItemsSource;
        }

        public ObservableCollection<string> ItemsSource
        {
            get { return (ObservableCollection<string>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private void OnRemoveSelectedItem(object sender, RoutedEventArgs e)
        {
            var selectedItem = List.SelectedItem as string;
            if (selectedItem != null && ItemsSource.Contains(selectedItem))
            {
                ItemsSource.Remove(selectedItem);
            }
        }

        private void OnAddNewItem(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewItem.Text))
            {
                ItemsSource.Add(NewItem.Text);
                NewItem.Clear();
            }
        }
    }
}
