namespace ServiceInsight.Framework.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;
    using Explorer;

    public class BindableSelectedItemBehavior : Behavior<TreeView>
    {
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(BindableSelectedItemBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));

        static TreeViewItem FindItemNode(TreeView treeView, object item)
        {
            TreeViewItem node = null;
            foreach (object data in treeView.Items)
            {
                var temp = treeView.ItemContainerGenerator.ContainerFromItem(data);
                _ = temp;
                node = treeView.ItemContainerGenerator.ContainerFromItem(data) as TreeViewItem;
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
        static TreeViewItem FindItemNodeInChildren(TreeViewItem parent, object item)
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

        static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var treeView = ((BindableSelectedItemBehavior)sender).AssociatedObject;
            if (e.NewValue is ExplorerItem)
            {
                var treeViewItem = FindItemNode(treeView, e.NewValue);
                //walk the whole TreeView looking for a node bound to the incoming item and mark as selected
                treeViewItem.SetValue(TreeViewItem.IsSelectedProperty, true);
            }

            /*if (e.OldValue is ExplorerItem)
            {
                //walk the whole TreeView looking for a node bound to the incoming item and mark as not selected
            }

            if (e.NewValue is TreeViewItem item)
            {
                item.SetValue(TreeViewItem.IsSelectedProperty, true);
            }*/
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
            {
                AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
            }
        }

        void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue;
        }
    }
}