﻿using Particular.ServiceInsight.Desktop.Shell;
using log4net;
using log4net.Repository.Hierarchy;

namespace Particular.ServiceInsight.Desktop.LogWindow
{
    public interface ILogWindowView
    {
        void Initialize();
        void SetupContextMenu();
        void Clear();
        void Copy();
    }

    public partial class LogWindowView : ILogWindowView
    {
        private readonly IMenuManager _menuManager;
        private RichTextBoxAppender appender;

        public LogWindowView(IMenuManager menuManager)
        {
            _menuManager = menuManager;
        }

        public LogWindowView()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            appender = new RichTextBoxAppender(LogContainer);
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.AddAppender(appender);
        }

        public void SetupContextMenu()
        {
            _menuManager.CreateContextMenu(this, Model.ContextMenuItems);
        }

        public void Clear()
        {
            appender.Clear();
        }

        public void Copy()
        {
            LogContainer.Copy();
        }

        public void Close()
        {
            
        }

        private ILogWindowViewModel Model
        {
            get { return (ILogWindowViewModel)DataContext; }
        }

    }
}
