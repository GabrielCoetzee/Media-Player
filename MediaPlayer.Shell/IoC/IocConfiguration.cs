using System.Collections.Generic;
using MediaPlayer.ApplicationSettings.Interfaces;
using MediaPlayer.Common.Metadata_Readers.Derived;
using MediaPlayer.MetadataReaders.Abstract;
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
