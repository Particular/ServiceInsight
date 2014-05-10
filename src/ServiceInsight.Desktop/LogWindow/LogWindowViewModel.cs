namespace Particular.ServiceInsight.Desktop.LogWindow
{
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.Screens;
    using Core.UI;
    using Shell.Menu;

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