using System.Collections.Generic;
using MediaPlayer.ApplicationSettings.Interfaces;
using MediaPlayer.MetadataReaders.Abstract;
using MediaPlayer.MetadataReaders.Derived;
using Ninject.Modules;

namespace MediaPlayer.Shell.IoC
{
    public class IocConfiguration : NinjectModule
    {
        public override void Load()
        {
            Bind<IExposeApplicationSettings>().To<ApplicationSettings.Interface_Implementations.ApplicationSettings>().InSingletonScope();
            Bind<MetadataReaderProvider>().To<TaglibMetadataReaderProvider>().Named("TaglibMetadataReaderProvider");
            Bind<IEnumerable<MetadataReaderProvider>>().To<MetadataReaderProvider[]>();
        }
    }
}
