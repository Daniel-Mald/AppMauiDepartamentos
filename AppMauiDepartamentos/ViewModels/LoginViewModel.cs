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
        public LoginViewModel()
        {
            dto = new();
            _service = new() ;
            _service.Logout();
            _serviceActividad = new ActividadService();
        }
        [RelayCommand]
        public async Task Login()
        {
            
            bool respuesta = await _service.Login(dto.Username, dto.Password);
            if (respuesta)
            {
                //App._thread.Start();
                _service.ReiniciarHilo();
                await _serviceActividad.GetActividades();
                //if(_vm != null)
                //await _vm.ChecarSiEsAdmin();
                int id = await _service.GetDepartamentoId();
                bool Admin = false;
                if(id == 1)
                {
                    Admin = true;
                }
                
                await Shell.Current.GoToAsync($"//PrincipalView?EsAdmin={Admin}");
            }
            else
            {
                
                Error = "Credenciales incorrectas";
            }
        }
    }
}
