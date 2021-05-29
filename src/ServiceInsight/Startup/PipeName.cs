namespace ServiceInsight.Startup
{
    using System;

    /// <summary>
    /// NamedPipeServerStream pipe name.
    /// </summary>
    public static class PipeName
    {
        /// <summary>
        /// "ServiceInsight-username" where username is the Windows currently logged in user username.
        /// </summary>
        public static string Value { get; } = $"ServiceInsight-{Environment.UserName}";
    }
}