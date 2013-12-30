using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Core.Settings;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Desktop.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    class ExceptionDetailViewModel : Screen, IExceptionDetailViewModel
    {
        private ISettingsProvider _settingsProvider;
        public virtual IPersistableLayout View { get; private set; }

        public IExceptionDetails Exception { get; set; }

        public ExceptionDetailViewModel(ISettingsProvider settingsProvider) 
        {
            this._settingsProvider = settingsProvider;
        }

        public ExceptionDetailViewModel(IExceptionDetails exception)
        {
            this.Exception = exception;
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            View = (IPersistableLayout)view;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            RestoreLayout();
        }

        public virtual void Deactivate(bool close)
        {
            base.OnDeactivate(close);
            SaveLayout();
        }

        private void SaveLayout()
        {
            View.OnSaveLayout(_settingsProvider);
        }

        private void RestoreLayout()
        {
            View.OnRestoreLayout(_settingsProvider);
        }

        public string FormattedSource
        {
            get
            {
                return string.Format("{0} (@{1})", Exception.ExceptionType, Exception.Source);
            }
        }
    }

    interface IExceptionDetailViewModel : IScreen
    {
        IExceptionDetails Exception { get; set; }
        string FormattedSource { get; }
    }
}
