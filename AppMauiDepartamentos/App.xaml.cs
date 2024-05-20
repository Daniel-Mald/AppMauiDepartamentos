using AppMauiDepartamentos.Services;
//using AuthenticationServices;

namespace AppMauiDepartamentos
{
    public partial class App : Application
    {
        public static ActividadService _service = new();
        public static LoginService _loginService = new();
        public static DepartamentoService _departmentoService = new();

         public static Thread? _thread { get; set; }
        public static Thread? _thread2 { get; set; }

        public App()
        {
            InitializeComponent();
            //_service = service;
           //_thread = new Thread(Sincronizador) { IsBackground = true };
           // _thread.Start();
            MainPage = new AppShell();
        }
        async static void Sincronizador()
        {
            //int rol = await _loginService.GetDepartamentoId();
            while (true)
            {
                await _service.GetActividades();
                Thread.Sleep(TimeSpan.FromSeconds(30));
            }
        }
        async static void SincronizadorDepartametos()
        {
            while (true)
            {
                await _departmentoService.GetDepartamentos();
                Thread.Sleep(TimeSpan.FromSeconds(30));
            }
        }
        public static void NuevoHilo()
        {
            _thread = new Thread(Sincronizador) { IsBackground = true };
            _thread.Start();
            _thread2 = new Thread(SincronizadorDepartametos) { IsBackground = true };
            _thread2.Start();

        }
        //public  void IniciarHilo()
        //{
        //    _thread.Start();
        //}
    }
}
