namespace Particular.ServiceInsight.Desktop.Shell.Menu
{
    using System.Windows;
    using System.Windows.Controls;

    public class MenuItemDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var app = Application.Current;
            var menu = (IMenuItem)item;

            if(app == null || menu == null) return null;

            if (menu.SubMenuItems.Count > 0) return app.Resources["SubButtonContextMenu"] as DataTemplate;
            if (menu.IsCheckable) return app.Resources["CheckButtonContextMenu"] as DataTemplate;
            if (menu.IsSeparator) return app.Resources["SeperatorContextMenu"] as DataTemplate;

            return app.Resources["ButtonContextMenu"] as DataTemplate;
        }
    }
}