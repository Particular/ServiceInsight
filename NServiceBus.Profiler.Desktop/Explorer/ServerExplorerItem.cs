using System.Drawing;
using NServiceBus.Profiler.Desktop.Properties;

namespace NServiceBus.Profiler.Desktop.Explorer
{
    public class ServerExplorerItem : ExplorerItem
    {
        public const int ServerNodeIdentifier = int.MinValue;

        public ServerExplorerItem(string name) : base(name)
        {
        }

        public override Bitmap Image
        {
            get { return Resources.Computer; }
        }

        public override int? ParentId
        {
            get { return null; }
        }
    }
}