namespace ServiceInsight.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    public class LicensePopup : Popup
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            CustomPopupPlacementCallback += PlaceContextMenu;
        }

        CustomPopupPlacement[] PlaceContextMenu(Size popupSize, Size targetSize, Point offset)
        {
            var ret = new List<CustomPopupPlacement>
            {
                new CustomPopupPlacement(new Point(0, targetSize.Height / 2), PopupPrimaryAxis.Horizontal)
            };

            return ret.ToArray();
        }
    }
}