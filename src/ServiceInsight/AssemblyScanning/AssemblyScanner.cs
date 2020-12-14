using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ServiceInsight.AssemblyScanning
{
    class AssemblyScanner
    {
        readonly List<string> searchDirectories = new List<string>
        {
            AppContext.BaseDirectory
        };

        internal AssemblyScanner(params string[] additionalSearchDirectories)
        {
            if (additionalSearchDirectories != null && additionalSearchDirectories.Any())
            {
                searchDirectories.AddRange(additionalSearchDirectories.Where(Directory.Exists));
            }
        }

        public IEnumerable<Assembly> Scan()
        {
            var assemblies = new List<Assembly>();

            foreach (var searchDirectory in searchDirectories)
            {
                var assembliesFullPaths = Directory
                    .GetFiles(searchDirectory, "*.dll", SearchOption.TopDirectoryOnly);

                foreach (var assemblyFullPath in assembliesFullPaths)
                {
                    var assemblyFileName = Path.GetFileNameWithoutExtension(assemblyFullPath);
                    if (
                        assemblyFileName.StartsWith("Autofac", StringComparison.InvariantCultureIgnoreCase)
                        || assemblyFileName.StartsWith("Caliburn", StringComparison.InvariantCultureIgnoreCase)
                        || assemblyFileName.StartsWith("DevExpress", StringComparison.InvariantCultureIgnoreCase)
                        || assemblyFileName.StartsWith("Serilog", StringComparison.InvariantCultureIgnoreCase)
                        || assemblyFileName.StartsWith("Mindscape", StringComparison.InvariantCultureIgnoreCase)
                        )
                    {
                        continue;
                    }

                    AssemblyValidator.ValidateAssemblyFile(assemblyFullPath, out var shouldLoad, out var reason);
                    if (shouldLoad)
                    {
                        assemblies.Add(Assembly.LoadFrom(assemblyFullPath));
                    }
                }
            }

            return assemblies.Distinct();
        }
    }
}
