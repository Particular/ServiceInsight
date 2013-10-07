using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Editors;

namespace NServiceBus.Profiler.Desktop.MessageProperties.Editors
{
//    public class ResizableDropDownEditor : ITypeEditor
//    {
//        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
//        {
//            var editor = new MemoEdit();
//            editor.AcceptsReturn = false;
//            editor.ShowBorder = false;
//            editor.IsReadOnly = true;
//            editor.ShowIcon = false;
//            editor.Background = Brushes.Transparent;
//            editor.TextWrapping = TextWrapping.NoWrap;
//            editor.MemoTextWrapping = TextWrapping.WrapWithOverflow;
//            editor.MemoVerticalScrollBarVisibility = ScrollBarVisibility.Auto;
//            editor.Height = 22;
//            editor.PopupWidth = 500;
//            editor.PopupFooterButtons = PopupFooterButtons.Close;
//            
//            var binding = new Binding("Value");
//            binding.Source = propertyItem;
//            binding.ValidatesOnExceptions = true;
//            binding.ValidatesOnDataErrors = true;
//            binding.Mode = BindingMode.OneWay;
//            BindingOperations.SetBinding(editor, BaseEdit.EditValueProperty, binding);
//
//            return editor;
//        }
//    }
}