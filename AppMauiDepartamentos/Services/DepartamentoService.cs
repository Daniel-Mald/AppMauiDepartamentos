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
        Repository<Actividad> _reposActividad;
        public  LoginService _loginService;
        ActividadService _actividadService;

        public event Action? AlActualizar;
        public DepartamentoService(ActividadService actividadService, LoginService lg,
            Repository<Departamento> dr, Repository<Actividad> ar)
        {
            _reposActividad = ar;
            _loginService = lg;
            _repos = dr;
            _client = new()
            {
                //cambiar
                BaseAddress = new Uri("https://apiregistroactividades.websitos256.com/")
                //BaseAddress = new Uri("https://localhost:44341")

            };
            _actividadService = actividadService;
            LimpiarDatos();

        }
        public void LimpiarDatos()
        {
            var x = _repos.GetAll();
            if(x!= null)
            {
                foreach (var item in x)
                {
                    _repos.Delete(item);
                }
            }
        }
        public async Task GetDepartamentos()
        {
            if (await _loginService.EsAdmin() == true)
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

                        if (_numLocal != _numRemoto)
                        {
                            if (_numLocal > 0)
                            {
                                var x = _repos.GetAll();
                                foreach (var item in x)
                                {
                                    //aqui hay un problema

                                    _repos.Delete(item);
                                }
                            }

                            foreach (var item in _response)
                            {
                                if (item.Username.EndsWith("@apiequipo10.com"))
                                {
                                    Departamento _entity = new()
                                    {
                                        Nombre = item.Nombre,
                                        SuperiorId = item.IdSuperior,
                                        UserName = item.Username,
                                        Id = item.Id
                                    };
                                    _repos.Insert(_entity);
                                }
                                

                            }
                            _aviso = true;
                        }
                        else
                        {
                            var y = _repos.GetAll();
                            foreach (var item in _response)
                            {
                                var x = y.FirstOrDefault(x => x.Id == item.Id);
                                if (x != null)
                                {
                                    if (item.Nombre != x.Nombre ||
                                    item.Username != x.UserName ||
                                    item.IdSuperior != x.SuperiorId)
                                    {
                                        Departamento _entity = new()
                                        {
                                            Nombre = item.Nombre,
                                            //SuperiorId = item.IdSuperior == 0 ? (int)item.IdSuperior : 0,
                                            SuperiorId = item.IdSuperior,
                                            UserName = item.Username,
                                            Id = item.Id,
                                            Superior = GetDepartamentoLocal(item.IdSuperior) ?? new Departamento()
                                        };

                                        _repos.Update(_entity);
                                        _aviso = true;
                                    }
                                }
                                //else
                                //{
                                //    await Add(item);
                                //}

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
                catch (Exception e)
                {


                }
            }
            
        }
        public async Task<string?> Add(DepartamentoDTO _dto)
        {
            var token = await _loginService.GetToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var _response = await _client.PostAsJsonAsync("api/Departamentos", _dto);
            if (_response.IsSuccessStatusCode)
            {
                await GetDepartamentos();
            }
            else
            {
                var _errores = await _response.Content.ReadAsStringAsync();
                throw new Exception(_errores);
            }
            return _response.StatusCode.ToString();
        }
        public async Task Delete(int _id)
        {
            var token = await _loginService.GetToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //var actividades = _reposActividad.GetAll().Where(x=>x.IdDepartamento == _id&& x.Estado ==1);
            //if(actividades != null)
            //{
            //    foreach (var item in actividades)
            //    {
            //        await _actividadService.Delete(item.Id);
            //    }
            //}

            var _response = await _client.DeleteAsync($"api/Departamentos/{_id}");
            
            if (_response.IsSuccessStatusCode)
            {
                await GetDepartamentos();
            }
        }
        public async Task Update(DepartamentoDTO _dto)
        {
            if(_dto.IdSuperior == _dto.Id)
            {
                _dto.IdSuperior = 0;
            }
            _dto.Password = "";
            _dto.DepartamentoSuperior = "";
            var token = await _loginService.GetToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var _response = await _client.PutAsJsonAsync($"api/Departamentos/{_dto.Id}", _dto);
      
            if (_response.IsSuccessStatusCode)
            {
                await GetDepartamentos();
            }

        }
        public Departamento? GetDepartamentoLocal(int? id)
        {
            if(id!= null)
            {
                var x = _repos.Get((int)id);
                if(x!= null) { return x; }
            }
            return null;
           
        }
        public async Task<DepartamentoDTO?> GetDepartamento(int id)
        {
            var token = await _loginService.GetToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var _response = await _client.GetFromJsonAsync<DepartamentoDTO>($"api/Departamentos/Departamento/{id}");
            if (_response != null)
            {
                return _response;
            }
            return null;
        }
    }
}
