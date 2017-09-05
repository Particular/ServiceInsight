namespace ServiceInsight.MessageList
{
    using System.Drawing;
    using System.Windows.Media.Imaging;
    using ExtensionMethods;
    using global::ServiceInsight.Properties;
    using Models;

    public class MessageErrorInfo
    {
        public MessageErrorInfo()
        {
        }

        public MessageErrorInfo(StoredMessage msg)
        {
            Description = GetDescription(msg);
            Image = GetImage(msg);
        }

        private string GetDescription(StoredMessage msg)
        {
            var description = msg.Status.GetDescription();

            if (msg.HasClockDrifts)
            {
                description += ". Endpoint shows symptoms of clock drifts.";
            }

            return description;
        }

        public BitmapImage Image { get; }

        public string Description { get; }

        BitmapImage GetImage(StoredMessage msg)
        {
            var imageName = GetImageName(msg);

            if (msg.HasClockDrifts && msg.Status != MessageStatus.ResolvedSuccessfully) /* Don't have Warning on Warning */
            {
                imageName += "_Warning";
            }

            return GetImage(imageName).ToBitmapImage();
        }

        private string GetImageName(StoredMessage msg)
        {
            switch (msg.Status)
            {
                case MessageStatus.Failed:
                    return "Message_Failed";

                case MessageStatus.ArchivedFailure:
                    return "Message_Archived";

                case MessageStatus.RepeatedFailure:
                    return "Message_Failed_Repeatedly";

                case MessageStatus.ResolvedSuccessfully:
                    return "Message_Warning";

                case MessageStatus.RetryIssued:
                    return "Message_Retry_Requested";

                case MessageStatus.Successful:
                default:
                    return "Message_Successful";
            }
        }

        private Bitmap GetImage(string name)
        {
            return Resources.ResourceManager.GetObject(name) as Bitmap;
        }
    }
}