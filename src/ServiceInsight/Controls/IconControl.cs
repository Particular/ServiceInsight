namespace Particular.ServiceInsight.Desktop.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    [TemplatePart(Name = "PART_Path", Type = typeof(Path))]
    public class IconControl : Control
    {
        static IconControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconControl), new FrameworkPropertyMetadata(typeof(IconControl)));
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data", typeof(Geometry), typeof(IconControl), new FrameworkPropertyMetadata(default(Geometry), (o, args) => ((IconControl)o).OnDataChanged()));

        public Geometry Data
        {
            get { return (Geometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private Path path;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            path = GetTemplateChild("PART_Path") as Path;

            OnDataChanged();
        }

        void OnDataChanged()
        {
            if (path != null)
                path.Data = Data;
        }
    }
}