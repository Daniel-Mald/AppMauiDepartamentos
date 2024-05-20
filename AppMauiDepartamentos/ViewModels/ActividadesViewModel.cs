using AppMauiDepartamentos.Models.DTOs;
using AppMauiDepartamentos.Models.Entities;
using AppMauiDepartamentos.Models.Validators;
using AppMauiDepartamentos.Repositories;
using AppMauiDepartamentos.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AppMauiDepartamentos.ViewModels
{
    [QueryProperty("Esadmin","EsAdmin")]
    public partial class ActividadesViewModel : ObservableObject
    {
        [ObservableProperty]
        public bool esadmin = false;
        Repository<Actividad> _repos = new();
        ActividadService _service = new();
        ActividadValidator _validator;
        LoginService _loginService;
        //public bool Boton { get; set; }
        [ObservableProperty]
        int isAdmin;

        public ActividadesViewModel()
        {
            //_repos = repository;
            //_service = service;
            _validator = new();
            // ChecarSiEsAdmin();
            if (IsAdmin == 1)
                Esadmin = true;
            else
                Esadmin = false;
            _service.AlActualizar += _service_AlActualizar;
            ActualizarActividades();
        }
        //public async Task ChecarSiEsAdmin()
        //{
        //    _loginService = new();
        //    int x = await _loginService.GetDepartamentoId();
        //    if (x == 1)
        //    {
        //        Esadmin = true;
        //    }
        //    else
        //    {
        //        Esadmin = false;
        //    }
        //    OnPropertyChanged(nameof(Esadmin));
        //}

        private  void _service_AlActualizar()
        {
            ActualizarActividades();
        }

        [ObservableProperty]
        public ActividadDTO? actividad = new();
        [ObservableProperty]
        public string error = "";
        [ObservableProperty]
        public Actividad? seleccionado;

        public ObservableCollection<Actividad> Actividades { get; set; } = new();

        async void ActualizarActividades()
        {
            Actividades.Clear();
            var y = _repos.GetAll();
            foreach (var l in y)
            {
                Actividades.Add(l);
            }
            //await ChecarSiEsAdmin();

            OnPropertyChanged();
        }
        void CambiarVista(string vista)
        {
            Shell.Current.GoToAsync(vista);
        }
        [RelayCommand]
        public void Cancelar()
        {
            Actividad = null;
            Seleccionado = null;
            Error = "";
            ActualizarActividades();
            CambiarVista("//PrincipalView");
        }
        [RelayCommand]
        public void VerAgregar()
        {
            Actividad = new();
            CambiarVista("//AddView");
        }
        [RelayCommand]
        public async Task Agregar()
        {
            try
            {
                if(Actividad!= null)
                {

                    var _result = _validator.Validate(Actividad);
                    if (_result.IsValid)
                    {
                        LoginService _loginService = new();
                        var token = _loginService.GetDepartamentoId();
                        Actividad.FechaCreacion = DateTime.UtcNow;
                        Actividad.FechaActualizacion = DateTime.UtcNow;
                        //Actividad.FechaRealizacion = new DateTime;
                        Actividad.IdDepartamento = await token;
                        Actividad.Estado = 1;
                        await _service.Add(Actividad);
                        Cancelar();
                    }
                    else
                    {
                        Error = string.Join("\n", _result.Errors.Select(x => x.ErrorMessage));
                    }
                }
            }
            catch (Exception e)
            {

                Error = e.Message;
            }
        }
        [RelayCommand]
        public void VerEditar(int id)
        {
            Seleccionado = _repos.Get(id);
            if(Seleccionado != null)
            {
                Error = "";
                CambiarVista("//UpdateView");
            

            }
        }
        [RelayCommand]
        public async Task Editar()
        {
            try
            {
                if (Seleccionado != null)
                {
                    ActividadValidatorNoDTO _validator2 = new();
                    var _result = _validator2.Validate(Seleccionado);
                    if (_result.IsValid)
                    {
                        LoginService _loginService = new(); // este service es de donde agarro el token
                        var token = _loginService.GetDepartamentoId(); //aqui obtengo el idDepartamento del token
                        var y = new ActividadDTO()
                        {
                            Titulo = Seleccionado.Titulo,
                            FechaActualizacion = DateTime.UtcNow,
                            IdDepartamento = await token,
                            FechaRealizacion = Seleccionado.FechaRealizacion,
                            Descripcion = Seleccionado.Descripcion,
                            FechaCreacion = Seleccionado.FechaCreacion,
                            Estado = Seleccionado.Estado,
                            Id = Seleccionado.Id

                        };
                        await _service.Update(y);
                        Cancelar();
                    }
                    else
                    {
                        Error = string.Join("\n", _result.Errors.Select(x => x.ErrorMessage));
                    }
                }
            }
            catch (Exception e)
            {

                Error = e.Message;
            }
        }
        [RelayCommand]
        public void VerEliminar(int id)
        {
            Seleccionado = _repos.Get(id);
            if(Seleccionado != null)
            {
                CambiarVista("//DeleteView");

            }
        }
        [RelayCommand]
        public async Task Eliminar()
        {
            if(Seleccionado!= null)
            {
                
                await _service.Delete(Seleccionado.Id);
                CambiarVista("//PrincipalView");
                ActualizarActividades();
                
            }
            
        }
        [RelayCommand]
        public void Logout()
        {
            _loginService = new();
            _loginService.Logout();
            _loginService.ReiniciarHilo();
            CambiarVista("//LoginView");
        }
        [RelayCommand]
        public void GoToDepartamentos()
        {
           // int id = await _loginService.GetDepartamentoId();
            if (Esadmin)
            {
                CambiarVista("//PrincipalDepartamentosView");
            }
        }
    }
}
