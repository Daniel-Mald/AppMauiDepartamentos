using AppMauiDepartamentos.Models.DTOs;
using AppMauiDepartamentos.Models.Entities;
using AppMauiDepartamentos.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AppMauiDepartamentos.Services
{
    public class DepartamentoService
    {
        HttpClient _client;
        Repository<Departamento> _repos;
        LoginService _loginService = new();

        public event Action? AlActualizar;
        public DepartamentoService()
        {
            _repos = new();
            _client = new()
            {
                //cambiar
                BaseAddress = new Uri("https://localhost:44341/")
            };
        }
        public async Task GetDepartamentos()
        {
            try
            {
               // var _fecha = Preferences.Get("UltimaActualizacion", DateTime.MinValue);
                bool _aviso = false;
                //cambiar
                var token = await _loginService.GetToken();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var _response = await _client.GetFromJsonAsync<List<DepartamentoDTO>>($"api/Departamentos");
                if (_response != null)
                {
                    int _numLocal = _repos.GetAll().Count();
                    int _numRemoto = _response.Count;

                    if(_numLocal != _numRemoto)
                    {
                        if( _numLocal > 0)
                        {
                            foreach (var item in _response)
                            {
                                var x = _repos.Get(item.Id);
                                _repos.Delete(x);
                            }
                        }
                        
                        foreach (var item in _response)
                        {
                            Departamento _entity = new()
                            {
                                Nombre = item.Nombre,
                                SuperiorId = item.Id,
                                UserName = item.UserName,
                                Id = item.Id
                            };
                            _repos.Insert(_entity);
                            
                        }
                        _aviso = true;
                    }
                    else
                    {
                        foreach(var item in _response)
                        {
                            var x = _repos.Get(item.Id);
                            if(item.Nombre != x.Nombre||
                                item.UserName != x.UserName||
                                item.SuperiorId != x.SuperiorId)
                            {
                                Departamento _entity = new()
                                {
                                    Nombre = item.Nombre,
                                    SuperiorId = item.Id,
                                    UserName = item.UserName,
                                    Id = item.Id
                                };
                                _repos.Update(_entity);
                                _aviso = true;
                            }
                        }
                    }


                    
                        
                    
                    if (_aviso)
                    {
                        _ = MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            AlActualizar?.Invoke();
                        });
                    }
                    //Preferences.Set("UltimaActualizacion", _response.Max(x => x.FechaActualizacion));
                }

            }
            catch (Exception)
            {


            }
        }
        public async Task Add(Departamento _dto)
        {
            var _response = await _client.PostAsJsonAsync("api/Departamento", _dto);
            if (_response.IsSuccessStatusCode)
            {
                await GetDepartamentos();
            }
            else
            {
                var _errores = await _response.Content.ReadAsStringAsync();
                throw new Exception(_errores);
            }
        }
        public async Task Delete(int _id)
        {
            var _response = await _client.DeleteAsync($"api/Departamento/{_id}");
            if (_response.IsSuccessStatusCode)
            {
                await GetDepartamentos();
            }
        }
        public async Task Update(DepartamentoDTO _dto)
        {
            var _response = await _client.PutAsJsonAsync($"api/Departamento", _dto);
            if (_response.IsSuccessStatusCode)
            {
                await GetDepartamentos();
            }

        }
    }
}
