namespace Particular.ServiceInsight.Desktop.Startup
{
    using System;

    public class EnvironmentWrapper
    {
        public virtual string[] GetCommandLineArgs()
        {
            return Environment.GetCommandLineArgs();
        }
    }
}