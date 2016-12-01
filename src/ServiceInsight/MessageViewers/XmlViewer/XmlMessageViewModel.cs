namespace ServiceInsight.MessageViewers.XmlViewer
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using Caliburn.Micro;
    using ServiceInsight.ExtensionMethods;
    using ServiceInsight.Models;

    public class XmlMessageViewModel : Screen, IDisplayMessageBody
    {
        IXmlMessageView messageView;

        protected override void OnActivate()
        {
            base.OnActivate();
            DisplayName = "Xml";
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            messageView = (IXmlMessageView)view;
            OnSelectedMessageChanged();
        }

        public MessageBody SelectedMessage { get; set; }

        public void OnSelectedMessageChanged()
        {
            if (messageView == null)
            {
                return;
            }

            messageView.Clear();

            if (SelectedMessage == null || SelectedMessage.Body == null)
            {
                return;
            }

            messageView.Display(GetFormatted(SelectedMessage.Body.Text));
        }

        public void Display(StoredMessage selectedMessage)
        {
            if (SelectedMessage == selectedMessage) //Workaround, to force refresh the property. Should refactor to use the same approach as hex viewer.
            {
                OnSelectedMessageChanged();
            }
            else
            {
                SelectedMessage = selectedMessage;
            }
        }

        public void Clear()
        {
            messageView?.Clear();
        }

        private string GetFormatted(string message)
        {
            try
            {
                using (var stringReader = new StringReader(message))
                using (var xmlReader = new XmlTextReader(stringReader))
                {
                    var xml = XDocument.Load(xmlReader);
                    return xml.ToString();
                }
            }
            catch (Exception)
            {
                return message;
                // It looks like we having issues parsing the xml
                // Best to do in this circunstances is to still display the text
            }
        }
    }
}