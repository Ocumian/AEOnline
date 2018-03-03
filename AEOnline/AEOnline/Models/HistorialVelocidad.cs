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
    [Table("HistorialVelocidad")]
    public class HistorialVelocidad
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
        public float ValorUnCuarto { get; set; }
        [Required]
        public DateTime HoraUnCuarto { get; set; }

        [Required]
        public float ValorMitad { get; set; }
        [Required]
        public DateTime HoraMitad { get; set; }

        [Required]
        public float ValorTresCuartos { get; set; }
        [Required]
        public DateTime HoraTresCuartos { get; set; }

    }
}