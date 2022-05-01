using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.BusinessLogic.Commands.Concrete;
using MediaPlayer.BusinessLogic.Services.Abstract;
using MediaPlayer.BusinessLogic.Services.Concrete;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaPlayer.BusinessLogic
{
    public static class DependencyInjection
    {
        public static void AddServices(IServiceCollection services)
        {
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
            services.AddTransient<IMediaOpenedCommand, MediaOpenedCommand>();
            services.AddTransient<IClearMediaListCommand, ClearMediaListCommand>();
            services.AddTransient<ISeekbarThumbStartedDraggingCommand, SeekbarThumbStartedDraggingCommand>();
            services.AddTransient<ISeekbarThumbCompletedDraggingCommand, SeekbarThumbCompletedDraggingCommand>();
            services.AddTransient<ISeekbarPreviewMouseUpCommand, SeekbarPreviewMouseUpCommand>();

            //Services

            services.AddTransient<IMediaListService, MediaListService>();
        }
    }
}
