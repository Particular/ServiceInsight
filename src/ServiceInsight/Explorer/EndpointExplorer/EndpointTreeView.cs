namespace ServiceInsight.Explorer.EndpointExplorer
{
    using System.Windows;
    using System.Windows.Controls;

    public class EndpointTreeView : TreeView
    {
        TreeViewItem selectedTreeViewItem;
        bool syncInProgress;

        public EndpointTreeView()
        {
            SelectedItemChanged += TreeViewSelectedItemChanged;
        }

        void TreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (syncInProgress)
            {
                return;
            }

            SelectedItem = e.NewValue;
        }

        public new object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static new readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(EndpointTreeView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedItemPropertyChanged));

        static void SelectedItemPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is EndpointTreeView targetObject)
            {
                targetObject.syncInProgress = true;
                TreeViewItem tvi = targetObject.FindItemNode(targetObject.SelectedItem);
                if (tvi != null)
                {
                    if (targetObject.selectedTreeViewItem != null)
                    {
                        targetObject.selectedTreeViewItem.IsSelected = false;
                    }

                    tvi.IsSelected = true;
                    targetObject.selectedTreeViewItem = tvi;
                }
                targetObject.syncInProgress = false;
            }
        }

        public TreeViewItem FindItemNode(object item)
        {
            TreeViewItem node = null;
            foreach (object data in Items)
            {
                node = ItemContainerGenerator.ContainerFromItem(data) as TreeViewItem;
                if (node != null)
                {
                    if (data == item)
                    {
                        break;
                    }

                    node = FindItemNodeInChildren(node, item);
                    if (node != null)
                    {
                        break;
                    }
                }
            }

            return node;
        }

        protected TreeViewItem FindItemNodeInChildren(TreeViewItem parent, object item)
        {
            TreeViewItem node = null;
            bool isExpanded = parent.IsExpanded;
            if (!isExpanded) //Can't find child container unless the parent node is Expanded once
            {
                parent.IsExpanded = true;
                parent.UpdateLayout();
            }

            foreach (object data in parent.Items)
            {
                node = parent.ItemContainerGenerator.ContainerFromItem(data) as TreeViewItem;
                if (data == item && node != null)
                {
                    break;
                }

                node = FindItemNodeInChildren(node, item);
                if (node != null)
                {
                    break;
                }
            }

            if (node == null && parent.IsExpanded != isExpanded)
            {
                parent.IsExpanded = isExpanded;
            }

            if (node != null)
            {
                parent.IsExpanded = true;
            }

            return node;
        }
    }
}