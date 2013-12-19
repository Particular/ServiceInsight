using NServiceBus.Profiler.Desktop.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceBus.Profiler.Desktop.MessageList
{
    public class MessageSelectionPreserver : IDisposable
    {
        private readonly StoredMessage currentItem;
        private readonly IViewWithGrid _view;

        public MessageSelectionPreserver(IViewWithGrid view)
        {
            currentItem = view.SelectedItem as StoredMessage;
            _view = view;
        }

        public void Dispose()
        {
            if (currentItem != null)
            {
                try
                {
                    _view.BeginSelection();

                    var itemsSource = _view.ItemsSource as IEnumerable<MessageInfo>;

                    for (int i = 0; i < itemsSource.Count(); i++)
                    {
                        if (itemsSource.ElementAt(i).Id == currentItem.Id)
                        {
//                            _view.SelectRow(i);
                            _view.SelectedItem = itemsSource.ElementAt(i);
                        }
                    }
                }
                finally
                {
                    _view.EndSelection();
                }
            }
        }

    }
}
