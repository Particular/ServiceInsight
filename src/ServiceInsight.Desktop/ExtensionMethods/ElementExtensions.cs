namespace Particular.ServiceInsight.Desktop.ExtensionMethods
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    public static class ElementExtensions
    {
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child != null && child is T)
                {
                    yield return (T)child;
                }

                foreach (var childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }

        public static T GetResource<T>(this DependencyObject element, object key)
        {
            for (DependencyObject dependencyObject = element; dependencyObject != null; dependencyObject = (LogicalTreeHelper.GetParent(dependencyObject) ?? VisualTreeHelper.GetParent(dependencyObject)))
            {
                var frameworkElement = dependencyObject as FrameworkElement;
                if (frameworkElement != null)
                {
                    if (frameworkElement.Resources.Contains(key))
                    {
                        return (T)frameworkElement.Resources[key];
                    }
                }
                else
                {
                    var frameworkContentElement = dependencyObject as FrameworkContentElement;
                    if (frameworkContentElement != null && frameworkContentElement.Resources.Contains(key))
                    {
                        return (T)frameworkContentElement.Resources[key];
                    }
                }
            }
            if (Application.Current != null && Application.Current.Resources.Contains(key))
            {
                return (T)Application.Current.Resources[key];
            }
            return default(T);
        }
    }
}