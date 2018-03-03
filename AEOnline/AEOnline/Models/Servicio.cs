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
    [Table("Servicio")]
    public class Servicio
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(25, MinimumLength = 3, ErrorMessage = "El servicio debe tener un mínimo de 3 carácteres y máximo de 25")]
        public string NombreServicio { get; set; }


        public static void CrearServicio(ProyectoAutoContext _db, int _idFlota, string _nombreServicio)
        {
            Flota flota = _db.Flotas.Where(f => f.Id == _idFlota).FirstOrDefault();

            Servicio nuevoServicio = new Servicio();
            nuevoServicio.NombreServicio = _nombreServicio;

            flota.Servicios.Add(nuevoServicio);
            _db.SaveChanges();

        }

        public static void EliminarServicio(ProyectoAutoContext _db, int _idFlota, int _idServicio)
        {
            Flota flota = _db.Flotas.Where(f => f.Id == _idFlota).FirstOrDefault();
            Servicio serv = flota.Servicios.Where(s => s.Id == _idServicio).FirstOrDefault();

            List<MantencionServicio> ManSerEliminables = new List<MantencionServicio>();

            foreach(Auto a in flota.Autos)
            {
                foreach(HistorialMantencion hm in a.Mantenciones)
                {
                    foreach(MantencionServicio ms in hm.ServiciosAplicados)
                    {
                        if (ms.ServicioId == _idServicio)
                            ManSerEliminables.Add(ms);
                    }
                }
            }
            _db.MantencionServicios.RemoveRange(ManSerEliminables);
            _db.Servicios.Remove(serv);
            _db.SaveChanges();

        }
    }
}