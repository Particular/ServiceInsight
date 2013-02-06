using System.Drawing;
using NServiceBus.Profiler.Desktop.Properties;

namespace NServiceBus.Profiler.Desktop.Explorer
{
    public class AuditQueueExplorerItem : ExplorerItem
    {
        public AuditQueueExplorerItem(string name) : base(name)
        {
        }

        public override Bitmap Image
        {
            get { return Resources.TreeAuditQueue; }
        }
    }
}