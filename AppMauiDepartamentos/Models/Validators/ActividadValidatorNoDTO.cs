using AppMauiDepartamentos.Models.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMauiDepartamentos.Models.Validators
{
    public class ActividadValidatorNoDTO: AbstractValidator<Actividad>
    {
        public ActividadValidatorNoDTO()
        {
            RuleFor(x => x.Titulo).NotEmpty().WithMessage("El titulo no debe de estar vacio");
            RuleFor(x => x.Descripcion).NotEmpty().WithMessage("La descripcion no debe de estar vacia");
            RuleFor(x => x.FechaRealizacion).Must(ValidarFechaRealizacion).WithMessage("La fecha no puede ser de hoy o antes de hoy.");
        }
        bool ValidarFechaRealizacion(DateTime? fecha)
        {
            if (fecha <= DateTime.UtcNow)
            {
                return false;
            }
            return true;
        }
    }
}
