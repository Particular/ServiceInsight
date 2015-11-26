namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Threading;

    [ContentProperty("Items")]
    public class DiagramControl : ListBox, IDiagram
    {
        bool isLoaded;

        public static string ItemHostPart = "ItemsHost";
        public static string DiagramSurfacePart = "DiagramSurface";

        public static readonly DependencyProperty LayoutManagerProperty = DependencyProperty.Register("LayoutManager", typeof(ILayoutManager), typeof(DiagramControl), new PropertyMetadata());
        public static readonly DependencyProperty ShowCoordinatesProperty = DependencyProperty.Register("ShowCoordinates", typeof(bool), typeof(DiagramControl), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowGridProperty = DependencyProperty.Register("ShowGrid", typeof(bool), typeof(DiagramControl), new PropertyMetadata(true));

        static DiagramControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramControl), new FrameworkPropertyMetadata(typeof(DiagramControl)));
            
        }

        public DiagramControl()
        {
            SelectionChanged += OnSelectionChanged;
            LayoutManager = new SequenceDiagramLayoutManager();
            Loaded += (sender, args) => OnControlLoaded();
            IsVisibleChanged += (sender, args) => PerformLayout();
            SizeChanged += (s, a) => PerformLayout();
        }

        public ILayoutManager LayoutManager
        {
            get { return (ILayoutManager)GetValue(LayoutManagerProperty); }
            set { SetValue(LayoutManagerProperty, value); }
        }

        public bool ShowGrid
        {
            get { return (bool)GetValue(ShowGridProperty); }
            set { SetValue(ShowGridProperty, value); }
        }

        public bool ShowCoordinates
        {
            get { return (bool)GetValue(ShowCoordinatesProperty); }
            set { SetValue(ShowCoordinatesProperty, value); }
        }

        public DiagramItemCollection DiagramItems
        {
            get { return ItemsSource as DiagramItemCollection; }
        }

        public DiagramVisualItem GetItemFromContainer(DiagramItem item)
        {
            if (item == null) return null;
            return (DiagramVisualItem)ItemContainerGenerator.ContainerFromItem(item);
        }

        void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var diagramItem = (DiagramItem)e.AddedItems[0];
                var itemToBringIntoView = diagramItem.GetFocusableItem();
                if (itemToBringIntoView != null)
                {
                    var visualItem = GetItemFromContainer(itemToBringIntoView);
                    visualItem?.BringIntoView();
                }
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DiagramVisualItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DiagramVisualItem();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ItemContainerGenerator.StatusChanged += OnGeneratorStatusChanged;
        }

        void OnGeneratorStatusChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(PerformLayout));
        }

        void OnControlLoaded()
        {
            isLoaded = true;
        }

        void PerformLayout()
        {
            if (isLoaded && IsVisible)
            {
                LayoutManager.PerformLayout(this);
            }
        }
    }
}