using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.MessageList
{
    public class GridSelectionPreserver<T> : IDisposable where T : MessageInfo
    {
        private readonly IList<T> _selectedrows;
        private readonly ITableViewModel<T> _viewModel;

        public GridSelectionPreserver(ITableViewModel<T> viewModel)
        {
            _viewModel = viewModel;
            _selectedrows = new List<T>(viewModel.SelectedRows);
        }

        public void Dispose()
        {
            foreach (var row in _selectedrows)
            {
                var matchingRow = _viewModel.Rows.FirstOrDefault(x => x.Id == row.Id);
                if (matchingRow != null)
                {
                    _viewModel.SelectedRows.Add(matchingRow);
                }
            }
        }
    }
}