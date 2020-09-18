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
                // Check running process
                if (p.Id != currentProcess.Id) 
                {
                    if (p.ProcessName.Equals(currentProcess.ProcessName))
                    {
                        IntPtr hFound = p.MainWindowHandle;
                        // Activate the window, if process is already running
                        User32API.SetForegroundWindow(hFound); 
                        break;
                    }
                }
            }
        }
    }
}