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
    [Table("Auto")]
    public class Auto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NombreVehiculo { get; set; }

        [Required, StringLength(25, MinimumLength = 0, ErrorMessage = "El nombre de auto puede tener un máximo de 25 carácteres.")]
        public string Patente { get; set; }

        [StringLength(25, MinimumLength = 0, ErrorMessage = "La marca de auto puede tener un máximo de 25 carácteres.")]
        public string Marca { get; set; }
        [StringLength(25, MinimumLength = 0, ErrorMessage = "El modelo de auto puede tener un máximo de 25 carácteres.")]
        public string Modelo { get; set; }
        public int Year { get; set; }
        public virtual TipoVehiculo TipoVehiculo { get; set; }

        public int KilometrajeActual { get; set; }

        public double Latitud { get; set; }
        public double Longitud { get; set; }

        public virtual Flota Flota { get; set; }
        public virtual GrupoFlota Grupo { get; set; }

        public int? OperadorId { get; set; }
        [ForeignKey("OperadorId")]
        public virtual Operador Operador { get; set; }

        public bool Atajo { get; set; }

        #region HistorialGeneral

        public float LitrosTotalesConsumidos { get; set; }

        public float RendimientoPromedio { get; set; } //Kilometros por litro
        public float CostoKilometroPromedio { get; set; } //Costo por kilometro
        public float CostoLitroPromedio { get; set; } //Costo por litro
        public float CostoTotalCombustible { get; set; }
        public float CostoTotalMantenimiento { get; set; }


        public virtual List<HistorialDiario> HistorialesDiarios { get; set; }
        public virtual List<HistorialIncidente> Incidentes { get; set; }
        public virtual List<HistorialMantencion> Mantenciones { get; set; }
        public virtual List<HistorialCargaCombustible> CargasCombustible { get; set; }

        #endregion


        public static void CrearAuto(ProyectoAutoContext _db, string _nombre, string _patente, TipoVehiculo _tipo, string _marca, string _modelo, int _year, int _kilometraje, int _idFlota, int _idOperador = 0)
        {
            Auto nuevoAuto = new Auto();
            nuevoAuto.NombreVehiculo = _nombre;
            nuevoAuto.Patente = _patente;
            nuevoAuto.TipoVehiculo = _tipo;
            nuevoAuto.Marca = _marca;
            nuevoAuto.Modelo = _modelo;
            nuevoAuto.Year = _year;
            nuevoAuto.KilometrajeActual = _kilometraje;
            nuevoAuto.Latitud = 0;
            nuevoAuto.Longitud = 0;
            _db.Autos.Add(nuevoAuto);


            if (_idFlota != 0)
            {
                Flota flota = _db.Flotas.Where(f => f.Id == _idFlota).FirstOrDefault();
                nuevoAuto.Flota = flota;

                if(_idOperador != 0)
                {
                    Operador operador = flota.Operadores.Where(o => o.Id == _idOperador).FirstOrDefault();
                    nuevoAuto.Operador = operador;
                    //operador.Auto = nuevoAuto;
                    operador.Autos.Add(nuevoAuto);
                }
            }
            _db.SaveChanges();
        }

        public static void EditarAuto(ProyectoAutoContext _db, int _idOriginal, string _nombre, string _patente, TipoVehiculo _tipo, string _marca, string _modelo, int _year, int _kilometraje, int _idFlota, int _idNuevoOperador = 0)
        {
            Auto autoOriginal = _db.Autos.Where(a => a.Id == _idOriginal).FirstOrDefault();

            Flota flota = _db.Flotas.Where(f => f.Id == _idFlota).FirstOrDefault();

            #region Cambiar Flota

            if (_idFlota == 0)
            {
                if (autoOriginal.Flota != null)
                    autoOriginal.Flota.Autos.Remove(autoOriginal);
            }
            else if (flota != null)
            {
                autoOriginal.Flota = flota;
            }

            #endregion

            #region CambiarOperador

            if(_idNuevoOperador == 0)
            {
                if (flota != null && autoOriginal.OperadorId != null)
                    autoOriginal.Operador.Autos.Clear();
                   // autoOriginal.Operador.Auto = null;

                autoOriginal.Operador = null;
            }
            if(flota != null && _idNuevoOperador != 0)
            {
                if (autoOriginal.OperadorId != null)
                    autoOriginal.Operador.Autos.Clear();
                    //autoOriginal.Operador.Auto = null;

                Operador operador = flota.Operadores.Where(o => o.Id == _idNuevoOperador).FirstOrDefault();
                //operador.Auto = autoOriginal;
                operador.Autos.Add(autoOriginal);
                autoOriginal.Operador = operador;

            }

            #endregion

            autoOriginal.Patente = _patente;
            autoOriginal.NombreVehiculo = _nombre;
            autoOriginal.Patente = _patente;
            autoOriginal.TipoVehiculo = _tipo;
            autoOriginal.Marca = _marca;
            autoOriginal.Modelo = _modelo;
            autoOriginal.Year = _year;
            autoOriginal.KilometrajeActual = _kilometraje;
            _db.SaveChanges();


        }

        public static void ElimiarAuto(ProyectoAutoContext _db, int _idAuto)
        {
            Auto auto = _db.Autos.Where(a => a.Id == _idAuto).FirstOrDefault();

            if (auto.OperadorId != null)
                auto.Operador.Autos.Clear();
                //auto.Operador.Auto = null;

            _db.HistorialesCargaCombustible.RemoveRange(auto.CargasCombustible);

            foreach(HistorialMantencion hm in auto.Mantenciones)
            {
                _db.MantencionServicios.RemoveRange(hm.ServiciosAplicados);
            }
            _db.HistorialesMantencion.RemoveRange(auto.Mantenciones);

            foreach(HistorialDiario hd in auto.HistorialesDiarios)
            {
                _db.HistorialesVelocidad.RemoveRange(hd.historialesVelocidad);
                _db.HistorialesPosicion.RemoveRange(hd.historialesPosicion);
            }
            _db.HistorialesDiarios.RemoveRange(auto.HistorialesDiarios);
            _db.Autos.Remove(auto);

            _db.SaveChanges();
        }

    }
}