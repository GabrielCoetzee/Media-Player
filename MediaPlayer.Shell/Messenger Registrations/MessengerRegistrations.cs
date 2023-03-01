using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
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
                var view = container?.GetExports<ViewMediaPlayer>().Single().Value;

                view.Show();
            });
        }

        public static void OpenApplicationSettingsDialog(CompositionContainer container)
        {
            Messenger<MessengerMessages>.Register(MessengerMessages.OpenApplicationSettingsDialog, (args) =>
            {
                var view = container?.GetExports<ViewApplicationSettings>().Single().Value;

                view.ShowDialog();
            });
        }

        public static void ProcessFilePaths(CompositionContainer container)
        {
            Messenger<MessengerMessages>.Register(MessengerMessages.ProcessFilePaths, async (args) =>
            {
                var vm = container?.GetExports<MainViewModel>().Single().Value;

                await vm.ProcessFilePathsAsync(args as IEnumerable<string>);
            });
        }

        public static void SaveChangesToDirtyFiles(CompositionContainer container)
        {
            Messenger<MessengerMessages>.Register(MessengerMessages.SaveChangesToDirtyFiles, async (args) =>
            {
                var vm = container?.GetExports<MainViewModel>().Single().Value;

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
                var audioItem = container?.GetExports<MainViewModel>().Single().Value.SelectedMediaItem as AudioItem;

                var vm = container?.GetExports<ThemeViewModel>().Single().Value;

                await vm.AutoAdjustAccentAsync(audioItem?.AlbumArt);
            });
        }
    }
}
