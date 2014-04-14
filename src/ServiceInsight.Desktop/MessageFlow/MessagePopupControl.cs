using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    public class MessagePopupControl : Popup
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.CustomPopupPlacementCallback += new CustomPopupPlacementCallback(placeContextMenu);
        }

        private CustomPopupPlacement[] placeContextMenu(Size popupSize, Size targetSize, Point offset)
        {
            var ret = new List<CustomPopupPlacement>();
            var target = this.PlacementTarget as FrameworkElement;

            ret.Add(new CustomPopupPlacement(new Point(0, targetSize.Height / 2), PopupPrimaryAxis.Horizontal));

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

        private void SubscribeChildEvents()
        {
            var actionPopup = (MessageActionPopup)Child;
            if (actionPopup != null)
            {
                actionPopup.RequestToClose += ClosePopup;
            }
        }

        private void UnsubscribeChildEvents()
        {
            var actionPopup = (MessageActionPopup)Child;
            if (actionPopup != null)
            {
                actionPopup.RequestToClose -= ClosePopup;
            }
        }

        private void ClosePopup()
        {
            IsOpen = false;
        }
    }
}