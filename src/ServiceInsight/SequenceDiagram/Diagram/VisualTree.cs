namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System.Windows;
    using System.Windows.Media;

    public static class VisualTree
    {
        public static T FindFirstVisualChild<T>(DependencyObject parentElement) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parentElement);
            if (count == 0)
                return null;

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parentElement, i);

                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    var result = FindFirstVisualChild<T>(child);
                    if (result != null)
                        return result;

                }
            }
            return null;
        }
    }
}