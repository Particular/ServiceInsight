namespace Particular.ServiceInsight.Desktop.MessageList
{
    using System;
    using System.Linq;
    using Models;

    public class GridFocusedRowPreserver<T> : IDisposable where T : MessageInfo
    {
        T currentItem;
        ITableViewModel<T> viewModel;

        public GridFocusedRowPreserver(ITableViewModel<T> viewModel)
        {
            this.viewModel = viewModel;
            currentItem = this.viewModel.FocusedRow;
        }

        public void Dispose()
        {
            if (currentItem == null) return;
            viewModel.FocusedRow = viewModel.Rows.FirstOrDefault(item => item.Id == currentItem.Id);
        }
    }
}
