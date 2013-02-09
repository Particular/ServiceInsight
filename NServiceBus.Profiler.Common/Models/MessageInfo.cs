using System;
using Caliburn.PresentationFramework;

namespace NServiceBus.Profiler.Common.Models
{
    [Serializable]
    public class MessageInfo : PropertyChangedBase
    {
        private string _id;
        private DateTime _timeSent;
        private bool _isDeleted;
        private string _label;

        public MessageInfo()
        {
        }

        public MessageInfo(string id, string label, DateTime timeSent)
        {
            _id = id;
            _label = label;
            _timeSent = timeSent;
            _isDeleted = false;
        }

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyOfPropertyChange("Id");
            }
        }

        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                NotifyOfPropertyChange("Label");
            }
        }

        public DateTime TimeSent
        {
            get { return _timeSent; }
            set
            {
                _timeSent = value;
                NotifyOfPropertyChange("TimeSent");
            }
        }

        public bool IsDeleted
        {
            get { return _isDeleted; }
            set
            {
                _isDeleted = value;
                NotifyOfPropertyChange("IsDeleted");
            }
        }
    }
}