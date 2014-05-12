namespace Particular.ServiceInsight.Desktop.MessageFlow
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    public class MessagePopupControl : Popup
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            CustomPopupPlacementCallback += placeContextMenu;
        }

        CustomPopupPlacement[] placeContextMenu(Size popupSize, Size targetSize, Point offset)
        {
            var ret = new List<CustomPopupPlacement>
            {
                new CustomPopupPlacement(new Point(0, targetSize.Height/2), PopupPrimaryAxis.Horizontal)
            };
            return ret.ToArray();
        }

        protected override void OnOpened(EventArgs e)
        {
            SubscribeChildEvents();
            base.OnOpened(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            UnsubscribeChildEvents();
            base.OnClosed(e);
        }

        void SubscribeChildEvents()
        {
            var actionPopup = (MessageActionPopup)Child;
            if (actionPopup != null)
            {
                actionPopup.RequestToClose += ClosePopup;
            }
        }

        void UnsubscribeChildEvents()
        {
            var actionPopup = (MessageActionPopup)Child;
            if (actionPopup != null)
            {
                actionPopup.RequestToClose -= ClosePopup;
            }
        }

        void ClosePopup()
        {
            IsOpen = false;
        }
    }
}