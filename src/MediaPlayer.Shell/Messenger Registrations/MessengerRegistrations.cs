using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using Generic.Mediator;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Common.Model;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.View.Views;
using MediaPlayer.ViewModel;
using MediaPlayer.ViewModel.Services.Abstract;
using MediaPlayer.ViewModel.ViewModels;

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
                var vm = container?.GetExportedValue<QueueViewModel>();

                await vm.AddMediaAsync(args as IEnumerable<string>);
            });
        }

        public static void SaveChangesToDirtyFiles(CompositionContainer container)
        {
            Messenger<MessengerMessages, ShutdownArgs>.Register(MessengerMessages.SaveChangesToDirtyFiles, async (args) =>
            {
                var loader = container?.GetExportedValue<IMediaLoader>();
                var updater = container?.GetExportedValue<IMetadataUpdateService>();
                var queue = container?.GetExportedValue<QueueViewModel>();

                loader.Cancel();
                updater.Cancel();

                await queue.SaveDirtyMetadataAsync();

                if (args.IsEnabled)
                    Application.Current.Shutdown(0);
            });
        }

        public static void AutoAdjustAccent(CompositionContainer container)
        {
            Messenger<MessengerMessages>.Register(MessengerMessages.AutoAdjustAccent, async (args) =>
            {
                var audioItem = container?.GetExportedValue<PlayerShellViewModel>().QueueViewModel.SelectedMediaItem as AudioItem;

                var vm = container?.GetExportedValue<ThemeViewModel>();

                await vm.AutoAdjustAccentAsync(audioItem?.AlbumArt);
            });
        }
    }
}
