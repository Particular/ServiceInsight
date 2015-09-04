namespace ServiceInsight.SequenceDiagram.Drawing
{
    using System.Windows;
    using System.Windows.Controls;

    public class UmlViewModel
    {
        public string Title { get; set; }
        public virtual int X { get; set; }
        public virtual int Y { get; set; }
    }

    public class UmlViewModelSelector : DataTemplateSelector
    {
        public override DataTemplate
            SelectTemplate(object item, DependencyObject container)
        {
            var presenter = container as ContentPresenter;

            var element = item as UmlViewModel;

            if (presenter == null || element == null)
            {
                return null;
            }

            if (element is ArrowViewModel)
            {
                return presenter.FindResource("ArrowViewTemplate") as DataTemplate;
            }

            if (element is HandlerViewModel)
            {
                return presenter.FindResource("HandlerViewTemplate") as DataTemplate;
            }

            return null;
        }
    }

}
