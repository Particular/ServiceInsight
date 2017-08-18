namespace ServiceInsight.Framework.Behaviors
{
    using System.Windows.Data;
    using DevExpress.Mvvm.UI.Interactivity;
    using DevExpress.Xpf.Editors;
    using ServiceInsight.ValueConverters;

    public class ComboBoxEditValuePatcherBehavior : Behavior<ComboBoxEdit>
    {
        // This behaviour fixes a bug where the user typing ':' into the SearchControl woudld
        // cause the application the crash. The behaviour sanitizes that input by wrapping
        // it in quotation marks (").
        protected override void OnAttached()
        {
            base.OnAttached();
            var binding = new Binding("SearchText")
            {
                RelativeSource = RelativeSource.TemplatedParent,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay,
                Converter = new SearchStringConverter()
            };
            AssociatedObject.SetBinding(BaseEdit.EditValueProperty, binding);
        }
    }
}