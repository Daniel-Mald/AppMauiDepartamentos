using AppMauiDepartamentos.Models.Entities;
using AppMauiDepartamentos.Repositories;
using AppMauiDepartamentos.Services;
using AppMauiDepartamentos.ViewModels;
using AppMauiDepartamentos.Views;
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
                    //fonts.AddFont("fontello.ttf", "Icons");
                    fonts.AddFont("Font Awesome 6 Free-Regular-400.otf","iconoss");
                });
           // builder.UseFluentMauiIcons().UseMaterialMauiIcons().UseCupertinoMauiIcons();


#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton(typeof(IRepository<>),typeof(Repository<>));
            builder.Services.AddSingleton<Repository<Actividad>>();
            builder.Services.AddSingleton<Repository<Departamento>>();


            builder.Services.AddSingleton<ActividadService>();
            builder.Services.AddSingleton<ActividadesViewModel>();
            builder.Services.AddSingleton<DepartamentoViewModel>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<DepartamentoService>();

            builder.Services.AddSingleton<LoginService>();
            builder.Services.AddSingleton<AppShell>(x => new AppShell()
            {
                BindingContext = x.GetRequiredService<ActividadesViewModel>()
            });
            //builder.Services.AddTransient<UpdateView>(x => new UpdateView()
            //{
            //    BindingContext = x.GetRequiredService<ActividadesViewModel>()
            //});
            //builder.Services.AddSingleton<DeleteView>(x => new DeleteView()
            //{
            //    BindingContext = x.GetRequiredService<ActividadesViewModel>()
            //});
            builder.Services.AddSingleton<PrincipalView>(x => new PrincipalView()
            {
                BindingContext = x.GetRequiredService<ActividadesViewModel>()
            });
            builder.Services.AddSingleton<AddDepartamentoView>(x => new AddDepartamentoView()
            {
                BindingContext = x.GetRequiredService<DepartamentoViewModel>()
            });
            builder.Services.AddSingleton<DeleteDepartamentoView>(x => new DeleteDepartamentoView()
            {
                BindingContext = x.GetRequiredService<DepartamentoViewModel>()
            });
            builder.Services.AddSingleton<UpdateDepartamentoView>(x => new UpdateDepartamentoView()
            {
                BindingContext = x.GetRequiredService<DepartamentoViewModel>()
            });
            builder.Services.AddSingleton<PrincipalDepartamentosView>(x => new PrincipalDepartamentosView()
            {
                BindingContext = x.GetRequiredService<DepartamentoViewModel>()
            });
            builder.Services.AddSingleton<LoginView>(x => new LoginView()
            {
                BindingContext = x.GetRequiredService<LoginViewModel>()
            });
            return builder.Build();
        }
    }
}
