using AppMauiDepartamentos.Models.Entities;
using AppMauiDepartamentos.Repositories;
using AppMauiDepartamentos.Services;
using System.Threading;
//using AuthenticationServices;

namespace AppMauiDepartamentos
{
    public partial class App : Application
    {
        //public static ActividadService _service = new(_loginService);
        public static ActividadService _service;

        public static LoginService _loginService;
        public static DepartamentoService _departmentoService;
        static Repository<Actividad> _reposA;

         public static Thread? _thread { get; set; }
        public static Thread? _thread2 { get; set; }
        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        //private static Thread _thread2;


        public App(ActividadService acs , DepartamentoService dss , LoginService ls,
            Repository<Actividad> ra)
        {

            InitializeComponent();
            //_service = service;
            //_thread = new Thread(Sincronizador) { IsBackground = true };
            // _thread.Start();
            _reposA = ra;
            _service = acs;
            _departmentoService = dss;
            _loginService = ls;
            _loginService.Logout();
            MainPage = new AppShell();
        }
        async static void Sincronizador(CancellationToken token)
        {
            
            while (!token.IsCancellationRequested)
            {
                await _service.GetActividades();
                Thread.Sleep(TimeSpan.FromSeconds(20));

            }
        }
        async static void SincronizadorDepartametos()
        {
            
            while (true)
            {
                await _departmentoService.GetDepartamentos();
                Thread.Sleep(TimeSpan.FromSeconds(20));
            }
        }
        public async static Task CerrarHilos()
        {
            
            if (_thread != null && (_thread.IsAlive || _thread.ThreadState == ThreadState.Stopped))
            {
                _cancellationTokenSource.Cancel(); // Solicitar la cancelación

                try
                {
                    _thread.Join(); // Esperar a que el hilo termine
                }
                catch (ThreadInterruptedException ex)
                {
                    
                }
            }
            if (_thread2 != null && _thread2.IsAlive)
            {
                _cancellationTokenSource.Cancel(); // Solicitar la cancelación

                try
                {
                    _thread2.Join(); // Esperar a que el hilo termine
                }
                catch (ThreadInterruptedException ex)
                {
                    
                }
            }

            await Task.CompletedTask;

        }
        public async static Task NuevoHilo()
        {
            
            await CerrarHilos();
            //_thread.Start();
            _cancellationTokenSource = new CancellationTokenSource();
            _thread = new Thread(() => Sincronizador(_cancellationTokenSource.Token)){ IsBackground = true };
            _thread.Start();
            _thread2 = new Thread(SincronizadorDepartametos) { IsBackground = true };
            _thread2.Start();

        }
        static async Task LimpiarActividades()
        {
            try
            {
                List<Actividad> acts = _reposA.GetAll().ToList();
                foreach (var act in acts)
                {
                    _reposA.Delete(act);
                }
                
            }
            catch (Exception ejeje)
            {
            }
        }
        //public  void IniciarHilo()
        //{
        //    _thread.Start();
        //}
    }
}
