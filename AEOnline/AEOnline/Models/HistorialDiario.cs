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
    [Table("HistorialDiario")]
    public class HistorialDiario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        public virtual List<HistorialVelocidad> historialesVelocidad { get; set; }
        public virtual List<HistorialPosicion> historialesPosicion { get; set; }
        public virtual List<HistorialEnergia> historialesEnergia { get; set; }
    }
}