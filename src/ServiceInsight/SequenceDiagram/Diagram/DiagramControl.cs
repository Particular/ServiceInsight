namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    [ContentProperty("Items")]
    public class DiagramControl : Control, IDiagram
    {
        private ListBox itemshost;

        public static string ItemHostPart = "ItemsHost";

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(DiagramItemCollection), typeof(DiagramControl), new PropertyMetadata(OnItemsChanged));
        public static readonly DependencyProperty LayoutManagerProperty = DependencyProperty.Register("LayoutManager", typeof(ILayoutManager), typeof(DiagramControl), new PropertyMetadata());
        public static readonly DependencyProperty AreaHeightProperty = DependencyProperty.Register("AreaHeight", typeof(double), typeof(DiagramControl), new PropertyMetadata(500d));
        public static readonly DependencyProperty AreaWidthProperty = DependencyProperty.Register("AreaWidth", typeof(double), typeof(DiagramControl), new PropertyMetadata(500d));
        public static readonly DependencyProperty ShowCoordinatesProperty = DependencyProperty.Register("ShowCoordinates", typeof(bool), typeof(DiagramControl), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowGridProperty = DependencyProperty.Register("ShowGrid", typeof(bool), typeof(DiagramControl), new PropertyMetadata(true));

        static DiagramControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramControl), new FrameworkPropertyMetadata(typeof(DiagramControl)));
        }

        public DiagramControl()
        {
            LayoutManager = new SequenceDiagramLayoutManager();
            Items = new DiagramItemCollection();
            Loaded += (sender, args) => OnControlLoaded();
        }

        public DiagramItemCollection Items
        {
            get { return (DiagramItemCollection)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
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

        public double AreaHeight
        {
            get { return (double)GetValue(AreaHeightProperty); }
            set { SetValue(AreaHeightProperty, value); }
        }

        public double AreaWidth
        {
            get { return (double)GetValue(AreaWidthProperty); }
            set { SetValue(AreaWidthProperty, value); }
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
                oldValue.CollectionChanged -= OnCollectionChanged;
            }

            if (newValue != null)
            {
                newValue.CollectionChanged += OnCollectionChanged;
            }
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PerformLayout();
        }


        public T GetContainerFromItem<T>(DiagramItem item) where T : UIElement
        {
            if (itemshost == null) throw new Exception("Can not get items from the container.");

            var container = itemshost.ItemContainerGenerator.ContainerFromItem(item);
            return (T)container;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            itemshost = Template.FindName(ItemHostPart, this) as ListBox;
        }

        void OnControlLoaded()
        {
            PerformLayout();
        }

        void PerformLayout()
        {
            LayoutManager.PerformLayout(this);
        }
    }
}