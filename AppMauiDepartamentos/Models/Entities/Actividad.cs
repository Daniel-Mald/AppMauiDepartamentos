using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMauiDepartamentos.Models.Entities
{
    [SQLite.Table("Actividad")]
    public class Actividad
    {
        [PrimaryKey]
        public int Id { get; set; }
        [NotNull]
        public string Titulo { get; set; } = null!;
        [NotNull]
        public string Descripcion { get; set; } = null!;
        
        public DateTime? FechaRealizacion { get; set; }
        [NotNull]
        public int IdDepartamento { get; set; }




        [NotNull]
        public DateTime FechaCreacion { get; set; }
        [NotNull]
        public DateTime FechaActualizacion { get; set; }
        [NotNull]
        public int Estado { get; set; }
    }
}
