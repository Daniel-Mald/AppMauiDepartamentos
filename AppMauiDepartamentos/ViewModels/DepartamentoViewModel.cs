using AppMauiDepartamentos.Models.DTOs;
using AppMauiDepartamentos.Models.Entities;
using AppMauiDepartamentos.Models.Validators;
using AppMauiDepartamentos.Repositories;
using AppMauiDepartamentos.Services;
using AppMauiDepartamentos.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using MetalPerformanceShaders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMauiDepartamentos.ViewModels
{
    public partial class DepartamentoViewModel:ObservableObject
    {
        Repository<Departamento> _repos;
        Repository<Actividad> _reposAct;

        DepartamentoService _service;
        DepartamentoDTOValidator _validator;
        DepartamentoEditarValidator _validatorEditar;
        ActividadService _activationService;
        //LoginService _loginService;
        public DepartamentoViewModel(DepartamentoService ser ,ActividadService actividadService,
            Repository<Actividad> ra,
            Repository<Departamento> rd)
        {
            //_repos = repository;
            //_service = service;
            _repos = rd;
            _service = ser;
            _reposAct = ra;
            _validator = new(_repos);
            _validatorEditar = new(_repos);
            _activationService = actividadService;
            App._departmentoService.AlActualizar += _departmentoService_AlActualizar;
           // _service.AlActualizar += _service_AlActualizar;
            ActualizarDepartamentos();
        }

        private void _departmentoService_AlActualizar()
        {
            ActualizarDepartamentos();
        }

        private void _service_AlActualizar()
        {
            ActualizarDepartamentos();
        }

        [ObservableProperty]
        public DepartamentoDTO? departamento = new();
        [ObservableProperty]
         Departamento? departamentoTemporal  = new();

        [ObservableProperty]
        Departamento? departamentoTemporal2 = new();


        [ObservableProperty]
        public string error = "";
        [ObservableProperty]
        public Departamento? seleccionado;

        
         public ObservableCollection<Departamento> Departamentos { get; set; } = new();
        //public ObservableCollection<Departamento> Departamentos { get; set; } = new();

        void ActualizarDepartamentos()
        {
            Departamentos = new();
            var y =  _repos.GetAll();
            foreach (var l in y)
            {
                Departamentos.Add(l);
            }
            OnPropertyChanged(nameof(Departamentos));
        }
        void CambiarVista(string vista)
        {
            //var nuevaVista = new PrincipalDepartamentosView();
            //Shell.curr
            Shell.Current.GoToAsync(vista);
        }
        [RelayCommand]
        public void Cancelar()
        {
            //var nuevaVista = new PrincipalDepartamentosView();
            //var currentPage = Shell.Current.Navigation.NavigationStack.LastOrDefault();
            //Shell.Current.Navigation.PushAsync(nuevaVista);
            Shell.Current.GoToAsync("//PrincipalDepartamentosView");
            Departamento = new();
            DepartamentoTemporal = new();
            Seleccionado = null;
            Error = "";
             ActualizarDepartamentos();
        }
        [RelayCommand]
        public void VerAgregarD()
        {
            Departamento = new();
            DepartamentoTemporal = new() { Id = 0};
            //DepartamentoDTO x = (DepartamentoDTO)N;
            CambiarVista("//AddDepartamentoView");
        }
        [RelayCommand]
        public async Task Agregar()
        {
            try
            {
                if (Departamento != null)
                {

                    var _result = _validator.Validate(Departamento);
                    if (_result.IsValid )
                    {


                        DepartamentoDTO d = new()
                        {
                            Username = Departamento.Username,
                            Nombre = Departamento.Nombre,
                            Password = Departamento.Password,
                            
                            IdSuperior = DepartamentoTemporal.Id==0?null:DepartamentoTemporal.Id,
                            DepartamentoSuperior = _repos.Get(DepartamentoTemporal.Id).Nombre
                        };
                        Seleccionado = null;
                        await _service.Add(d);
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
            var n = _repos.Get(id);
            if(DepartamentoTemporal == null)
            {
                DepartamentoTemporal = new();
            }
            if (DepartamentoTemporal!= null)
            {
               
                DepartamentoTemporal = n;
                if(n.SuperiorId != n.SuperiorId || n.SuperiorId != 0 || n.SuperiorId != null)
                {
                    if(n.SuperiorId == null) 
                    {
                        DepartamentoTemporal2 = new();
                    }
                    else
                    {
                        int rr = (int)n.SuperiorId;


                        DepartamentoTemporal2 = new()
                        {
                            Id = rr
                        };
                        
                    }
                }
                //Departamento.IdSuperior =n.SuperiorId;
                //Departamento.Nombre = n.Nombre;
                //Departamento.Username = n.UserName;
                //Departamento.Password = "";
                //Departamento.Id = n.Id;
                Error = "";
                var nuevaVista = new UpdateDepartamentoView();
                var currentPage = Shell.Current.Navigation.NavigationStack.LastOrDefault();
                
                //Shell.Current.Navigation.PushAsync(nuevaVista);
                CambiarVista("//UpdateDepartamentoView");
               

            }
        }
        [RelayCommand]
        public async Task Editar()
        {
            try
            {
                if (Departamento != null)
                {
                    //ActividadValidatorNoDTO _validator2 = new();
                    var x = _repos.Get(DepartamentoTemporal.Id);
                    Departamento.Username = DepartamentoTemporal.UserName;
                    Departamento.Nombre = DepartamentoTemporal.Nombre;
                    if(x.Id!= x.Id || x.Id!= 0)
                    {
                        
                        Departamento.IdSuperior = DepartamentoTemporal2.Id== 0?null:DepartamentoTemporal.Id;
                    }
                    
                    Departamento.Id = DepartamentoTemporal.Id;
                   

                    var _result = _validatorEditar.Validate(Departamento);
                    if (_result.IsValid)
                    {
                        //   LoginService _loginService = new(); // este service es de donde agarro el token
                        // var token = _loginService.GetDepartamentoId(); //aqui obtengo el idDepartamento del token
                        //var y = new Departamento()
                        //{
                        //    Nombre = Departamento.Nombre,
                        //    SuperiorId = Departamento.SuperiorId,
                        //    UserName = Departamento.UserName

                        //};
                        await _service.Update(Departamento);
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
            if (Seleccionado != null)
            {
                //var nuevaVista = new DeleteDepartamentoView();
                //var currentPage = Shell.Current.Navigation.NavigationStack.LastOrDefault();

                //Shell.Current.Navigation.PushAsync(nuevaVista);
                CambiarVista("//DeleteDepartamentoView");

            }
        }
        [RelayCommand]
        public async Task Eliminar()
        {
            if (Seleccionado != null)
            {
                await _service.Delete(Seleccionado.Id);
                Cancelar();
                ActualizarDepartamentos();
            }

        }
        [RelayCommand]
        public async void GoBack()
        {
            CambiarVista("//PrincipalView");


            bool Admin = await _service._loginService.EsAdmin();


            await Shell.Current.GoToAsync($"//PrincipalView?EsAdmin={Admin}");
        }
        //[RelayCommand]
        //public void Logout()
        //{
        //    _loginService = new();
        //    _loginService.Logout();
        //    _loginService.ReiniciarHilo();
        //    CambiarVista("//LoginView");
        //}
    }
}
