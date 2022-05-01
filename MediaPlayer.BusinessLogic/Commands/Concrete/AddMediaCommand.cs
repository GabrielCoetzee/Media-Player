﻿using MediaPlayer.ApplicationSettings;
using MediaPlayer.BusinessEntities.Objects.Base;
using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete
{
    public class AddMediaCommand : IAddMediaCommand
    {
        readonly ModelMediaPlayer _model;
        readonly ISettingsProvider _settingsProvider;
        readonly MetadataReaderResolver _metadataReaderResolver;

        public AddMediaCommand(ModelMediaPlayer model, ISettingsProvider settingsProvider, MetadataReaderResolver metadataReaderResolver)
        {
            _model = model;
            _settingsProvider = settingsProvider;
            _metadataReaderResolver = metadataReaderResolver;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var chooseFiles = new OpenFileDialog
            {
                Title = "Choose Files",
                DefaultExt = this._settingsProvider.SupportedFileFormats.First(),
                Filter = CreateDialogFilter(),
                Multiselect = true
            };

            var result = chooseFiles.ShowDialog();

            if (result != DialogResult.OK)
                return;

            var metadataReader = _metadataReaderResolver.Resolve(MetadataReaders.Taglib);

            var mediaItems = chooseFiles.FileNames.Select(file => metadataReader.GetFileMetadata(file)).ToList();

            AddToMediaList(mediaItems);
        }

        private string CreateDialogFilter()
        {
            return string.Join("|", $"Supported Formats ({AppendSupportedFormats(",")})", AppendSupportedFormats(";"));
        }

        private string AppendSupportedFormats(string seperator)
        {
            return this._settingsProvider.SupportedFileFormats.Aggregate(string.Empty, (current, format) => current + $"*{format}{(this._settingsProvider.SupportedFileFormats.Last() != format ? seperator : string.Empty)}");
        }

        public void AddToMediaList(IEnumerable<MediaItem> mediaItems)
        {
            this._model.MediaItems.AddRange(mediaItems);

            if (_model.SelectedMediaItem != null || this._model.IsMediaListEmpty())
                return;

            this._model.SelectMediaItem(this._model.GetFirstMediaItemIndex());
            this._model.PlayMedia();

            RefreshUIBindings();
        }
        private void RefreshUIBindings()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
