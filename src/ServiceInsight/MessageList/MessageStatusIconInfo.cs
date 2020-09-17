namespace ServiceInsight.MessageList
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using ExtensionMethods;
    using Models;

    public class MessageStatusIconInfo : IComparable
    {
        static Dictionary<MessageStatus, string> statusToIconNameMap;
        Func<string, ImageSource> resourceFinder;
        MessageStatus status;
        bool statusSpecified;

        static MessageStatusIconInfo()
        {
            statusToIconNameMap = new Dictionary<MessageStatus, string>
            {
                [MessageStatus.Successful] = "Successful",
                [MessageStatus.Failed] = "Failed",
                [MessageStatus.ArchivedFailure] = "Archived",
                [MessageStatus.RepeatedFailure] = "RepeatedFailed",
                [MessageStatus.ResolvedSuccessfully] = "Successful",
                [MessageStatus.RetryIssued] = "RetryIssued",
            };
        }
        
        private ImageSource DefaultApplicationResourceFinder(string imageName)
        {
            return Application.Current.TryFindResource(imageName) as DrawingImage;
        }

        public MessageStatusIconInfo(StoredMessage message, Func<string, ImageSource> resourceFinder = null)
        {
            this.resourceFinder = resourceFinder ?? DefaultApplicationResourceFinder;
            
            HasWarn = Warn(message);
            Status = message.Status;
            Description = status.GetDescription();
            Image = GetImage();
        }

        private static bool Warn(StoredMessage message)
        {
            return message.ProcessingTime < TimeSpan.Zero ||
                   message.CriticalTime < TimeSpan.Zero ||
                   message.DeliveryTime < TimeSpan.Zero;
        }

        public ImageSource Image { get; }

        public MessageStatus Status
        {
            get => status;
            private set
            {
                status = value;
                statusSpecified = true;
            }
        }
        
        public bool HasWarn { get; }

        public string Description { get; }

        ImageSource GetImage()
        {
            var currentStatus = statusSpecified ? Status : MessageStatus.Successful;
            var imageName = $"MessageStatus_{statusToIconNameMap[currentStatus]}";
            
            if (HasWarn || Status == MessageStatus.ResolvedSuccessfully)
            {
                imageName += "_Warn";
            }

            var image = resourceFinder(imageName); 

            return image;
        }

        public int CompareTo(object obj)
        {
            var that = obj as MessageStatusIconInfo;
            if (that == null)
            {
                return -1;
            }

            if (statusSpecified == false &&
                that.statusSpecified == false)
            {
                return 0;
            }

            return that.Status.CompareTo(Status);
        }

        public override string ToString()
        {
            if (!statusSpecified)
            {
                return "Not Specified";
            }

            switch (Status)
            {
                case MessageStatus.Failed:
                    return "Failed";

                case MessageStatus.RepeatedFailure:
                    return "Faulted";

                case MessageStatus.Successful:
                case MessageStatus.ResolvedSuccessfully:
                    return "Success";

                case MessageStatus.RetryIssued:
                    return "Retried";

                case MessageStatus.ArchivedFailure:
                    return "Archived";

                default:
                    throw new NotSupportedException($"Enum 'Status' is {Status}");
            }
        }
    }
}