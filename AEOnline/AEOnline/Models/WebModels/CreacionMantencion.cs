using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace AEOnline.Models.WebModels
{
    public class CreacionMantencion
    {

        [Key]
        public int Id { get; set; }

        public DateTime Fecha { get; set; }
        public int AutoId { get; set; }
        public int Kilometraje { get; set; }
        public HistorialMantencion.TipoMantenimiento TipoDeMantenimiento { get; set; }
        public int ProveedorId { get; set; }
        public List<int> ServiciosId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "El costo debe ser mayor a 0.")]
        public int Costo { get; set; }
    }
}