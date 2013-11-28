using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Core.UI;
using NServiceBus.Profiler.Desktop.Shell.Menu;

namespace NServiceBus.Profiler.Desktop.LogWindow
{
    public class LogWindowViewModel : Screen, ILogWindowViewModel
    {
        private ILogWindowView _view;

        public LogWindowViewModel()
        {
            ContextMenuItems = new BindableCollection<IMenuItem>
            {
                new MenuItem("Clear All", new RelayCommand(Clear), Properties.Resources.Clear),
                new MenuItem("Copy", new RelayCommand(CopyToClipboard), Properties.Resources.Copy)
            };
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _view = (ILogWindowView)view;
            _view.Initialize();
        }

        public void Clear()
        {
            _view.Clear();
        }

        public IObservableCollection<IMenuItem> ContextMenuItems { get; private set; }

        public void OnContextMenuOpening()
        {
        }

        public void CopyToClipboard()
        {
            _view.Copy();
        }
    }

    public interface ILogWindowViewModel : IScreen, IHaveContextMenu
    {
        void Clear();
        void CopyToClipboard();
    }
}