namespace ServiceInsight.ExtensionMethods
{
    using System;
    using System.IO;
    using Anotar.Serilog;
    using DevExpress.Xpf.Bars;
    using DevExpress.Xpf.Docking;
    using DevExpress.Xpf.Grid;

    public static class LayoutExtensions
    {
        public static string GetLayout(this BarManager barManager)
        {
            return TrySaveLayout(barManager);
        }

        public static string GetLayout(this DockLayoutManager dockLayoutManager)
        {
            return TrySaveLayout(dockLayoutManager);
        }

        public static string GetLayout(this GridControl gridControl)
        {
            return TrySaveLayout(gridControl);
        }

        public static void RestoreLayout(this BarManager barManager, Stream stream)
        {
            TryRestoreLayout(barManager, stream);
        }

        public static void RestoreLayout(this DockLayoutManager dockLayoutManager, Stream stream)
        {
            TryRestoreLayout(dockLayoutManager, stream);
        }

        public static void RestoreLayout(this GridControl gridControl, Stream stream)
        {
            TryRestoreLayout(gridControl, stream);
        }

        static string TrySaveLayout(dynamic control) //Lack of common interface :(
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    control.SaveLayoutToStream(ms);
                    return ms.GetAsString();
                }
            }
            catch (Exception ex)
            {
                LogTo.Information(ex, "Failed to save the layout, reason is: {ex}", ex);
                return null;
            }
        }

        static void TryRestoreLayout(dynamic control, Stream layout)
        {
            if (layout == null)
            {
                return;
            }

            try
            {
                control.RestoreLayoutFromStream(layout);
            }
            catch (Exception ex)
            {
                LogTo.Information(ex, "Failed to restore layout, reason is: {ex}", ex);
            }
        }
    }
}