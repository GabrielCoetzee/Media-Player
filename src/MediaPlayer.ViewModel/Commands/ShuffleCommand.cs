using MediaPlayer.Common.Constants;
using MediaPlayer.ViewModel.ViewModels;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands
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
            if (parameter is not MediaControlsViewModel vm)
                return false;

            return vm.QueueViewModel.MediaItems.Count > 2;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MediaControlsViewModel vm)
                return;

            if (vm.IsShuffled)
            {
                OrderMediaList(vm);
                return;
            }

            ShuffleMediaList(vm);
        }

        /// <summary>
        /// Ordering is easy since when we first populated list, we assigned it an id based on it's index, now we just sort by Id
        /// and add extra check at the end to remove currently playing item back to it's original spot. We need the check before moving so we
        /// don't try move it if it's already in the right spot
        /// </summary>
        /// <param name="vm"></param>
        public void OrderMediaList(MediaControlsViewModel vm)
        {
            var queue = vm.QueueViewModel;

            var items = queue.MediaItems
                .Where(x => x != queue.SelectedMediaItem)
                .ToList();

            var remove = queue.MediaItems.Where(x => items.Contains(x)).ToList();

            var ordered = items.OrderBy(x => x.Id);

            queue.MediaItems.RemoveRange(remove);
            queue.MediaItems.AddRange(ordered);

            var selectedIndex = queue.MediaItems.IndexOf(queue.SelectedMediaItem);

            if (selectedIndex != queue.SelectedMediaItem.Id)
                queue.MediaItems.Move(selectedIndex, queue.SelectedMediaItem.Id.GetValueOrDefault());

            vm.IsShuffled = false;
        }

        /// <summary>
        /// Remove all but currently selected item in the list so we don't break bindings and also having currently playing item on
        /// top is a better user experience.
        /// </summary>
        /// <param name="vm"></param>
        public void ShuffleMediaList(MediaControlsViewModel vm)
        {
            var queue = vm.QueueViewModel;

            var items = queue.MediaItems
                .Where(x => x != queue.SelectedMediaItem)
                .ToList();

            var remove = queue.MediaItems.Where(x => items.Contains(x)).ToList();

            var shuffled = items.OrderBy(x => _randomIdGenerator.Next());

            queue.MediaItems.RemoveRange(remove);
            queue.MediaItems.AddRange(shuffled);

            vm.IsShuffled = true;
        }
    }
}
