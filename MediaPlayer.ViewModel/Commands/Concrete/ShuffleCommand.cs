using MediaPlayer.Common.Constants;
using MediaPlayer.Model.Collections;
using System;
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

            if (!vm.IsMediaItemsShuffled)
            {
                ShuffleMediaList(vm);
                return;
            }

            OrderMediaList(vm);
        }

        public void OrderMediaList(MainViewModel vm)
        {
            vm.MediaItems = new MediaItemObservableCollection(vm.MediaItems.OrderBy(x => x.Id));

            vm.IsMediaItemsShuffled = false;
        }

        public void ShuffleMediaList(MainViewModel vm)
        {
            vm.MediaItems = new MediaItemObservableCollection(vm.MediaItems
                .OrderBy(x => x != vm.SelectedMediaItem)
                .ThenBy(x => _randomIdGenerator.Next()));

            vm.IsMediaItemsShuffled = true;
        }
    }
}
