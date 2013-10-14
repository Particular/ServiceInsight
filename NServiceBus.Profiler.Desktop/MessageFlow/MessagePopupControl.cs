using System;
using System.Windows.Controls.Primitives;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    public class MessagePopupControl : Popup
    {
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