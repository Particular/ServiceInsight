namespace Particular.ServiceInsight.Desktop.Startup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Autofac;
    using log4net;
    using Module = Autofac.Module;

    public static class BuilderExtensions
    {
        static readonly ILog Logger = LogManager.GetLogger("Container");

        internal static Func<string, ILog, Assembly> LoadModuleAssembly = (file, logger) =>
        {
            const string errorFormat = "Plugin assembly {0} was not loaded. The reason is: {1}";

            try
            {
                return Assembly.LoadFile(file);
            }
            catch (ReflectionTypeLoadException ex)
            {
                logger.WarnFormat(errorFormat, file, ex.LoaderExceptions[0].GetBaseException().Message);
            }
            catch (Exception ex)
            {
                logger.WarnFormat(errorFormat, file, ex.GetBaseException().Message);
            }

            return null;
        };

        public static void RegisterPluginModules(this ContainerBuilder builder, string folder, string filePattern)
        {
            Guard.NotNullOrEmpty(() => folder, folder);
            Guard.NotNullOrEmpty(() => filePattern, filePattern);

            if (!Directory.Exists(folder))
            {
                Logger.InfoFormat("Plugin folder was not found at {0}", folder);
                return;
            }

            var plugins = Directory.GetFiles(folder, filePattern);
            if (plugins.Length <= 0)
            {
                Logger.InfoFormat("No plugin was found at {0}", folder);
                return;
            }

            foreach (var fullPath in plugins)
            {
                var assembly = LoadModuleAssembly(fullPath, Logger);
                if (assembly != null)
                {
                    RegisterModules(builder, assembly);
                }
            }
        }

        public static void RegisterModules(this ContainerBuilder builder, Assembly assembly)
        {
            Guard.NotNull(() => builder, builder);
            Guard.NotNull(() => assembly, assembly);

            foreach (var type in assembly.GetExportedTypes())
            {
                if(type.IsAssignableTo<Module>())
                {
                    var module = (Module)Activator.CreateInstance(type);
                    builder.RegisterModule(module);
                }
                else if(type.IsViewOrViewModel())
                {
                    builder.RegisterType(type).AsImplementedInterfaces().SingleInstance();
                }
            }
        }

        public static bool IsViewOrViewModel(this Type type)
        {
            return type != null &&
                   type.IsClass &&
                   !type.IsAbstract &&
                   type.Namespace != null &&
                   MatchingNames.Any(ns => type.Name.EndsWith(ns, StringComparison.InvariantCultureIgnoreCase));
        }

        static IEnumerable<string> MatchingNames
        {
            get
            {
                yield return "ViewModel";
                yield return "View";
            }
        }
    }
}