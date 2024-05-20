using AppMauiDepartamentos.Repositories;
using AppMauiDepartamentos.Services;
using AppMauiDepartamentos.ViewModels;
using Microsoft.Extensions.Logging;

namespace AppMauiDepartamentos
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("fontello.ttf", "Icons");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton(typeof(IRepository<>),typeof(Repository<>));
            builder.Services.AddSingleton<ActividadService>();
            builder.Services.AddSingleton<ActividadesViewModel>();
            builder.Services.AddSingleton<LoginService>();

            return builder.Build();
        }
    }
}
