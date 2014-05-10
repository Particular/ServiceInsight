namespace Particular.ServiceInsight.Desktop.MessageList
{
    using System;
    using System.Linq;
    using Models;

    public class GridFocusedRowPreserver<T> : IDisposable where T : MessageInfo
    {
        private readonly T _currentItem;
        private readonly ITableViewModel<T> _viewModel;

        public GridFocusedRowPreserver(ITableViewModel<T> viewModel)
        {
            _viewModel = viewModel;
            _currentItem = _viewModel.FocusedRow;
        }

        public void Dispose()
        {
            if (_currentItem == null) return;
            _viewModel.FocusedRow = _viewModel.Rows.FirstOrDefault(item => item.Id == _currentItem.Id);
        }
    }
}
