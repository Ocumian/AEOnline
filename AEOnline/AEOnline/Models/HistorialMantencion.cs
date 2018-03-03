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
    [Table("HistorialMantencion")]
    public class HistorialMantencion
    {
        public enum TipoMantenimiento { correctivo, preventivo }

        [Key]
        public int Id { get; set; }

        public DateTime Fecha { get; set; }
        public virtual Auto Auto { get; set; }
        public int Kilometraje { get; set; }
        public TipoMantenimiento TipoDeMantenimiento { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        public virtual List<MantencionServicio> ServiciosAplicados { get; set; }
        public int Costo { get; set; }



        public static void NuevoRegistroDeServicio(ProyectoAutoContext _db, int _idAuto ,DateTime _fecha, 
            List<Servicio> _servicios, int _kilometraje, TipoMantenimiento _tipoMantenimiento, 
            int _costo, Proveedor _proveedor)
        {
            Auto auto = _db.Autos.Where(a => a.Id == _idAuto).FirstOrDefault();
            auto.CostoTotalMantenimiento += _costo;

            if(_proveedor != null)
            {
                _proveedor.GastoTotalMantenimiento += _costo;
            }

            HistorialMantencion nuevaMantencion = new HistorialMantencion();
            nuevaMantencion.Fecha = _fecha;
            nuevaMantencion.Kilometraje = _kilometraje;
            nuevaMantencion.TipoDeMantenimiento = _tipoMantenimiento;
            nuevaMantencion.Costo = _costo;
            nuevaMantencion.Proveedor = _proveedor;
            nuevaMantencion.ServiciosAplicados = new List<MantencionServicio>();

            for (int i = 0; i < _servicios.Count; i++)
            {
                MantencionServicio nuevoMantencionServ = new MantencionServicio();
                nuevoMantencionServ.HistorialMantencion = nuevaMantencion;
                nuevoMantencionServ.Servicio = _servicios[i];

                nuevaMantencion.ServiciosAplicados.Add(nuevoMantencionServ);
            }

            auto.Mantenciones.Add(nuevaMantencion);
            _db.SaveChanges();

        }

        public static void EditarHistorialMantencion(ProyectoAutoContext _db, int _idAuto, int _idHistorialOriginal,
            DateTime _fecha, List<Servicio> _servicios, int _kilometraje, TipoMantenimiento _tipoMantenimiento,
            int _costo, Proveedor _proveedor)
        {
            //editar datos proveedor
            //tambien considerar lo que pasa con las relaciones MantencionServicio cuando se remueven/reemplazan

            Auto auto = _db.Autos.Where(a => a.Id == _idAuto).FirstOrDefault();
            HistorialMantencion histOriginal = auto.Mantenciones.Where(h => h.Id == _idHistorialOriginal).FirstOrDefault();

            auto.CostoTotalMantenimiento += _costo - histOriginal.Costo;

            #region Actualizar viejo y nuevo Proveedor

            Proveedor provAnterior = histOriginal.Proveedor;
            Proveedor provNuevo = _proveedor;

            if (provAnterior != null)
                provAnterior.GastoTotalMantenimiento -= histOriginal.Costo;
            if (provNuevo != null)
                provNuevo.GastoTotalMantenimiento += _costo;

            #endregion

            _db.MantencionServicios.RemoveRange(histOriginal.ServiciosAplicados);
            for (int i = 0; i < _servicios.Count; i++)
            {
                MantencionServicio nuevoMantencionServ = new MantencionServicio();
                nuevoMantencionServ.HistorialMantencion = histOriginal;
                nuevoMantencionServ.Servicio = _servicios[i];

                histOriginal.ServiciosAplicados.Add(nuevoMantencionServ);
            }

            histOriginal.Fecha = _fecha;
            histOriginal.Kilometraje = _kilometraje;
            histOriginal.TipoDeMantenimiento = _tipoMantenimiento;
            histOriginal.Costo = _costo;
            histOriginal.Proveedor = _proveedor;

            _db.SaveChanges();
        }

        public static void EliminarHistorialMantencion(ProyectoAutoContext _db, int _idAuto, int _idHistorialOriginal)
        {
            //editar datos proveedor
            //tambien considerar lo que pasa con las relaciones MantencionServicio al removerlas

            Auto auto = _db.Autos.Where(a => a.Id == _idAuto).FirstOrDefault();
            HistorialMantencion histOriginal = auto.Mantenciones.Where(h => h.Id == _idHistorialOriginal).FirstOrDefault();

            auto.CostoTotalMantenimiento -= histOriginal.Costo;

            #region Actualizar viejo y nuevo Proveedor

            Proveedor provAnterior = histOriginal.Proveedor;

            if (provAnterior != null)
                provAnterior.GastoTotalMantenimiento -= histOriginal.Costo;
            #endregion

            _db.MantencionServicios.RemoveRange(histOriginal.ServiciosAplicados);

            _db.HistorialesMantencion.Remove(histOriginal);
            _db.SaveChanges();
        }
    }
}