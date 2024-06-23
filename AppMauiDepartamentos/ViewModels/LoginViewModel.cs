using AppMauiDepartamentos.Models.DTOs;
using AppMauiDepartamentos.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMauiDepartamentos.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        LoginService _service;
        ActividadService _serviceActividad;
        //ActividadesViewModel _vm = new();

        [ObservableProperty]
        string error = "";
        [ObservableProperty]
        LoginDTO dto;
        public LoginViewModel(ActividadService ss)
        {
            dto = new();
            _service = new() ;
            _service.Logout();
            _serviceActividad = ss;
        }
        [RelayCommand]
        public async Task Login()
        {
            
            bool respuesta = await _service.Login(dto.Username, dto.Password);
            if (respuesta)
            {
                //App._thread.Start();
                LoginService.ReiniciarHilo();
                //await _serviceActividad.GetActividades();
                //if(_vm != null)
                //await _vm.ChecarSiEsAdmin();
                int id = await _service.GetDepartamentoId();
                var idsuperior =await _service.GetIdSuperior();
                bool Admin = false;
                if(id == 1|| idsuperior == "")
                {
                    Admin = true;
                }
                
                await Shell.Current.GoToAsync($"//PrincipalView?EsAdmin={Admin}");
                Dto = new();
            }
            else
            {
                
                Error = "Credenciales incorrectas";
               // Dto = new();
            }
        }
    }
}
