using System.Drawing;

namespace NServiceBus.Profiler.Desktop.MessageList
{
    public class ContextMenuModel
    {
        public ContextMenuModel(string name, string displayName, Bitmap image = null)
        {
            Name = name;
            DisplayName = displayName;
        }

        public virtual string Name { get; private set; }
        public virtual string DisplayName { get; private set; }
        public virtual Bitmap Image { get; set; } 
    }
}