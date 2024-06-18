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
    public class DepartamentoEditarValidator:AbstractValidator<DepartamentoDTO>
    {
        Repository<Departamento> _repos;
        public DepartamentoEditarValidator(Repository<Departamento> repos)
        {
            _repos = repos;
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("Debe de escribir un nombre del departamento");
            RuleFor(x => x.Username).NotEmpty().WithMessage("Debe de escribir el username del departamento");
            
        }
        bool SiYaExiste(DepartamentoDTO dto)
        {
            if (_repos.Get(dto.Id) != null)
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
