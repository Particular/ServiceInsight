namespace Particular.ServiceInsight.Desktop.LogWindow
{
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.Screens;
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

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
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