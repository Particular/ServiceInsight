namespace ServiceInsight.Framework.Commands
{
    using System;
    using Mindscape.Raygun4Net;
    using ServiceInsight.Framework.UI.ScreenManager;
    using ServiceInsight.Models;

    public class ReportMessageCommand : BaseCommand
    {
        readonly RaygunClient client;
        readonly IServiceInsightWindowManager windowManager;

        public ReportMessageCommand(IServiceInsightWindowManager windowManager)
        {
            this.windowManager = windowManager;
            client = RaygunUtility.GetClient();
        }

        public override void Execute(object parameter)
        {
            if (!(parameter is ReportMessagePackage package))
            {
                return;
            }

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