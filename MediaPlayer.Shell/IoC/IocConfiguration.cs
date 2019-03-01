using System.Collections.Generic;
using MediaPlayer.ApplicationSettings.Settings_Provider;
using MediaPlayer.MetadataReaders.Abstract;
using MediaPlayer.MetadataReaders.Derived;
using Ninject.Modules;

namespace MediaPlayer.Shell.IoC
{
    public class IocConfiguration : NinjectModule
    {
        public override void Load()
        {
            Bind<ISettingsProvider>().To<SettingsProvider>().InSingletonScope();
            Bind<MetadataReaderProvider>().To<TaglibMetadataReaderProvider>().Named("TaglibMetadataReaderProvider");
            Bind<IEnumerable<MetadataReaderProvider>>().To<MetadataReaderProvider[]>();
        }
    }
}
