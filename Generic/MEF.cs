using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Generic
{
    public class MEF
    {

        public static CompositionContainer Container;

        public static void Compose(Assembly assembly, string applicationName)
        {
            var aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(assembly));

            var directoryPath = Path.GetDirectoryName(assembly.Location);

            if (directoryPath != null)
            {
                aggregateCatalog.Catalogs.Add(new DirectoryCatalog(directoryPath, $"{applicationName}.*.dll"));

                foreach (var subdirectory in Directory.GetDirectories(directoryPath))
                    aggregateCatalog.Catalogs.Add(new DirectoryCatalog(subdirectory, $"{applicationName}.*.dll"));
            }

            Container = new CompositionContainer(aggregateCatalog);
        }

        public static void Build(Application app)
        {
            Container.ComposeParts(app);
        }
    }
}
