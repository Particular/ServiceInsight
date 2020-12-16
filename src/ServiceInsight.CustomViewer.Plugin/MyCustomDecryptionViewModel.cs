using Caliburn.Micro;
using ServiceInsight.MessageViewers;
using ServiceInsight.Models;
using ServiceInsight.ServiceControl;

namespace ServiceInsight.CustomViewer.Plugin
{
    public class MyCustomDecryptionViewModel : Screen, ICustomMessageBodyViewer
    {
        private IDisplayMessageBody view;
        
        public MyCustomDecryptionViewModel()
        {
            DisplayName = "Decryption Viewer";
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            this.view = (IDisplayMessageBody)view;           
        }

        public void Display(StoredMessage selectedMessage)
        {
            if (selectedMessage != null)
            {
                this.view?.Display(selectedMessage);
            }
        }

        public void Clear()
        {
            this.view?.Clear();
        }
        
        public bool IsVisible(StoredMessage selectedMessage, PresentationHint presentationHint)
        {
            return selectedMessage != null;
        }
    }
}