namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public partial class SequenceDiagramView
    {
        public SequenceDiagramView()
        {
            InitializeComponent();

            ScrollViewer_Body.ScrollChanged += ScrollViewer_Body_ScrollChanged;
        }

        void ScrollViewer_Body_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer_Header.Width = e.ViewportWidth;
            ScrollViewer_Header.ScrollToHorizontalOffset(e.HorizontalOffset);
        }

        void ExportToPng(object sender, RoutedEventArgs e)
        {
            var scalingRatioX = ScrollViewer_Body_Content.ActualWidth/ScrollViewer_Body.ViewportWidth;
            var scalingRatioY = ScrollViewer_Body_Content.ActualHeight/ScrollViewer_Body.ViewportHeight;
            var dpi = 96;
            var rtb = new RenderTargetBitmap(
                (int)(ScrollViewer_Body_Content.ActualWidth * scalingRatioX),
                (int)(ScrollViewer_Header_Content.ActualHeight * scalingRatioY + ScrollViewer_Body_Content.ActualHeight * scalingRatioY),
                dpi * scalingRatioX, dpi * scalingRatioY,
                PixelFormats.Pbgra32 // pixelformat 
                );

            var offset = Math.Abs(ScrollViewer_Body_Content.ActualWidth - ScrollViewer_Header_Content.ActualHeight) / 2;
            VisualBrush sourceBrush = new VisualBrush(ScrollViewer_Header_Content);
            VisualBrush sourceBrush2 = new VisualBrush(ScrollViewer_Body_Content);
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Size(ScrollViewer_Body_Content.ActualWidth, ScrollViewer_Header_Content.ActualHeight)));
                drawingContext.DrawRectangle(sourceBrush2, null, new Rect(new Point(0, 69.88), new Size(ScrollViewer_Body_Content.ActualWidth, ScrollViewer_Body_Content.ActualHeight)));
            }
            rtb.Render(drawingVisual);

            SaveRTBAsPNG(rtb, "C:\\SequenceDiagram1.png");
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