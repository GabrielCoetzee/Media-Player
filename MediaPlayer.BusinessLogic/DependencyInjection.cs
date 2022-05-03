using MediaPlayer.BusinessLogic.Services.Abstract;
using MediaPlayer.BusinessLogic.Services.Concrete;
using Microsoft.Extensions.DependencyInjection;

namespace MediaPlayer.BusinessLogic
{
    public static class DependencyInjection
    {
        public static void AddServices(IServiceCollection services)
        {
            //Services

            services.AddTransient<IMediaService, MediaService>();
        }
    }
}
