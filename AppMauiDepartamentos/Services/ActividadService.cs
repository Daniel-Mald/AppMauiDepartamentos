using AppMauiDepartamentos.Models.DTOs;
using AppMauiDepartamentos.Models.Entities;
using AppMauiDepartamentos.Repositories;
//using MetalPerformanceShaders;

//using Foundation;

//using AuthenticationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
//using static UIKit.UIGestureRecognizer;

namespace AppMauiDepartamentos.Services
{
    public class ActividadService
    {
        HttpClient _client;
        Repository<Actividad> _repos = new() ;
        LoginService _loginService = new();
        public event Action? AlActualizar;
        public ActividadService()
        {
            //_repos = repos;
            _client = new()
            {
                //cambiar
                BaseAddress = new Uri("https://localhost:44341/")
            };
        }
        public async Task GetActividades( )
        {
            try
            {
                var _fecha = Preferences.Get("UltimaActualizacion", DateTime.MinValue);
                bool _aviso = false;
                var token = await _loginService.GetToken();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                //cambiar
                var _response = await _client.GetFromJsonAsync<List<ActividadDTO>>($"api/Actividades/departamentos");
                //var r = _response.FirstOrDefault(x => x.Id == 108);
                //if (r != null) { var tr = 3; }
                if (_response != null)
                {
                    int numero = _response.Count;
                    for (int i = 0; i < numero; i++)
                    {
                        var item = _response[i];
                        var _entity = _repos.Get(item.Id);
                        //if (r.Id == item.Id)
                        //{ int t = 2; }
                        if (_entity == null && item.Estado != 2)
                        {
                            _entity = new()
                            {
                                Id = item.Id,
                                FechaActualizacion = item.FechaActualizacion,
                                IdDepartamento = item.IdDepartamento,
                                Descripcion = item.Descripcion??"",
                                Estado = item.Estado,
                                FechaCreacion = item.FechaCreacion,
                                Titulo = item.Titulo,
                                FechaRealizacion = item.FechaRealizacion ?? DateTime.Now
                            };
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
                                        _entity.Descripcion = item.Descripcion;
                                        _entity.FechaActualizacion = item.FechaActualizacion;
                                        _entity.FechaRealizacion = item.FechaRealizacion?? DateTime.Now;
                                        _repos.Update(_entity);
                                        _aviso = true;
                                    }
                                }
                            }
                        }
                    }
                    if (_aviso)
                    {
                        _ = MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            AlActualizar?.Invoke();
                        });
                        Application.Current.Dispatcher.Dispatch(() =>
                        {
                            AlActualizar?.Invoke();

                        });
                        AlActualizar?.Invoke();

                    }
                    Preferences.Set("UltimaActualizacion", _response.Max(x => x.FechaActualizacion));


                
                    //foreach (var item in _response)
                    //{ }
                        
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
                await GetActividades();

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
                await GetActividades();

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
                    await GetActividades();

                }
            }
            

        }
    }
}
