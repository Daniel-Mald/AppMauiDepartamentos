using AppMauiDepartamentos.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMauiDepartamentos.Models.Entities
{
    public class ActividadConImagen
    {
        public Actividad Actividad { get; set; } = null!;
        public byte[]? Imagen { get; set; }
    }
}
