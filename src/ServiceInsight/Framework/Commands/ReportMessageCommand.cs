using System;
using Mindscape.Raygun4Net;
using ServiceInsight.Framework.UI.ScreenManager;
using ServiceInsight.Models;

namespace ServiceInsight.Framework.Commands
{
    public class ReportMessageCommand : BaseCommand
    {
        readonly RaygunClient client;
        readonly IWindowManagerEx windowManager;

        public ReportMessageCommand(IWindowManagerEx windowManager)
        {
            this.windowManager = windowManager;
            client = RaygunUtility.GetClient();
        }

        public override void Execute(object parameter)
        {
            var package = parameter as ReportMessagePackage;
            if (package == null)
                return;

            RaygunUtility.SendError(client, package.Exception, package.Message);

            windowManager.ShowMessageBox("Your message has been sent to Particular Software.");
        }

        public class ReportMessagePackage
        {
            public ReportMessagePackage(Exception ex, StoredMessage message)
            {
                Exception = ex;
                Message = message;
            }

            public StoredMessage Message { get; }

            public Exception Exception { get; }
        }
    }
}