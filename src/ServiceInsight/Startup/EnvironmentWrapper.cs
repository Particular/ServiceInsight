namespace ServiceInsight.Startup
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