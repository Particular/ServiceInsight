namespace ServiceInsight
{
    using System;

    public interface IAppCommands
    {
        void ShutdownImmediately();
    }

    public class AppCommands : IAppCommands
    {
        public void ShutdownImmediately()
        {
            Environment.Exit(0);
        }
    }
}