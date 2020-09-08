namespace ServiceInsight
{
    using System;
    using System.Diagnostics;

    public static class FocusWindow
    {
        public static void ShowWindow()
        {
            // Getting collection of process
            Process currentProcess = Process.GetCurrentProcess();
            
            // Check with other process already running
            foreach (var p in Process.GetProcesses())
            {
                if (p.Id != currentProcess.Id) // Check running process
                {
                    if (p.ProcessName.Equals(currentProcess.ProcessName))
                    {
                        IntPtr hFound = p.MainWindowHandle;
                        User32API.SetForegroundWindow(hFound); // Activate the window, if process is already running
                        break;
                    }
                }
            }
        }
    }
}