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
                    if (ReservedAssemblyNames.Any(a => assemblyFileName.StartsWith(a, StringComparison.InvariantCultureIgnoreCase)))
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

        private IEnumerable<string> ReservedAssemblyNames
        {
            get
            {
                yield return "Autofac";
                yield return "Caliburn";
                yield return "DevExpress";
                yield return "Serilog";
                yield return "Mindscape";
                yield return "System";
                yield return "Microsoft";
                yield return "RestSharp";
                yield return "GongSolutions";
                yield return "ICSharpCode";
                yield return "Nito";
            }
        }
    }
}
