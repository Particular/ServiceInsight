using System;
using System.Windows.Automation;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Models;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TreeItems;
using TestStack.White.Utility;

namespace NServiceBus.Profiler.FunctionalTests.Screens
{
    public class ShellScreen : ProfilerElement
    {
        public ShellScreen(IMainWindow mainWindow) : base(mainWindow)
        {
        }

        public Tree QueueExplorerTree { get; private set; }
        public MainMenu MainMenu { get; set; }

        public void WaitWhileBusy()
        {
            Retry.For(ShellIsBusy, isBusy => isBusy, TimeSpan.FromSeconds(10));
        }

        private bool ShellIsBusy()
        {
            var currentPropertyValue = MainWindow.AutomationElement.GetCurrentPropertyValue(AutomationElement.HelpTextProperty);
            return currentPropertyValue != null && ((string)currentPropertyValue).Contains("Busy");
        }

    }
}