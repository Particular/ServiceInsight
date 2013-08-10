using System;
using System.Collections.Generic;

namespace NServiceBus.Profiler.Desktop.MessageList
{
    public class GridSelectionPreserver : IDisposable
    {
        private readonly IList<int> _selectedrows;
        private readonly IViewWithGrid _view;

        public GridSelectionPreserver(IViewWithGrid view)
        {
            _view = view;
            _selectedrows = _view.Table.GetSelectedRowHandles();
        }

        public void Dispose()
        {
            try
            {
                _view.Table.BeginSelection();

                foreach (var row in _selectedrows)
                {
                    if(!_view.Table.IsRowSelected(row))
                        _view.Table.SelectRow(row);
                }
            }
            finally
            {
                _view.Table.EndSelection();
            }
        }
    }
}