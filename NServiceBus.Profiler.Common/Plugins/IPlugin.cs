using System.Collections.Generic;
using Caliburn.PresentationFramework.Screens;

namespace NServiceBus.Profiler.Common.Plugins
{
    public interface IPlugin : IScreen
    {
        IList<PluginContextMenu> ContextMenuItems { get; }
        int TabOrder { get; }
    }
}