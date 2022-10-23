using MediaPlayer.Common.Constants;
using MediaPlayer.Model.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.Shuffle, typeof(ICommand))]
    public class ShuffleCommand : ICommand
    {
        private readonly Random _randomIdGenerator = new();

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return false;

            return vm.MediaItems.Count > 2;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

            if (!vm.MediaControlsViewModel.IsShuffled)
            {
                ShuffleMediaList(vm);
                return;
            }

            OrderMediaList(vm);
        }

        /// <summary>
        /// Ordering is easy since when we first populated list, we assigned it an id based on it's index, now we just sort by Id
        /// and add extra check at the end to remove currently playing item back to it's original spot. We need the check before moving so we
        /// don't try move it if it's already in the right spot
        /// </summary>
        /// <param name="vm"></param>
        public void OrderMediaList(MainViewModel vm)
        {
            var items = vm.MediaItems
                .Where(x => x != vm.SelectedMediaItem)
                .ToList();

            foreach (var mediaItem in vm.MediaItems.Where(x => items.Contains(x)).ToList())
                vm.MediaItems.Remove(mediaItem);

            var ordered = items.OrderBy(x => x.Id);

            vm.MediaItems.AddRange(ordered);

            var selectedIndex = vm.MediaItems.IndexOf(vm.SelectedMediaItem);

            if (selectedIndex != vm.SelectedMediaItem.Id)
                vm.MediaItems.Move(selectedIndex, vm.SelectedMediaItem.Id.GetValueOrDefault());

            vm.MediaControlsViewModel.IsShuffled = false;
        }

        /// <summary>
        /// Remove all but currently selected item in the list so we don't break bindings and also having currently playing item on
        /// top is a better user experience.
        /// </summary>
        /// <param name="vm"></param>
        public void ShuffleMediaList(MainViewModel vm)
        {
            var items = vm.MediaItems
                .Where(x => x != vm.SelectedMediaItem)
                .ToList();

            foreach (var mediaItem in vm.MediaItems.Where(x => items.Contains(x)).ToList())
                vm.MediaItems.Remove(mediaItem);

            var shuffled = items.OrderBy(x => _randomIdGenerator.Next());

            vm.MediaItems.AddRange(shuffled);

            vm.MediaControlsViewModel.IsShuffled = true;
        }
    }
}
