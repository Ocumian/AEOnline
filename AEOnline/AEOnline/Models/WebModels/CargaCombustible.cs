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
    public class CargaCombustible
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        public DateTime Hora { get; set; }

        public bool EstanqueLleno { get; set; }

        [Range(1, float.MaxValue, ErrorMessage = "La cantidad de litros debe ser mayor a 0.")]
        public float CantidadLitros { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "El costo debe ser mayor que 0.")]
        public int CostoUnitario { get; set; }
        public int CostoTotal { get; set; }
        [Required]
        public int Kilometraje { get; set; }
        [Required]
        public string RutEstacion { get; set; }
        [Required]
        [Range(1,int.MaxValue, ErrorMessage = "Debe ingresar un número de boleta")]
        public int NumeroDeBoleta { get; set; }


        public int ProveedorId { get; set; }
        public int VehiculoId { get; set; }
        public string VehículoNombre { get; set; }

    }
}