using System.Drawing;
using System.Windows.Input;

namespace NServiceBus.Profiler.Common.Plugins
{
    public class PluginContextMenu
    {
        public PluginContextMenu(string name, ICommand command, int order = 100)
        {
            Name = name;
            Command = command;
            Order = order;
        }

        public virtual ICommand Command { get; private set; }
        public virtual string Name { get; private set; }
        public virtual int Order { get; private set; }
        public virtual string DisplayName { get; set; }
        public virtual Bitmap Image { get; set; }
    }
}