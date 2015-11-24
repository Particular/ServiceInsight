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
            int zoom = 1;
            UIElement body = (UIElement) ScrollViewer_Body.Content;
            UIElement header = (UIElement) ScrollViewer_Header.Content;

            double actualHeight = body.RenderSize.Height + header.RenderSize.Height;
            double actualWidth = body.RenderSize.Width;

            double renderHeight = actualHeight*zoom;
            double renderWidth = actualWidth*zoom;

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int) renderWidth, (int) renderHeight, 96, 96, PixelFormats.Pbgra32);
            VisualBrush headerSourceBrush = new VisualBrush(header);
            VisualBrush bodySourceBrush = new VisualBrush(body);

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.PushTransform(new ScaleTransform(zoom, zoom));
                drawingContext.DrawRectangle(headerSourceBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, header.RenderSize.Height)));
                drawingContext.DrawRectangle(bodySourceBrush, null, new Rect(new Point(0, header.RenderSize.Height), new Point(actualWidth, body.RenderSize.Height)));
            }
            renderTarget.Render(drawingVisual);

            var saveFileDialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".png",
                FileName = "sequencediagram.png"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveRTBAsPNG(renderTarget, saveFileDialog.FileName);
            }
        }

        public static void SnapShotPNG(UIElement source, int zoom)
        {
            double actualHeight = source.RenderSize.Height;
            double actualWidth = source.RenderSize.Width;

            double renderHeight = actualHeight*zoom;
            double renderWidth = actualWidth*zoom;

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int) renderWidth, (int) renderHeight, 96, 96, PixelFormats.Pbgra32);
            VisualBrush sourceBrush = new VisualBrush(source);

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.PushTransform(new ScaleTransform(zoom, zoom));
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
            }
            renderTarget.Render(drawingVisual);
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