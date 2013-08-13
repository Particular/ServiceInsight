using System.Drawing;
using Caliburn.PresentationFramework.Screens;

namespace Particular.ServiceInsight.Desktop.MessageList
{
    public class ContextMenuModel
    {
        public ContextMenuModel(IScreen owner, string name, string displayName, Bitmap image = null)
        {
            Owner = owner;
            Name = name;
            DisplayName = displayName;
            Image = image;
        }

        public virtual IScreen Owner { get; private set; }
        public virtual string Name { get; private set; }
        public virtual string DisplayName { get; private set; }
        public virtual Bitmap Image { get; set; } 
    }
}