namespace ServiceInsight.ExtensionMethods
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    public static class ElementExtensions
    {
        public static T TryFindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            var parentObject = GetParentObject(child);
            if (parentObject == null)
            {
                return null;
            }

            var parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }

            return TryFindParent<T>(parentObject);
        }

        public static DependencyObject GetParentObject(this DependencyObject child)
        {
            if (child == null)
            {
                return null;
            }

            var contentElement = child as ContentElement;
            if (contentElement != null)
            {
                var parent = ContentOperations.GetParent(contentElement);
                if (parent != null)
                {
                    return parent;
                }

                var fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            var frameworkElement = child as FrameworkElement;
            if (frameworkElement != null)
            {
                var parent = frameworkElement.Parent;
                if (parent != null)
                {
                    return parent;
                }
            }

            return VisualTreeHelper.GetParent(child);
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
            {
                yield break;
            }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T)
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
            for (var dependencyObject = element; dependencyObject != null; dependencyObject = LogicalTreeHelper.GetParent(dependencyObject) ?? VisualTreeHelper.GetParent(dependencyObject))
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