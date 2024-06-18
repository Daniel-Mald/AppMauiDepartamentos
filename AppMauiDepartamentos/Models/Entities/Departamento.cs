using SQLite;
using SQLiteNetExtensions.Attributes;

//using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

//using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMauiDepartamentos.Models.Entities
{
    [SQLite.Table("Departamento")]
    public class Departamento
    {
        [PrimaryKey]
        public int Id { get; set; }
        [NotNull]
        public string Nombre { get; set; } = null!;
        [NotNull]
        public string UserName { get; set; } = null!;

        [SQLiteNetExtensions.Attributes.ForeignKey(typeof(Departamento))]
        public int? SuperiorId { get; set; }
        [OneToMany]
        public Departamento? Superior { get; set; }
    }
}
