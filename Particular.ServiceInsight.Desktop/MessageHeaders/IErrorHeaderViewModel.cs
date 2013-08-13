﻿using Caliburn.PresentationFramework.Screens;

namespace Particular.ServiceInsight.Desktop.MessageHeaders
{
    public interface IErrorHeaderViewModel : IScreen
    {
        string ExceptionInfo { get; }
        string FailedQueue { get; }
        string TimeOfFailure { get; }

        void ReturnToSource();
        bool CanReturnToSource();
    }
}