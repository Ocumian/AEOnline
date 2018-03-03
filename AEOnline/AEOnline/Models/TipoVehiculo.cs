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
    [Table("TipoVehiculo")]
    public class TipoVehiculo
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(25, MinimumLength = 3, ErrorMessage = "El tipo de vehículo debe tener un mínimo de 3 carácteres y máximo de 25")]
        public string Tipo { get; set; }


        public static void CrearTipoVehiculo(ProyectoAutoContext _db, int _idFlota, string _tipo)
        {
            Flota flota = _db.Flotas.Where(f => f.Id == _idFlota).FirstOrDefault();

            TipoVehiculo nuevoTipo = new TipoVehiculo();
            nuevoTipo.Tipo = _tipo;

            flota.TiposVehiculo.Add(nuevoTipo);
            _db.SaveChanges();

        }
    }
}