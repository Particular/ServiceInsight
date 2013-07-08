using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.MessageList;

namespace NServiceBus.Profiler.Desktop.LogWindow
{
    public class LogWindowViewModel : Screen, ILogWindowViewModel
    {
        private ILogWindowView _view;

        public LogWindowViewModel()
        {
            ContextMenuItems = new BindableCollection<ContextMenuModel>();
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _view = (ILogWindowView)view;
            _view.Initialize();

            ContextMenuItems.Add(new ContextMenuModel(this, "Clear", "Clear All", Properties.Resources.Clear));
            ContextMenuItems.Add(new ContextMenuModel(this, "CopyToClipboard", "Copy", Properties.Resources.Copy));

            _view.SetupContextMenu();
        }

        public void Clear()
        {
            _view.Clear();
        }

        public IObservableCollection<ContextMenuModel> ContextMenuItems { get; private set; }

        public void CopyToClipboard()
        {
            _view.Copy();
        }
    }

    public interface ILogWindowViewModel : IScreen
    {
        void Clear();
        IObservableCollection<ContextMenuModel> ContextMenuItems { get; }
    }
}