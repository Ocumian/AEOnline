using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AEOnline.Models
{
    [Table("HistorialPosicion")]
    public class HistorialPosicion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }
        [Required]
        public double Latitud { get; set; }
        [Required]
        public double Longitud { get; set; }

        public bool Inicio { get; set; }

        public bool Procesado { get; set; }
    }
}