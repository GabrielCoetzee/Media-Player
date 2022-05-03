﻿using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.BusinessLogic.Commands.Abstract.EventTriggers;
using MediaPlayer.BusinessLogic.Commands.Concrete;
using MediaPlayer.BusinessLogic.Commands.Concrete.EventTriggers;
using MediaPlayer.BusinessLogic.Services.Abstract;
using MediaPlayer.BusinessLogic.Services.Concrete;
using MediaPlayer.BusinessLogic.State.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace MediaPlayer.BusinessLogic
{
    public static class DependencyInjection
    {
        public static void AddServices(IServiceCollection services)
        {
            // State

            services.AddSingleton<IState, State.Concrete.State>();

            //Commands

            services.AddTransient<IOpenSettingsWindowCommand, OpenSettingsWindowCommand>();
            services.AddTransient<IShuffleCommand, ShuffleCommand>();
            services.AddTransient<IAddMediaCommand, AddMediaCommand>();
            services.AddTransient<IPlayPauseCommand, PlayPauseCommand>();
            services.AddTransient<IMuteCommand, MuteCommand>();
            services.AddTransient<IPreviousTrackCommand, PreviousTrackCommand>();
            services.AddTransient<IStopCommand, StopCommand>();
            services.AddTransient<INextTrackCommand, NextTrackCommand>();
            services.AddTransient<IRepeatMediaListCommand, RepeatMediaListCommand>();
            services.AddTransient<IClearMediaListCommand, ClearMediaListCommand>();

            //Event Trigger Commands

            services.AddTransient<IMediaOpenedCommand, MediaOpenedCommand>();
            services.AddTransient<ISeekbarThumbStartedDraggingCommand, SeekbarThumbStartedDraggingCommand>();
            services.AddTransient<ISeekbarThumbCompletedDraggingCommand, SeekbarThumbCompletedDraggingCommand>();
            services.AddTransient<ISeekbarPreviewMouseUpCommand, SeekbarPreviewMouseUpCommand>();
            services.AddTransient<ITopMostGridDragEnterCommand, TopMostGridDragEnterCommand>();
            services.AddTransient<ITopMostGridDropCommand, TopMostGridDropCommand>();
            services.AddTransient<ILoadThemeOnWindowLoadedCommand, LoadThemeOnWindowLoadedCommand>();
            services.AddTransient<IFocusOnPlayPauseButtonCommand, FocusOnPlayPauseButtonCommand>();
            services.AddTransient<ISaveSettingsCommand, SaveSettingsCommand>();

            //Services

            services.AddTransient<IMediaListService, MediaListService>();
        }
    }
}
