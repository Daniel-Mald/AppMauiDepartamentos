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
using System.Text;
using System.Threading.Tasks;

namespace AppMauiDepartamentos.ViewModels
{
    public partial class DepartamentoViewModel:ObservableObject
    {
        Repository<Departamento> _repos = new();
        DepartamentoService _service = new();
        DepartamentoDTOValidator _validator;
        //LoginService _loginService;
        public DepartamentoViewModel()
        {
            //_repos = repository;
            //_service = service;
            _validator = new(_repos);

            _service.AlActualizar += _service_AlActualizar;
            ActualizarDepartamentos();
        }

        private void _service_AlActualizar()
        {
            ActualizarDepartamentos();
        }

        [ObservableProperty]
        public DepartamentoDTO? departamento = new();
        public DepartamentoDTO? DepartamentoTemporal { get; set; } = new(); 


        [ObservableProperty]
        public string error = "";
        [ObservableProperty]
        public Departamento? seleccionado;

        public ObservableCollection<Departamento> Departamentos { get; set; } = new();

        void ActualizarDepartamentos()
        {
            Departamentos.Clear();
            var y = _repos.GetAll();
            foreach (var l in y)
            {
                Departamentos.Add(l);
            }
            OnPropertyChanged();
        }
        void CambiarVista(string vista)
        {
            Shell.Current.GoToAsync(vista);
        }
        [RelayCommand]
        public void Cancelar()
        {
            Departamento = null;
            Seleccionado = null;
            Error = "";
            ActualizarDepartamentos();
            CambiarVista("//PrincipaDepartamentoslView");
        }
        [RelayCommand]
        public void VerAgregarD()
        {
            Departamento = new();
           // DepartamentoTemporal = new();
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
                    if (_result.IsValid)
                    {


                        Departamento d = new()
                        {
                            UserName = Departamento.UserName,
                            Nombre = Departamento.Nombre,
                            //Password = Departamento.Password,

                            SuperiorId = DepartamentoTemporal.Id
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
            if (n != null && Departamento!= null)
            {
                Departamento.SuperiorId =n.SuperiorId;
                Departamento.Nombre = n.Nombre;
                Departamento.UserName = n.UserName;
                Departamento.Password = "";
                Departamento.Id = n.Id;
                Error = "";
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

                    var _result = _validator.Validate(Departamento);
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
                CambiarVista("//DeleteDepartamentoView");

            }
        }
        [RelayCommand]
        public async Task Eliminar()
        {
            if (Seleccionado != null)
            {

                await _service.Delete(Seleccionado.Id);
                CambiarVista("PrincipalDepartamentosView");
                ActualizarDepartamentos();
            }

        }
        [RelayCommand]
        public void GoBack()
        {
            CambiarVista("//PrincipalView");
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
