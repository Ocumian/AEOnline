using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AEOnline.Models.WebModels
{
    public class CreacionAuto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string NombreVehiculo { get; set; }

        [Required, StringLength(25, MinimumLength = 0, ErrorMessage = "El nombre de auto puede tener un máximo de 25 carácteres.")]
        public string Patente { get; set; }
        [Required]
        [StringLength(25, MinimumLength = 0, ErrorMessage = "La marca de auto puede tener un máximo de 25 carácteres.")]
        public string Marca { get; set; }
        [Required]
        [StringLength(25, MinimumLength = 0, ErrorMessage = "El modelo de auto puede tener un máximo de 25 carácteres.")]
        public string Modelo { get; set; }

        public int Year { get; set; }

        public int KilometrajeActual { get; set; }

        public int TipoVehiculoId { get; set; }
        public int OperadorId { get; set; }
        public int FlotaId { get; set; }


    }
}