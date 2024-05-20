using AppMauiDepartamentos.Models.DTOs;
using AppMauiDepartamentos.Models.Entities;
using AppMauiDepartamentos.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMauiDepartamentos.Models.Validators
{
    public class DepartamentoDTOValidator:AbstractValidator<DepartamentoDTO>
    {
        Repository<Departamento> _repos;
        public DepartamentoDTOValidator(Repository<Departamento> repos)
        {
            _repos = repos;
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("Debe de escribir un nombre del departamento");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Debe de escribir el username del departamento");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Debe de escribir una contraseña").MinimumLength(6)
                .WithMessage("La contraseña debe de medir almenos 6 caracteres");
            RuleFor(x => x).Must(SiYaExiste).WithMessage("Ya existe dicho departamento");
           // RuleFor(x => x.SuperiorId).NotEqual(0).WithMessage("Debe seleccionar un ");
        }
        bool SiYaExiste(DepartamentoDTO dto)
        {
            if(_repos.Get(dto.Id) != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
