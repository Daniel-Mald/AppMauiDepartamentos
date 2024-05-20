using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMauiDepartamentos.Models.DTOs
{
    public class ActividadDTO
    {
        public int Id { get; set; }
       
        public string Titulo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public DateTime? FechaRealizacion { get; set; }
        public int IdDepartamento { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public int Estado { get; set; }
        public string Imagen { get; set; } = null!;

    }
}
