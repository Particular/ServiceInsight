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
            _selectedrows = _view.GetSelectedRowsIndex();
        }

        public void Dispose()
        {
            try
            {
                _view.BeginSelection();

                foreach (var row in _selectedrows)
                {
                    if (!_view.IsRowSelected(row))
                        _view.SelectRow(row);
                }
            }
            finally
            {
                _view.EndSelection();
            }
        }
    }
}