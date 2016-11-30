namespace ServiceInsight.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO.Packaging;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using ApprovalTests;
    using ApprovalTests.Core;
    using ApprovalTests.Namers.StackTraceParsers;
    using ApprovalTests.Reporters;
    using ApprovalTests.Wpf;
    using ApprovalUtilities.Wpf;
    using NUnit.Framework;

    //[UseReporter(typeof(AutoApprover))]
    [UseReporter(typeof(DiffReporter))]
    [Apartment(ApartmentState.STA)]
    [TestFixture]
    public class XamlIconsTests
    {
        [Test]
        [TestCaseSource(typeof(XamlIconsTests), nameof(GetControlTemplates))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveControlTemplate(string resourceName)
        {
            var icon = new ContentControl { Foreground = Brushes.Green };
            var control = CreateDisplayFrame(resourceName, icon);
            icon.Template = (ControlTemplate)control.FindResource(resourceName);

            PrepareForRender(control);

            Approvals.Verify(
                new ImageWriter(f => WpfUtils.ScreenCapture(control, f)),
                new IconTestsNamer(resourceName),
                Approvals.GetReporter());
        }

        [Test]
        [TestCaseSource(typeof(XamlIconsTests), nameof(GetDrawingImages))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveDrawingImage(string resourceName)
        {
            var icon = new Image();
            var control = CreateDisplayFrame(resourceName, icon);
            icon.Source = (DrawingImage)control.FindResource(resourceName);

            PrepareForRender(control);

            Approvals.Verify(
                new ImageWriter(f => WpfUtils.ScreenCapture(control, f)),
                new IconTestsNamer(resourceName),
                Approvals.GetReporter());
        }

        [Test]
        [TestCaseSource(typeof(XamlIconsTests), nameof(GetGeometries))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveGeometry(string resourceName)
        {
            var icon = new Path { Fill = Brushes.Black, Stretch = Stretch.Uniform };
            var control = CreateDisplayFrame(resourceName, icon);
            icon.Data = (Geometry)control.FindResource(resourceName);

            PrepareForRender(control);

            Approvals.Verify(
                new ImageWriter(f => WpfUtils.ScreenCapture(control, f)),
                new IconTestsNamer(resourceName),
                Approvals.GetReporter());
        }

        [Test]
        public void OnlyExpectedResourcesInDictionary()
        {
            var xamlIcons = GetXamlIconsResourceDictionary();

            var items = xamlIcons.Values.Cast<object>()
                .Where(i => !(i is ControlTemplate || i is DrawingImage || i is Geometry));

            Assert.IsEmpty(items);
        }

        public static IEnumerable GetControlTemplates => GetTestCases<ControlTemplate>();

        public static IEnumerable GetDrawingImages => GetTestCases<DrawingImage>();

        public static IEnumerable GetGeometries => GetTestCases<Geometry>();

        private static void InitWPFResourcesSystem()
        {
            // Hinky stuff to get the resource dictionary working
            PackUriHelper.Create(new Uri("reliable://0"));
            new FrameworkElement();
            Application.ResourceAssembly = typeof(App).Assembly;
        }

        private static ResourceDictionary GetXamlIconsResourceDictionary()
            => new ResourceDictionary { Source = new Uri("/ServiceInsight;component/Images/Xaml/XamlIcons.xaml", UriKind.Relative) };

        private static ContentControl CreateDisplayFrame(string resourceName, UIElement displayElement)
        {
            var control = new ContentControl();
            var dock = new DockPanel();
            var text = new TextBlock();

            control.Width = 500;
            control.Height = 500;
            control.Content = dock;
            DockPanel.SetDock(text, Dock.Top);
            dock.Children.Add(text);
            dock.Children.Add(displayElement);
            text.FontSize = 24;
            text.FontWeight = FontWeights.Bold;
            text.HorizontalAlignment = HorizontalAlignment.Center;

            text.Text = resourceName;

            var xamlIcons = GetXamlIconsResourceDictionary();
            control.Resources.MergedDictionaries.Add(xamlIcons);

            return control;
        }

        private static IEnumerable GetTestCases<TValue>()
        {
            IEnumerable<TestCaseData> results = null;
            var wait = new SemaphoreSlim(0);
            var ts = new ThreadStart(() =>
            {
                InitWPFResourcesSystem();

                var xamlIcons = GetXamlIconsResourceDictionary();

                results = xamlIcons.Cast<DictionaryEntry>()
                    .Where(e => e.Value is TValue)
                    .Select(e => new TestCaseData((string)e.Key).SetName((string)e.Key))
                    .ToArray();

                wait.Release();
            });
            var t = new Thread(ts);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            wait.Wait();
            return results;
        }

        private static void PrepareForRender(UIElement control)
        {
            // From http://stackoverflow.com/a/2596035
            control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            control.Arrange(new Rect(control.DesiredSize));

            control.Dispatcher.Invoke(() => { }, DispatcherPriority.Loaded);
        }

        class IconTestsNamer : IApprovalNamer
        {
            private readonly StackTraceParser stackTraceParser;
            private readonly string resourceName;

            public IconTestsNamer(string resourceName)
            {
                this.resourceName = resourceName;
                Approvals.SetCaller();
                stackTraceParser = new StackTraceParser();
                stackTraceParser.Parse(Approvals.CurrentCaller.StackTrace);
                System.IO.Directory.CreateDirectory(SourcePath);
            }

            public string Name => "Icon";

            public string SourcePath => System.IO.Path.Combine(stackTraceParser.SourcePath, "XamlIconResults", resourceName);
        }
    }
}