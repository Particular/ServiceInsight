namespace ServiceInsight.ExtensionMethods
{
    using System.Drawing;
    using System.IO;
    using System.Windows.Media.Imaging;

    public static class ImageExtensions
    {
        public static BitmapImage ToBitmapImage(this Image image)
        {
            if (image == null)
            {
                return null;
            }

            var ms = new MemoryStream();
            image.Save(ms, image.RawFormat);

            var bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            return bi;
        }
    }
}