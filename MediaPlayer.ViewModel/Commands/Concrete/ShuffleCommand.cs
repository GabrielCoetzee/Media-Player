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

        public void OrderMediaList(MainViewModel vm)
        {
            var items = vm.MediaItems
                .Where(x => x != vm.SelectedMediaItem)
                .ToList();

            foreach (var mediaItem in vm.MediaItems.Where(x => items.Contains(x)).ToList())
                vm.MediaItems.Remove(mediaItem);

            var ordered = items.OrderBy(x => x.Id);

            foreach (var mediaItem in ordered)
                vm.MediaItems.Add(mediaItem);

            var selectedIndex = vm.MediaItems.IndexOf(vm.SelectedMediaItem);

            if (selectedIndex != vm.SelectedMediaItem.Id)
                vm.MediaItems.Move(selectedIndex, vm.SelectedMediaItem.Id);

            vm.MediaControlsViewModel.IsShuffled = false;
        }

        public void ShuffleMediaList(MainViewModel vm)
        {
            var items = vm.MediaItems
                .Where(x => x != vm.SelectedMediaItem)
                .ToList();

            foreach (var mediaItem in vm.MediaItems.Where(x => items.Contains(x)).ToList())
                vm.MediaItems.Remove(mediaItem);

            var shuffled = items.OrderBy(x => _randomIdGenerator.Next());

            foreach (var mediaItem in shuffled)
                vm.MediaItems.Add(mediaItem);

            vm.MediaControlsViewModel.IsShuffled = true;
        }
    }
}
