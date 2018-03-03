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
    [Table("Proveedor")]
    public class Proveedor
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string NombreComercial { get; set; }

        public string Telefono { get; set; }

        public string Direccion { get; set; }


        public string PersonaContacto { get; set; }

        public string TelefonoContacto { get; set; }

        public string EmailContacto { get; set; }


        public int GastoTotalCombustible { get; set; }
        public int GastoTotalMantenimiento { get; set; }


        public static void CrearProveedor(ProyectoAutoContext _db, int _idFlota,
            string _nombreComercial, string _telefono, string _direccion, string _personaContacto,
            string _telefonoContacto, string _emailContacto)
        {
            Flota flota = _db.Flotas.Where(f => f.Id == _idFlota).FirstOrDefault();

            Proveedor nuevoProveedor = new Proveedor();
            nuevoProveedor.NombreComercial = _nombreComercial;
            nuevoProveedor.Telefono = _telefono;
            nuevoProveedor.Direccion = _direccion;
            nuevoProveedor.PersonaContacto = _personaContacto;
            nuevoProveedor.TelefonoContacto = _telefonoContacto;
            nuevoProveedor.EmailContacto = _emailContacto;
            nuevoProveedor.GastoTotalCombustible = 0;
            nuevoProveedor.GastoTotalMantenimiento = 0;

            flota.Proveedores.Add(nuevoProveedor);
            _db.SaveChanges();
        }

        public static void EditarProveedor(ProyectoAutoContext _db, int _idOriginal,
            string _nuevoNombre, string _nuevoTelefono, string _nuevaDireccion, string _nuevaPersonaContacto,
            string _nuevoTelefonoContacto, string _nuevoEmailContacto)
        {
            Proveedor proveedor = _db.Proveedores.Where(p => p.Id == _idOriginal).FirstOrDefault();

            proveedor.NombreComercial = _nuevoNombre;
            proveedor.Telefono = _nuevoTelefono;
            proveedor.Direccion = _nuevaDireccion;
            proveedor.PersonaContacto = _nuevaPersonaContacto;
            proveedor.TelefonoContacto = _nuevoTelefonoContacto;
            proveedor.EmailContacto = _nuevoEmailContacto;

            _db.SaveChanges();

        }

        public static void EliminarProveedor(ProyectoAutoContext _db, int _idFlota, int _idProveedor)
        {
            Flota flota = _db.Flotas.Where(f => f.Id == _idFlota).FirstOrDefault();
            Proveedor proveedor = flota.Proveedores.Where(p => p.Id == _idProveedor).FirstOrDefault();


            foreach(Auto auto in flota.Autos)
            {
                foreach(HistorialMantencion hm in auto.Mantenciones)
                {
                    if (hm.Proveedor == proveedor)
                        hm.Proveedor = null;
                }

                foreach(HistorialCargaCombustible hcc in auto.CargasCombustible)
                {
                    if (hcc.Proveedor == proveedor)
                        hcc.Proveedor = null;
                }

            }
            _db.Proveedores.Remove(proveedor);
            _db.SaveChanges();

        }

    }
}