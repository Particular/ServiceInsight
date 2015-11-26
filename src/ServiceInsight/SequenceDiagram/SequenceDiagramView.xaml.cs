namespace ServiceInsight.SequenceDiagram
{
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Win32;

    public partial class SequenceDiagramView
    {
        public SequenceDiagramView()
        {
            InitializeComponent();

            ScrollViewer_Body.ScrollChanged += ScrollViewer_Body_ScrollChanged;
            ScrollViewer_Body.PreviewMouseWheel += ScrollViewer_Body_MouseWheel;
        }

        void ScrollViewer_Body_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer_Body.ScrollToVerticalOffset(ScrollViewer_Body.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        void ScrollViewer_Body_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer_Header.Width = e.ViewportWidth;
            ScrollViewer_Header.ScrollToHorizontalOffset(e.HorizontalOffset);
        }

        void ExportToPng(object sender, RoutedEventArgs e)
        {
            var bodyElement = (UIElement) ScrollViewer_Body.Content;
            var headerElement = (UIElement) ScrollViewer_Header.Content;

            var actualHeight = bodyElement.RenderSize.Height + headerElement.RenderSize.Height;
            var actualWidth = bodyElement.RenderSize.Width;

            var renderTarget = new RenderTargetBitmap((int)actualWidth, (int)actualHeight, 96, 96, PixelFormats.Default);
            var bodySourceBrush = new VisualBrush(bodyElement);

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(new SolidColorBrush(Colors.White), null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
                drawingContext.DrawRectangle(bodySourceBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
            }

            renderTarget.Render(drawingVisual);

            var saveFileDialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".png",
                FileName = "sequencediagram.png",
                Filter = "Portable Network Graphics (.png)|*.png"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveRTBAsPNG(renderTarget, saveFileDialog.FileName);
            }
        }

        private static void SaveRTBAsPNG(RenderTargetBitmap bmp, string filename)
        {
            var enc = new PngBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bmp));

            using (var stm = File.Create(filename))
            {
                enc.Save(stm);
            }
        }
    }
}