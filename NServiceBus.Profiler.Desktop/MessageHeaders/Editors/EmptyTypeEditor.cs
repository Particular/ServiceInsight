using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace Particular.ServiceInsight.Desktop.MessageHeaders.Editors
{
    public class EmptyTypeEditor : ITypeEditor
    {
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            return new TextBlock();
        }
    }
}