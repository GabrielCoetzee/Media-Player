﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Generic.Mediator;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.View.Views;
using MediaPlayer.ViewModel;

namespace MediaPlayer.Shell.MessengerRegs
{
    public class MessengerRegistrations
    {
        public static void OpenMediaPlayerMainWindow(CompositionContainer container)
        {
            Messenger<MessengerMessages>.Register(MessengerMessages.OpenMediaPlayerMainWindow, (args) =>
            {
                var view = container?.GetExports<ViewMediaPlayer>().Single().Value;

                view.Show();
            });
        }

        public static void OpenApplicationSettingsWindow(CompositionContainer container)
        {
            Messenger<MessengerMessages>.Register(MessengerMessages.OpenApplicationSettings, (args) =>
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

                await vm.ProcessDroppedContentAsync(args as IEnumerable<string>);

                CommandManager.InvalidateRequerySuggested();
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
    }
}
