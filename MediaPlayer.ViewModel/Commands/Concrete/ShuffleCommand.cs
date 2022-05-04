using MediaPlayer.Model;
using MediaPlayer.Model.Collections;
using MediaPlayer.ViewModel.Commands.Abstract;
using System;
using System.Linq;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    public class ShuffleCommand : IShuffleCommand
    {
        private readonly Random _randomIdGenerator = new();

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is not ViewModelMediaPlayer vm)
                return false;

            return vm.MediaItems.Count > 2;
        }

        public void Execute(object parameter)
        {
            if (parameter is not ViewModelMediaPlayer vm)
                return;

            if (!vm.IsMediaItemsShuffled)
            {
                ShuffleMediaList(vm);

                return;
            }

            OrderMediaList(vm);
        }

        public void OrderMediaList(ViewModelMediaPlayer vm)
        {
            vm.MediaItems = new MediaItemObservableCollection(vm.MediaItems.OrderBy(x => x.Id));

            vm.IsMediaItemsShuffled = false;
        }

        public void ShuffleMediaList(ViewModelMediaPlayer vm)
        {
            vm.MediaItems = new MediaItemObservableCollection(vm.MediaItems
                .OrderBy(x => x != vm.SelectedMediaItem)
                .ThenBy(x => _randomIdGenerator.Next()));

            vm.IsMediaItemsShuffled = true;
        }
    }
}
