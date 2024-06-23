using AppMauiDepartamentos.Models.DTOs;
using AppMauiDepartamentos.Models.Entities;
using AppMauiDepartamentos.Repositories;
//using MetalPerformanceShaders;
using Microsoft.Maui.Dispatching;


//using Foundation;

//using AuthenticationServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
//using Xamarin.Google.Crypto.Tink.Shaded.Protobuf;
//using static UIKit.UIGestureRecognizer;

namespace AppMauiDepartamentos.Services
{
    public class ActividadService
    {
        HttpClient _client;
        Repository<Actividad> _repos;
        LoginService _loginService;
        //public event Action? AlActualizar;
        public event EventHandler? AlActualizar;
        public event EventHandler<List<ImagenConId>>? AlActualizarImagenes;

        public ActividadService(LoginService ls , Repository<Actividad> ar)
        {
            //_repos = repos;
            _loginService = ls;
            _repos = ar;
            _client = new()
            {
                //cambiar
                BaseAddress = new Uri("https://apiregistroactividades.websitos256.com/")
                //BaseAddress = new Uri("https://localhost:44341")

            };
            //_loginService.OnLogin += _loginService_OnLogin;
            //App._loginService.OnLogin += _loginService_OnLogin1;
            LimpiarActividades();
        }
        public List<ImagenConId> listaImagen = new();
     
        public async Task LimpiarActividades()
        {
            try
            {
                //listaImagen.Clear();
                List<Actividad> acts = _repos.GetAll().ToList();
                foreach (var act in acts)
                {
                    _repos.Delete(act);
                }
                //await GetActividades();
            }
            catch (Exception ejeje)
            {
            }
        }
        public async Task GetActividades(bool? nuevaImagen )
        {
            try
            {
                //if (_loginService.NewLoged)
                //{
                //    //checar si jala
                //     await LimpiarActividades();
                //    _loginService.NewLoged = false;
                //}
                var _fecha = Preferences.Get("UltimaActualizacion", DateTime.MinValue);
                bool _aviso = false;
                var token = await _loginService.GetToken();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                int y =await _loginService.GetDepartamentoId();

                //HttpResponseMessage response = await _client.GetAsync("api/Actividades/departamentos");
                //if(response.StatusCode == HttpStatusCode.NotFound)
                //{
                //    await LimpiarActividades();
                //}
                
                var _response = await _client.GetFromJsonAsync<List<ActividadDTO>>($"api/Actividades/departamentos");
                
                if (_response != null)
                {
                    
                    int numero = _response.Count;
                   if (numero < _repos.GetAll().Count())
                    {
                        await LimpiarActividades();
                        _aviso = true;
                        
                    }

                    
                    for (int i = 0; i < numero; i++)
                    {
                        
                        var item = _response[i];
                        var _entity = _repos.Get(item.Id);
                        
                        if (_entity == null && item.Estado == 1)
                        {
                            
                            _entity = new()
                            {
                                Id = item.Id,
                                FechaActualizacion = item.FechaActualizacion,
                                IdDepartamento = item.IdDepartamento,
                                Descripcion = item.Descripcion??"",
                                Estado = item.Estado,
                                FechaCreacion = item.FechaCreacion,
                                Titulo = item.Titulo??"",
                                FechaRealizacion = item.FechaRealizacion ?? DateTime.Now
                            };
                            //listaImagen.Add(item);
                            _repos.Insert(_entity);
                            _aviso = true;
                        }
                        else
                        {
                            if (_entity != null)
                            {
                                if (item.Estado == 2)
                                {
                                    _repos.Delete(_entity);
                                    
                                    _aviso = true;
                                }
                                else
                                {
                                    if (item.Titulo != _entity.Titulo ||
                                        item.Estado != _entity.Estado ||
                                        item.Descripcion != _entity.Descripcion ||
                                        item.FechaRealizacion != _entity.FechaRealizacion)
                                    {
                                        _entity.Titulo = item.Titulo;
                                        _entity.Estado = item.Estado;
                                        _entity.Descripcion = item.Descripcion??"";
                                        _entity.FechaActualizacion = item.FechaActualizacion;
                                        _entity.FechaRealizacion = item.FechaRealizacion;
                                        _repos.Update(_entity);
                                        //listaImagen.Remove(item);
                                        //ActividadDTO s= listaImagen.FirstOrDefault(x => x.Id == item.Id);
                                        
                                        _aviso = true;
                                    }
                                }
                            }
                        }
                    }
                    if (_aviso || nuevaImagen == true)
                    {
                        listaImagen.Clear();
                        //foreach (var item in _response)
                        //{
                        //    ImagenConId s = new()
                        //    {
                        //        Id = item.Id,
                        //        ImagenBase64 = item.Imagen ?? ""
                        //    };
                        //    listaImagen.Add(s);
                        //}
                        for (int i = 0; i < numero; i++)
                        {
                            ImagenConId s = new()
                            {
                                Id = _response[i].Id,
                                ImagenBase64 = _response[i].Imagen ?? ""
                            };
                            listaImagen.Add(s);
                        }
                        
                        //Application.Current.Dispatcher.Dispatch(() =>
                        //{
                        //    AlActualizar?.Invoke(this, EventArgs.Empty);

                        //});
                        //_ = MainThread.InvokeOnMainThreadAsync(() =>
                        //{
                        //    AlActualizar?.Invoke(this, EventArgs.Empty);

                        //});
                        _ = MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            AlActualizarImagenes?.Invoke(this, listaImagen);

                        });
                        //AlActualizar?.Invoke(this, null);


                    }
                    Preferences.Set("UltimaActualizacion", _response.Max(x => x.FechaActualizacion));


                
                    //foreach (var item in _response)
                    //{ }
                        
                }
                else
                {

                }

            }
            catch (Exception ex)
            {

                
            }
        }
        public async Task Add(ActividadDTO _dto)
        {
            var token = await _loginService.GetToken();
            var iddep = await _loginService.GetDepartamentoId();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var _response = await _client.PostAsJsonAsync("api/Actividades", _dto);
            if (_response.IsSuccessStatusCode) 
            {
                //await GetActividades(iddep);
                await GetActividades(false);

            }
            else
            {
                var _errores = await _response.Content.ReadAsStringAsync();
                throw new Exception(_errores);
            }
        }
        public async Task Delete(int _idActividad) 
        {
            var token = await _loginService.GetToken();
            var iddep = await _loginService.GetDepartamentoId();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var _response = await _client.DeleteAsync($"api/Actividades/{_idActividad}");
            if (_response.IsSuccessStatusCode)
            {
                //await GetActividades(iddep);
                await GetActividades(false);

            }
        }
        public async Task Update(ActividadDTO _dto)
        {
            var token = await _loginService.GetToken();
            var iddep = await _loginService.GetDepartamentoId(); //El id de departameto
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if(_dto.IdDepartamento == iddep ) //checo que el dto sea tenga el id del departamento
            {
                var _response = await _client.PutAsJsonAsync($"api/Actividades/{_dto.Id}", _dto);
                if (_response.IsSuccessStatusCode)
                {
                    //await GetActividades(iddep);
                    await GetActividades(true);

                }
            }
            

        }
        public async Task<ActividadDTO?> GetActividad(int id)
        {
            try
            {
                var token = await _loginService.GetToken();
                var iddep = await _loginService.GetDepartamentoId(); //El id de departameto
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var _response = await _client.GetFromJsonAsync<ActividadDTO>($"api/Actividades/{id}");
                if (_response != null)
                {
                    return _response;
                }
                return null;
            }
            catch (Exception c)
            {
                return null;
               
            }
            

        }
        public async Task<IEnumerable<ActividadDTO>?> GetBorradores(int id)
        {
            try
            {
                var token = await _loginService.GetToken();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var _response = await _client.GetFromJsonAsync<IEnumerable<ActividadDTO>>("api/actividades/departamento/borradores");
                if(_response != null)
                {
                    return _response;
                }
                return null;
            }
            catch (Exception a)
            {

                return null;
            }
        }
    }
}
