using System.Collections.Generic;
using MediaPlayer.MetadataReaders;
using MediaPlayer.Settings;
using Ninject.Modules;

namespace MediaPlayer.IoC
{
    public class IocConfiguration : NinjectModule
    {
        public override void Load()
        {
            Bind<IExposeApplicationSettings>().To<ApplicationSettings>().InSingletonScope();
            Bind<MetadataReaderProvider>().To<TaglibMetadataReaderProvider>().Named("TaglibMetadataReaderProvider");
            Bind<IEnumerable<MetadataReaderProvider>>().To<MetadataReaderProvider[]>();
        }
    }
}
