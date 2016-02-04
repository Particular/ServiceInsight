namespace ServiceInsight.MessageList
{
    using System;
    using System.Windows.Media.Imaging;
    using ExtensionMethods;
    using global::ServiceInsight.Properties;
    using Models;

    public class MessageErrorInfo : IComparable
    {
        MessageStatus status;
        bool statusSpecified;

        public MessageErrorInfo()
        {
            Image = GetImage();
        }

        public MessageErrorInfo(MessageStatus status)
        {
            Status = status;
            Image = GetImage();
            Description = status.GetDescription();
        }

        public BitmapImage Image { get; private set; }

        public MessageStatus Status
        {
            get { return status; }
            private set
            {
                status = value;
                statusSpecified = true;
            }
        }

        public string Description { get; private set; }

        BitmapImage GetImage()
        {
            if (!statusSpecified)
            {
                return Resources.BulletWhite.ToBitmapImage();
            }

            switch (Status)
            {
                case MessageStatus.Failed:
                    return Resources.BulletYellow.ToBitmapImage();

                case MessageStatus.ArchivedFailure:
                    return Resources.BulletArchived.ToBitmapImage();

                case MessageStatus.RepeatedFailure:
                    return Resources.BulletRed.ToBitmapImage();

                case MessageStatus.Successful:
                    return Resources.BulletGreen.ToBitmapImage();

                case MessageStatus.ResolvedSuccessfully:
                    return Resources.BulletGreen.ToBitmapImage();

                case MessageStatus.RetryIssued:
                    return Resources.BulletWhite.ToBitmapImage();

                default:
                    throw new ArgumentOutOfRangeException(string.Format("Status {0} is not implemented", Status));
            }
        }

        public int CompareTo(object obj)
        {
            var that = obj as MessageErrorInfo;
            if (that == null) return -1;

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