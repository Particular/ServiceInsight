using System;
using System.Collections.Generic;
using Caliburn.PresentationFramework.Screens;

namespace NServiceBus.Profiler.Desktop.Conversations
{
    public class ConversationViewModel : Screen, IConversationViewModel
    {
        private IConversationView _view;

        public ConversationViewModel()
        {
            var rootMsg = new ConversationMessage() {Title = "Root"};
            var child1 = new ConversationMessage {Title = "Child One", Parent = rootMsg };
            var child2 = new ConversationMessage {Title = "Child Two", Parent = rootMsg };
            var grandchild = new ConversationMessage {Title = "Grand Child", Parent = child1 };

            rootMsg.Children.Add(child1);
            rootMsg.Children.Add(child2);
            child1.Children.Add(grandchild);
            Message = rootMsg;
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _view = view as IConversationView;
        }

        public ConversationMessage Message { get; private set; }

        public void SwitchToNext()
        {
        }

        public void SwitchToPrevious()
        {
        }
    }

    public class ConversationMessage
    {
        public ConversationMessage(ConversationMessage parent = null)
        {
            Id = Guid.NewGuid();
            Children = new List<ConversationMessage>();
            Parent = parent;
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public ConversationMessage Parent { get; set; }
    
        public IList<ConversationMessage> Children { get; private set; }
    }

    public interface IConversationViewModel : IScreen
    {
    }
}