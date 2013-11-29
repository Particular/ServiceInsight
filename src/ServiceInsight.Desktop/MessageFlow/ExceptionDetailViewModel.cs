using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    class ExceptionDetailViewModel : Screen, IExceptionDetailViewModel
    {
        public IExceptionDetails Exception { get; set; }

        public ExceptionDetailViewModel() { }

        public ExceptionDetailViewModel(IExceptionDetails exception)
        {
            this.Exception = exception;
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
