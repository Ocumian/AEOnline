using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AEOnline.Models
{
    [Table("HistorialEnergia")]
    public class HistorialEnergia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime HoraRegistro { get; set; }

        [Required]
        public float ValorInicio { get; set; }
        [Required]
        public DateTime HoraInicio { get; set; }

        [Required]
        public float ValorFinal { get; set; }
        [Required]
        public DateTime HoraFinal { get; set; }

        [Required]
        public float ValorMenor { get; set; }
        [Required]
        public DateTime HoraMenor { get; set; }

        [Required]
        public float ValorMayor { get; set; }
        [Required]
        public DateTime HoraMayor { get; set; }

        [Required]
        public float ValorMitad { get; set; }
        [Required]
        public DateTime HoraMitad { get; set; }

    }
}