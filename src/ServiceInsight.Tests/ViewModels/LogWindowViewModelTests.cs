namespace Particular.ServiceInsight.Tests.ViewModels
{
    using System;
    using System.Text;
    using System.Windows.Media;
    using Desktop.Framework;
    using Desktop.LogWindow;
    using NSubstitute;
    using NUnit.Framework;
    using Serilog.Events;
    using Serilog.Parsing;

    [TestFixture]
    public class LogWindowViewModelTests
    {
        LogWindowViewModel ViewModel;
        IClipboard Clipboard;

        [SetUp]
        public void TestInitialize()
        {
            Clipboard = Substitute.For<IClipboard>();
            ViewModel = new LogWindowViewModel(Clipboard);
        }

        [Test, Ignore("Need to use the test scheduler to force the observable to update")]
        public void hooks_up_observer_to_list()
        {
            var currentLogCount = ViewModel.Logs.Count;
            LogWindowViewModel.LogObserver.OnNext(new LogEvent(DateTimeOffset.Now, LogEventLevel.Information, null, new MessageTemplate("LOGME", new MessageTemplateToken[0]), new LogEventProperty[0]));
            Assert.AreEqual(currentLogCount + 1, ViewModel.Logs.Count);
        }

        [Test]
        public void clear_command_removes_logs()
        {
            while (ViewModel.Logs.Count < 10)
            {
                ViewModel.Logs.Add(new LogMessage("", Colors.Red));
            }

            ViewModel.ClearCommand.Execute(null);

            Assert.AreEqual(0, ViewModel.Logs.Count);
        }

        [Test]
        public void copy_command_uses_clipboard()
        {
            ViewModel.Logs.Clear();
            var sb = new StringBuilder();
            for (var i = 0; i < 10; i++)
            {
                ViewModel.Logs.Add(new LogMessage(i + Environment.NewLine, Colors.Red));
                sb.AppendLine(i.ToString());
            }

            ViewModel.CopyCommand.Execute(null);

            Clipboard.Received(1).CopyTo(sb.ToString());
        }
    }
}