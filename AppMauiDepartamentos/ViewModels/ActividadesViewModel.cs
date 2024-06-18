using AppMauiDepartamentos.Models.DTOs;
using AppMauiDepartamentos.Models.Entities;
using AppMauiDepartamentos.Models.Validators;
using AppMauiDepartamentos.Repositories;
using AppMauiDepartamentos.Services;
using AppMauiDepartamentos.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using MessageUI;
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
        [ObservableProperty]
        public string img = "";
        Repository<Actividad> _repos = new();
        // ActividadService _service = new();
        ActividadService _service;

        ActividadValidator _validator;
        LoginService _loginService;
        //public bool Boton { get; set; }
        [ObservableProperty]
        int isAdmin;

        public ActividadesViewModel(ActividadService acs, LoginService ls)
        {
            _service = acs;
            _loginService = ls;
            //_repos = repository;
            //_service = service;
            _validator = new();
            // ChecarSiEsAdmin();
            EsAdministrador();
            // _service.AlActualizar += _service_AlActualizar;
            App._service.AlActualizar += _service_AlActualizar2;
           // App._service.AlActualizarImagenes += _service_AlActualizarImagenes;
            //_service.AlActualizar += _service_AlActualizar1;
           // _service.AlActualizar += _service_AlActualizar1;
            ActualizarActividades(false);
        }

        //private void _service_AlActualizarImagenes(object? sender, List<ActividadDTO> e)
        //{
        //    ActividadesImg.Clear();
        //    foreach (var item in e)
        //    {
        //        var x = new ActividadConImagen
        //        {
        //            Actividad = item,
        //            Imagen = ConvertirImagen(item.Imagen)
        //        };
        //    }
        //}
        public  ImageSource? ConvertirImagen(string base64Image)
        {
            if (string.IsNullOrEmpty(base64Image))
            {
                return null;
            }

            byte[] imageBytes = Convert.FromBase64String(base64Image);
            using (var ms = new MemoryStream(imageBytes))
            {
                return ImageSource.FromStream(() => ms);
            }
        }

        public async void EsAdministrador()
        {
            Esadmin = await _loginService.EsAdmin();
        }
        private void _service_AlActualizar2(object? sender, EventArgs e)
        {
            ActualizarActividades(true);
        }

        //private void _service_AlActualizar1(object? sender, EventArgs e)
        //{
        //    MainThread.BeginInvokeOnMainThread(() =>
        //    {
        //        ActualizarActividades(true);
        //    });

        //}


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

        //private  void _service_AlActualizar()
        //{
        //    ActualizarActividades(true);
        //}

        [ObservableProperty]
        public ActividadDTO? actividad = new();
        [ObservableProperty]
        public string error = "";
        [ObservableProperty]
        public Actividad? seleccionado;

        public ObservableCollection<Actividad> Actividades { get; set; } = new();
        public ObservableCollection<ActividadConImagen> ActividadesImg { get; set; } = new();



        void ActualizarActividades(bool? cambios)
        {
            //if (cambios == true)
            //{
            //    var nuevaVista = new PrincipalView();
            //    var currentPage = Shell.Current.Navigation.NavigationStack.LastOrDefault();
            //    await Shell.Current.Navigation.PushAsync(nuevaVista);
            //    //OnPropertyChanged();
            //    //await ChecarSiEsAdmin();
            //}
                Actividades.Clear();
                var y = _repos.GetAll();
                foreach (var l in y)
                {
                    Actividades.Add(l);
                }
            OnPropertyChanged(nameof(Actividades));
            
            
        }
        //async void VistaInicial()
        //{

        //}
        void CambiarVista(string vista)
        {
            Shell.Current.GoToAsync(vista);
        }
        [RelayCommand]
        public void Cancelar()
        {
            //var nuevaVista = new PrincipalView();
            //var currentPage = Shell.Current.Navigation.NavigationStack.LastOrDefault();
            //Shell.Current.Navigation.PushAsync(nuevaVista);
            Shell.Current.Navigation.PopAsync();
            Actividad = null;
            Seleccionado = null;
            Error = "";
            Img = "";
            //ActualizarActividades(true);
           
        }
        [RelayCommand]
        public void VerAgregar()
        {
            Actividad = new();
            Img = "";
        
            var nuevaVista = new AddView(this);
            Shell.Current.Navigation.PushAsync(nuevaVista);


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
                        Actividad.Imagen = Img;
                        Actividad.IdDepartamento = await token;
                        Actividad.Estado = 1;
                        await _service.Add(Actividad);
                        Cancelar();
                        ActualizarActividades(true);
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
        public async Task VerEditar(int id)
        {
            Seleccionado = _repos.Get(id);
            //obtener imagen
            ActividadDTO y = await _service.GetActividad(id)?? new ActividadDTO();
            Img = y.Imagen??"";
            if (Seleccionado != null)
            {
                Error = "";
                var nuevaVista = new UpdateView(this);
                await Shell.Current.Navigation.PushAsync(nuevaVista);


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
                            Id = Seleccionado.Id,
                            Imagen = Img

                        };
                        await _service.Update(y);
                        ActualizarActividades(true);
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
                //CambiarVista("//PrincipalView");
                ActualizarActividades(true);
                Cancelar();
                
            }
            
        }
        [RelayCommand]
        public async Task Logout()
        {
            _loginService = new();
            await _loginService.Logout();
            //LoginService.();
            await App.CerrarHilos();
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
        public async Task<string> GetImagen(int id)
        {
            ActividadDTO x = await _service.GetActividad(id);
            return x != null ? x.Imagen : "";
        }
    }
}
