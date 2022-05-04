using MediaPlayer.ViewModel.Commands.Abstract;
using MediaPlayer.ViewModel.Commands.Concrete;
using MediaPlayer.ViewModel.EventTriggers.Abstract;
using MediaPlayer.ViewModel.EventTriggers.Concrete;
using Microsoft.Extensions.DependencyInjection;

namespace MediaPlayer.ViewModel
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
        }
    }
}
