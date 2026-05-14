using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using Generic.Mediator;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.Settings.ViewModels;
using MediaPlayer.View.Views;
using MediaPlayer.ViewModel;

namespace MediaPlayer.Shell.MessengerRegs
{
    public class MessengerRegistrations
    {
        public static void OpenMainWindow(CompositionContainer container)
        {
            Messenger<MessengerMessages>.Register(MessengerMessages.OpenMainWindow, (args) =>
            {
                var view = container?.GetExportedValue<ViewMediaPlayer>();

                view.Show();
            });
        }

        public static void AddMedia(CompositionContainer container)
        {
            Messenger<MessengerMessages>.Register(MessengerMessages.AddMedia, async (args) =>
            {
                var vm = container?.GetExportedValue<MainViewModel>();

                await vm.AddMediaAsync(args as IEnumerable<string>);
            });
        }

        public static void SaveChangesToDirtyFiles(CompositionContainer container)
        {
            Messenger<MessengerMessages>.Register(MessengerMessages.SaveChangesToDirtyFiles, async (args) =>
            {
                var vm = container?.GetExportedValue<MainViewModel>();

                await vm.SaveChangesAsync();

                var shutdownApplication = (bool)args;

                if (shutdownApplication)
                    Application.Current.Shutdown(0);
            });
        }

        public static void AutoAdjustAccent(CompositionContainer container)
        {
            Messenger<MessengerMessages>.Register(MessengerMessages.AutoAdjustAccent, async (args) =>
            {
                var audioItem = container?.GetExportedValue<MainViewModel>().SelectedMediaItem as AudioItem;

                var vm = container?.GetExportedValue<ThemeViewModel>();

                await vm.AutoAdjustAccentAsync(audioItem?.AlbumArt);
            });
        }
    }
}
