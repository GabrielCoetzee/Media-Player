using System.Collections.Generic;
using MediaPlayer.ApplicationSettings.Interfaces;
using MediaPlayer.Common.Metadata_Readers.Abstract;
using MediaPlayer.Common.Metadata_Readers.Derived;
using Ninject.Modules;

namespace MediaPlayer.IoC
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
