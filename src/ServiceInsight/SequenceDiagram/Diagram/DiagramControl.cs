namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Markup;

    [ContentProperty("Items")]
    public class DiagramControl : ListBox, IDiagram
    {
        bool isLoaded;
        DiagramSurface surface;

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
            LayoutManager = new SequenceDiagramLayoutManager();
            Loaded += (sender, args) => OnControlLoaded();
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

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DiagramVisualItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DiagramVisualItem();
        }

        static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var diagram = (DiagramControl) d;
            diagram.OnItemsChanged(e.OldValue as DiagramItemCollection, e.NewValue as DiagramItemCollection);
        }

        void OnItemsChanged(DiagramItemCollection oldValue, DiagramItemCollection newValue)
        {
            if (oldValue != null)
            {
            }
        }

        public T GetContainerFromItem<T>(DiagramItem item) where T : UIElement
        {
            var container = ItemContainerGenerator.ContainerFromItem(item);
            return (T)container;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ItemContainerGenerator.StatusChanged += OnGeneratorStatusChanged;
            surface = (DiagramSurface) Template.FindName(DiagramSurfacePart, this);
        }

        void OnGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                PerformLayout();
            }
        }

        void OnControlLoaded()
        {
            isLoaded = true;
            PerformLayout();
        }

        void PerformLayout()
        {
            if (surface != null)
            {
                surface.InvalidateMeasure();
            }

            if (isLoaded)
            {
                LayoutManager.PerformLayout(this);
            }
        }
    }
}