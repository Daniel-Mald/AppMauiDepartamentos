using AppMauiDepartamentos.Models.DTOs;
using AppMauiDepartamentos.Models.Entities;
using AppMauiDepartamentos.Models.Validators;
using AppMauiDepartamentos.Repositories;
using AppMauiDepartamentos.Services;
using AppMauiDepartamentos.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using GoogleGson;

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
        public bool esBorrador = false;
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
            //App._service.AlActualizar += _service_AlActualizar2;
           App._service.AlActualizarImagenes += _service_AlActualizarImagenes;
            //_service.AlActualizar += _service_AlActualizar1;
           // _service.AlActualizar += _service_AlActualizar1;
            ActualizarActividades(false);
        }

        private void _service_AlActualizarImagenes(object? sender, List<ImagenConId> e)
        {
            //Imagenes.Clear();
            Imagenes = e;
            ActualizarActividades(true);
        }
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
            //Imagenes.Clear();
            //Imagenes = e.Eq
            //ActualizarActividades(true);
        }

        [ObservableProperty]
        public ActividadDTO? actividad = new();
        [ObservableProperty]
        public string error = "";
        [ObservableProperty]
        public Actividad? seleccionado;

        public ObservableCollection<Actividad> Actividades { get; set; } = new();
        public ObservableCollection<ActividadConImagen> ActividadesImg { get; set; } = new();
        public ObservableCollection<ActividadConImagen> Borradores { get; set; }=new();
        List<ImagenConId> Imagenes = new();



        void ActualizarActividades(bool? cambios)
        {

            //    Actividades.Clear();
                var y = _repos.GetAll().ToArray();
            //    foreach (var l in y)
            //    {
            //        Actividades.Add(l);
            //    }
            //OnPropertyChanged(nameof(Actividades));
            var imgs = Imagenes.OrderBy(x => x.Id).ToArray();
            ActividadesImg.Clear();
            for (int i = 0; i < y.Count(); i++)
            {
                //Image n = new()
                //{
                //    Source = (!string.IsNullOrWhiteSpace(Imagenes[i].ImagenBase64)) ? ConvertirImagen(Imagenes[i].ImagenBase64) : ImageSource.FromFile("default_image.png")
                //};
                //Image r = "..\Resources\defaultImage.png";
                ActividadConImagen x = new()
                {
                    Actividad = y[i],
                    //Imagen = (byte[])Convert.FromBase64String(imgs[i].ImagenBase64)
                    Imagen = (byte[])Convert.FromBase64String(imgs.FirstOrDefault(x => x.Id == y[i].Id).ImagenBase64) ?? null
                };
                ActividadesImg.Add(x);
            }
            OnPropertyChanged(nameof(ActividadesImg));

        }

        void CambiarVista(string vista)
        {
            Shell.Current.GoToAsync(vista);
        }
        [RelayCommand]
        public void Cancelar()
        {
            
            Shell.Current.Navigation.PopAsync();
            Actividad = null;
            Seleccionado = null;
            EsBorrador = false;
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
        public async Task Agregar(bool borrador)
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

                        Actividad.Estado = !borrador ? 1 : 0 ;
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
        List<ImagenConId> BorradoresImagenes = new();
        [RelayCommand]
        public async Task VerEditar(int id)
        {
            Seleccionado = _repos.Get(id);
            if(Seleccionado == null)
            {
                Seleccionado = (Borradores.FirstOrDefault(x => x.Actividad.Id == id)).Actividad;
                //var imgse = (Borradores.FirstOrDefault(x => x.Actividad.Id == id)).Imagen;
                Img = BorradoresImagenes.FirstOrDefault(x => x.Id == id).ImagenBase64??"";
                Error = "";
                var nuevaVista = new UpdateView(this);
                await Shell.Current.Navigation.PushAsync(nuevaVista);
                EsBorrador = true;
            }           
            else if (Seleccionado != null)
            {
                ImagenConId y = Imagenes.FirstOrDefault(x => x.Id == id) ?? new();
                Img = y.ImagenBase64 ?? "";
                Error = "";
                var nuevaVista = new UpdateView(this);
                await Shell.Current.Navigation.PushAsync(nuevaVista);


            }
        }
        [RelayCommand]
        public async Task VerBorradores()
        {
            int id = await  _loginService.GetDepartamentoId();
            var  borr =  await _service.GetBorradores(id);
            if(borr!=null)
            {
                Borradores.Clear();
                BorradoresImagenes.Clear();
                foreach (var item in borr)
                {
                    ImagenConId sss = new()
                    {
                        Id = item.Id,
                        ImagenBase64 = item.Imagen
                    };
                    BorradoresImagenes.Add(sss);
                    Actividad y = new()
                    {
                        Id = item.Id,
                        Descripcion = item.Descripcion,
                        IdDepartamento = item.IdDepartamento,
                        Estado = item.Estado,
                        FechaActualizacion = item.FechaActualizacion,
                        FechaCreacion = item.FechaCreacion,
                        Titulo = item.Titulo, FechaRealizacion = item.FechaRealizacion
                    };
                    ActividadConImagen x = new()
                    {
                        Actividad = y,
                        Imagen = (byte[])Convert.FromBase64String(item.Imagen??"")
                    };
                    Borradores.Add(x);
                }
               
            }
            var nuevaVista = new MisBorradoresView(this);
            await Shell.Current.Navigation.PushAsync(nuevaVista);
        }

        public async Task ResetBorradores()
        {
            int n = await _loginService.GetDepartamentoId();
            Borradores.Clear();
            BorradoresImagenes.Clear();
            var borr = await _service.GetBorradores(n);
            if(borr != null)
            {
                
                foreach (var item in borr)
                {
                    ImagenConId sss = new()
                    {
                        Id = item.Id,
                        ImagenBase64 = item.Imagen
                    };
                    BorradoresImagenes.Add(sss);
                    Actividad y = new()
                    {
                        Id = item.Id,
                        Descripcion = item.Descripcion,
                        IdDepartamento = item.IdDepartamento,
                        Estado = item.Estado,
                        FechaActualizacion = item.FechaActualizacion,
                        FechaCreacion = item.FechaCreacion,
                        Titulo = item.Titulo,
                        FechaRealizacion = item.FechaRealizacion
                    };
                    ActividadConImagen x = new()
                    {
                        Actividad = y,
                        Imagen = (byte[])Convert.FromBase64String(item.Imagen ?? "")
                    };
                    Borradores.Add(x);
                }
            
            }

        }

        [RelayCommand]
        public async Task Editar(bool publicar)
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

                            Estado = (!publicar&&!EsBorrador
                            ||publicar&&EsBorrador)?1:0,
                            Id = Seleccionado.Id,
                            Imagen = Img

                        };
                        await _service.Update(y);
                        if(EsBorrador)
                        {
                            await ResetBorradores();
                        }
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
        public async Task VerEliminar(int id)
        {
            Seleccionado = _repos.Get(id);
            if(Seleccionado != null)
            {
                //CambiarVista("//DeleteView");
                var nuevaVista = new DeleteView(this);
                await Shell.Current.Navigation.PushAsync(nuevaVista);

            }
            else
            {
                Seleccionado = Borradores.FirstOrDefault(x => x.Actividad.Id == id).Actividad;
                EsBorrador = true;
                var nuevaVista = new DeleteView(this);
                await Shell.Current.Navigation.PushAsync(nuevaVista);
            }
        }
        [RelayCommand]
        public async Task Eliminar()
        {
            if(Seleccionado!= null)
            {
                
                await _service.Delete(Seleccionado.Id);
                if (EsBorrador)
                {
                    await ResetBorradores();
                }
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
