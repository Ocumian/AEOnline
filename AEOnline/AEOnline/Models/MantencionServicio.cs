using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace AEOnline.Models
{
    [Table("MantencionServicio")]
    public class MantencionServicio
    {
        [Key]
        public int Id { get; set; }

        public int? HistorialMantencionId { get; set; }
        [ForeignKey("HistorialMantencionId")]
        public virtual HistorialMantencion HistorialMantencion { get; set; }

        public int? ServicioId { get; set; }
        [ForeignKey("ServicioId")]
        public virtual Servicio Servicio { get; set; }

    }
}