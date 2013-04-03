using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Editors;
using ExceptionHandler;
using ExceptionHandler.Wpf;
using NServiceBus.Profiler.Common.ExtensionMethods;
using NServiceBus.Profiler.Desktop.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace NServiceBus.Profiler.Desktop.MessageHeaders.Editors
{
    public class SelectableTextBoxEditor : ITypeEditor
    {
        private readonly IClipboard _clipboard;

        public SelectableTextBoxEditor()
        {
            _clipboard = new WpfClipboard();
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            var button = new ButtonInfo {GlyphKind = GlyphKind.User };
            button.Click += (s, e) => OnCopyPropertyValue(propertyItem);
            button.Content = new Image
            {
                Source = Resources.Clipboard.ToBitmapImage(), 
                Width = 16, 
                Height = 16,
                Stretch = Stretch.Uniform
            };

            var editor = new ButtonEdit();
            editor.Buttons.Add(button);
            editor.AllowDefaultButton = false;
            editor.ShowBorder = false;
            editor.IsReadOnly = false;
            editor.EditMode = EditMode.Standalone;
            editor.Background = Brushes.Transparent;
            editor.TextWrapping = TextWrapping.NoWrap;
            editor.TextTrimming = TextTrimming.WordEllipsis;

            var binding = new Binding("Value");
            binding.Source = propertyItem;
            binding.ValidatesOnExceptions = true;
            binding.ValidatesOnDataErrors = true;
            binding.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(editor, BaseEdit.EditValueProperty, binding);

            return editor;
        }

        private void OnCopyPropertyValue(PropertyItem property)
        {
            if (property.Value != null)
            {
                _clipboard.CopyTo(property.Value.ToString());
            }
        }
    }
}