namespace Particular.ServiceInsight.Desktop.LogWindow
{
    using Caliburn.Micro;
    using Core.UI;
    using Shell.Menu;

    public class LogWindowViewModel : Screen, IHaveContextMenu
    {
        ILogWindowView view;

        public LogWindowViewModel()
        {
            ContextMenuItems = new BindableCollection<IMenuItem>
            {
                new MenuItem("Clear All", new RelayCommand(Clear), Properties.Resources.Clear),
                new MenuItem("Copy", new RelayCommand(CopyToClipboard), Properties.Resources.Copy)
            };
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            this.view = (ILogWindowView)view;
            this.view.Initialize();
        }

        public void Clear()
        {
            view.Clear();
        }

        public IObservableCollection<IMenuItem> ContextMenuItems { get; private set; }

        public void OnContextMenuOpening()
        {
        }

        public void CopyToClipboard()
        {
            view.Copy();
        }
    }
}