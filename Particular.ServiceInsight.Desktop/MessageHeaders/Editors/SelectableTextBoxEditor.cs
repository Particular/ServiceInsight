using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Editors;
using ExceptionHandler;
using ExceptionHandler.Wpf;
using Particular.ServiceInsight.Desktop.ExtensionMethods;
using Particular.ServiceInsight.Desktop.Models;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace Particular.ServiceInsight.Desktop.MessageHeaders.Editors
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
            var editor = new ButtonEdit();
            var button = new ButtonInfo {GlyphKind = GlyphKind.User };

            button.Click += (s, e) => OnCopyValue(editor);
            button.Content = new Image
            {
                Source = Properties.Resources.Clipboard.ToBitmapImage(), 
                Width = 16, 
                Height = 16,
                Stretch = Stretch.Uniform
            };

            editor.Buttons.Add(button);
            editor.AllowDefaultButton = false;
            editor.ShowBorder = false;
            editor.IsReadOnly = false;
            editor.Mask = GetDisplayFormat(propertyItem);
            editor.MaskType = GetMaskType(propertyItem);
            editor.MaskUseAsDisplayFormat = true;
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

        private MaskType GetMaskType(PropertyItem propertyItem)
        {
            if (propertyItem.Value is DateTime)
            {
                return MaskType.DateTime;
            }

            return MaskType.None;
        }

        private string GetDisplayFormat(PropertyItem propertyItem)
        {
            if (propertyItem.Value is DateTime)
            {
                return HeaderInfo.MessageDateFormat;
            }

            return null;
        }

        private void OnCopyValue(BaseEdit editor)
        {
            if (editor.DisplayText != null)
            {
                _clipboard.CopyTo(editor.DisplayText);
            }
        }
    }
}