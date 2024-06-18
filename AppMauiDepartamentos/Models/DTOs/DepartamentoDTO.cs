using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMauiDepartamentos.Models.DTOs
{
    public class DepartamentoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int? IdSuperior { get; set; } 
        public string? DepartamentoSuperior { get; set; }
        //public DateTime FechaActualizacion { get; set; }
        //public bool Eliminado { get; set; }

    }
}
