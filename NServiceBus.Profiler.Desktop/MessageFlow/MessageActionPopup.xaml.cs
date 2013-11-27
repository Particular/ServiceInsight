using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    /// <summary>
    /// Interaction logic for MessageActionPopup.xaml
    /// </summary>
    public partial class MessageActionPopup
    {
        public MessageActionPopup()
        {
            InitializeComponent();
        }

        private CustomPopupPlacement[] placeContextMenu(Size popupSize, Size targetSize, Point offset)
        {
            var ret = new List<CustomPopupPlacement>();
            var target = this.ContextMenu.PlacementTarget as FrameworkElement;

            ret.Add(new CustomPopupPlacement(new Point(target.ActualHeight + 5, 0), PopupPrimaryAxis.Horizontal));

            return ret.ToArray();
        }

        public event Action RequestToClose = () => { };

        private void OnMenuClicked(object sender, RoutedEventArgs e)
        {
            RequestToClose();
        }
    }
}
