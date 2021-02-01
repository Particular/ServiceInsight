namespace ServiceInsight.Framework.Events
{
    using ServiceInsight.Models;

    public class ConfigurationUpdated
    {
        public ConfigurationUpdated(CommandLineOptions options)
        {
            Options = options;
        }

        public CommandLineOptions Options { get; }
    }
}