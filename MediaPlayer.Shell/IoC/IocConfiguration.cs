using System.Collections.Generic;
using MediaPlayer.ApplicationSettings.SettingsProvider;
using MediaPlayer.ApplicationSettings.ThemeChanger;
using MediaPlayer.MetadataReaders.Abstract;
using MediaPlayer.MetadataReaders.Derived;
using Ninject.Modules;

namespace MediaPlayer.Shell.IoC
{
    public class IocConfiguration : NinjectModule
    {
        public override void Load()
        {
            Bind<IThemeChanger>().To<ThemeChanger>().InSingletonScope();
            Bind<ISettingsProvider>().To<SettingsProvider>().InSingletonScope();
            Bind<MetadataReaderProvider>().To<TaglibMetadataReaderProvider>().Named("TaglibMetadataReaderProvider");
            Bind<IEnumerable<MetadataReaderProvider>>().To<MetadataReaderProvider[]>();
        }
    }
}
