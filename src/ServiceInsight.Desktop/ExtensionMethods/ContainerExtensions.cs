namespace Particular.ServiceInsight.Desktop.ExtensionMethods
{
    using System.Configuration;
    using Autofac;
    using Autofac.Configuration;

    public static class ContainerExtensions
    {
        public static void RegisterExternalModules(this ContainerBuilder builder)
        {
            var configSection = ConfigurationManager.GetSection("autofac");
            if (configSection != null)
            {
                builder.RegisterModule<ConfigurationSettingsReader>();
            }
        }
    }
}