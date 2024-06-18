using AppMauiDepartamentos.Models.DTOs;
//using Intents;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AppMauiDepartamentos.Services
{
    public class LoginService
    {
        HttpClient _client;
        public bool Administrador { get; set; } = false;
        public bool NewLoged { get; set; } = false;
        public event EventHandler? OnLogin;

        public LoginService()
        {
            //_repos = repos;
            _client = new()
            {
                //cambiar
                BaseAddress = new Uri("https://apiregistroactividades.websitos256.com/")
               // BaseAddress = new Uri("https://localhost:44341")

            };
        }
        public async Task<bool> Login(string username, string password)
        {
            try
            {
                App.CerrarHilos();
                var dto = new LoginDTO()
                {
                    Password = password,
                    Username = username
                };
                var response = await _client.PostAsJsonAsync($"api/Login", dto);
                string _token = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    await SecureStorage.SetAsync("JwtToken", _token);
                    NewLoged = true;
                    Administrador = await EsAdmin();
                    
                       OnLogin?.Invoke(this, EventArgs.Empty);
                    
                        

                    return true;
                }
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    await Login(username, password);
                }
                return false;
            }
            catch (Exception ex)
            {   

                return false;
            }
        }
        public async Task<int> GetDepartamentoId()
        {
            
            var token =  await SecureStorage.GetAsync("JwtToken")??"";
            var handler = new JwtSecurityTokenHandler();
            if(!string.IsNullOrWhiteSpace(token))
            {
                var x = handler.ReadJwtToken(token);
                var y =  x.Claims.FirstOrDefault(x=>x.Type == "IdDepartamento");
                if (y != null)
                return int.Parse(y.Value);

            }
            return 0;
        }
        public async Task<string> GetIdSuperior()
        {
            var token = await SecureStorage.GetAsync("JwtToken") ?? "";
            var handler = new JwtSecurityTokenHandler();
            if(!string.IsNullOrWhiteSpace(token))
            {
                var han = handler.ReadJwtToken(token);
                var valor = han.Claims.FirstOrDefault(x => x.Type == "IdSuperior");
                return valor.Value;
            }
            return "No jala el token";
        }
        public async Task<string> GetToken()
        {

          return await SecureStorage.GetAsync("JwtToken") ?? "";
            
        }

        public async Task Logout()
        {
            
             SecureStorage.Remove("JwtToken");
            
            await App.CerrarHilos();
           // App.Current
        }
        public async static void ReiniciarHilo()
        {
            await App.NuevoHilo();
        }
        public async Task<bool> EsAdmin()
        {
            string id = await GetIdSuperior();
            if(id == "0" || id == "")
            {
                return true;
            }
            return false;
        }
    }
}
