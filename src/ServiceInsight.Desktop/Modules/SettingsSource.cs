namespace Particular.ServiceInsight.Desktop.Modules
{
    using System;
    using System.Collections.Generic;
    using Autofac;
    using Autofac.Builder;
    using Autofac.Core;
    using ExceptionHandler.Settings;

    public class SettingsSource : IRegistrationSource
    {
        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            var typedService = service as IServiceWithType;
            if (typedService != null && typedService.ServiceType.IsClass && typedService.ServiceType.Name.EndsWith("Settings"))
            {
                yield return RegistrationBuilder.ForDelegate((c, p) =>
                {
                    var reader = c.Resolve<ISettingsReader>();
                    return reader.LoadSettings(typedService.ServiceType);
                }).As(typedService.ServiceType)
                  .CreateRegistration();
            }
        }

        public bool IsAdapterForIndividualComponents
        {
            get { return false; }
        }
    }
}