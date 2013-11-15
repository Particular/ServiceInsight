using NServiceBus.Profiler.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    class ExceptionDetailViewModel
    {
        public IExceptionDetails Exception { get; set; }

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
}
