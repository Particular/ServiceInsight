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
                    if (p.ProcessName.Equals(currentProcess.ProcessName) == true)
                    {IntPtr hFound = p.MainWindowHandle;
                        //if (User32API.IsIconic(hFound)) // If application is in ICONIC mode then
                        //    User32API.ShowWindow(hFound, User32API.SW_RESTORE);
                        User32API.SetForegroundWindow(hFound); // Activate the window, if process is already running
                        break;
                    }
                }
            }
        }
    }
}