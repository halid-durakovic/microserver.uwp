using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.ApplicationModel;

namespace MicroServer.Reflection
{
    public sealed class AppDomain
    {
        public static AppDomain CurrentDomain { get; private set; }

        static AppDomain()
        {
            CurrentDomain = new AppDomain();
        }

        public Assembly[] GetAssemblies()
        {
            return GetAssemblyListAsync().Result.ToArray();
        }

        private async Task<List<Assembly>> GetAssemblyListAsync()
        {
            List<string> excludedAssemblies = new List<string>();
            excludedAssemblies.Add("ClrCompression");
            excludedAssemblies.Add("UWPShim");

            var folder = Package.Current.InstalledLocation;

            List<Assembly> assemblies = new List<Assembly>();
            foreach (StorageFile file in await folder.GetFilesAsync())
            {
                if (file.FileType == ".dll" || file.FileType == ".exe")
                {
                    if (!excludedAssemblies.Contains(file.DisplayName))
                    {
                        AssemblyName name = new AssemblyName() { Name = file.DisplayName };
                        Assembly asm = Assembly.Load(name);
                        assemblies.Add(asm);
                    }
                }
            }
            return assemblies;
        }
    }
}
