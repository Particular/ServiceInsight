using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace NServiceBus.Profiler.Desktop.MessageHeaders.Editors
{
    public class EmptyTypeEditor : ITypeEditor
    {
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            return new TextBlock();
        }
    }
}