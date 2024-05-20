using AppMauiDepartamentos.Models.DTOs;
//using Intents;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AppMauiDepartamentos.Services
{
    public class LoginService
    {
        HttpClient _client;
        public LoginService()
        {
            //_repos = repos;
            _client = new()
            {
                //cambiar
                BaseAddress = new Uri("https://localhost:44341/")
            };
        }
        public async Task<bool> Login(string username, string password)
        {
            try
            {
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
                    return true;
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
                return int.Parse(y.Value);

            }
            return 0;
        }
        public async Task<string> GetToken()
        {

          return await SecureStorage.GetAsync("JwtToken") ?? "";
            
        }

        public void Logout()
        {
            
             SecureStorage.Remove("JwtToken");
        }
        public void ReiniciarHilo()
        {
            App.NuevoHilo();
        }
    }
}
