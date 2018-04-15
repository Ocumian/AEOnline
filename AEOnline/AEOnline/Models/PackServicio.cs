using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.ComponentModel;
using AEOnline.ClasesAdicionales;


namespace AEOnline.Models
{
    [Table("PackServicio")]
    public class PackServicio
    {
        [Key]
        public int Id { get; set; }

        public  string Nombre { get; set; }

        public int NumeroVehiculos { get; set; }
        public int NumeroOperadores { get; set; }

        public bool ModuloCombustible { get; set; }
        public bool ModuloMantencion { get; set; }

        public virtual List<Flota> Flotas { get; set; }

    }
}