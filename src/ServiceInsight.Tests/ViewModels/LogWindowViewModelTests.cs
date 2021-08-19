namespace ServiceInsight.Tests.ViewModels
{
    using System;
    using System.Text;
    using System.Windows.Media;
    using LogWindow;
    using NSubstitute;
    using NUnit.Framework;
    using Serilog.Events;
    using Serilog.Parsing;
    using ServiceInsight.Framework;

    [TestFixture]
    public class LogWindowViewModelTests
    {
        LogWindowViewModel viewModel;
        IClipboard clipboard;

        [SetUp]
        public void TestInitialize()
        {
            clipboard = Substitute.For<IClipboard>();
            viewModel = new LogWindowViewModel(clipboard);
        }

        [Test]
        [Ignore("Need to use the test scheduler to force the observable to update")]
        public void Hooks_up_observer_to_list()
        {
            var currentLogCount = viewModel.Logs.Count;
#pragma warning disable PS0023 // DateTime.UtcNow or DateTimeOffset.UtcNow should be used instead of DateTime.Now and DateTimeOffset.Now, unless the value is being used for displaying the current date-time in a user's local time zone
            LogWindowViewModel.LogObserver.OnNext(new LogEvent(DateTimeOffset.Now, LogEventLevel.Information, null, new MessageTemplate("LOGME", new MessageTemplateToken[0]), new LogEventProperty[0]));
#pragma warning restore PS0023 // DateTime.UtcNow or DateTimeOffset.UtcNow should be used instead of DateTime.Now and DateTimeOffset.Now, unless the value is being used for displaying the current date-time in a user's local time zone
            Assert.AreEqual(currentLogCount + 1, viewModel.Logs.Count);
        }

        [Test]
        public void Clear_command_removes_logs()
        {
            while (viewModel.Logs.Count < 10)
            {
                viewModel.Logs.Add(new LogMessage("", Colors.Red));
            }

            viewModel.ClearCommand.Execute(null);

            Assert.AreEqual(0, viewModel.Logs.Count);
        }

        [Test]
        public void Copy_command_uses_clipboard()
        {
            viewModel.Logs.Clear();
            var sb = new StringBuilder();
            for (var i = 0; i < 10; i++)
            {
                viewModel.Logs.Add(new LogMessage(i + Environment.NewLine, Colors.Red));
                sb.AppendLine(i.ToString());
            }

            viewModel.CopyCommand.Execute(null);

            clipboard.Received(1).CopyTo(sb.ToString());
        }
    }
}