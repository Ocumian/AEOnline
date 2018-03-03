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
    [Table("HistorialCargaCombustible")]
    public class HistorialCargaCombustible
    {
        [Key]
        public int Id { get; set; }

        public virtual Auto Auto { get; set; }

        public DateTime FechaHora { get; set; }
        public bool EstanqueLleno { get; set; }

        public float CantidadLitros { get; set; }
        public int CostoUnitario { get; set; }
        public int CostoTotal { get; set; }
        public int Kilometraje { get; set; }
        public virtual Proveedor Proveedor { get; set; }


        //Mostrables en la tabla
        public float KilometrosRecorridos { get; set; }
        public float CostoPorKilometro { get; set; }
        public float Rendimiento { get; set; } //Kilometros por litro


        public static void NuevaCargaCombustible(ProyectoAutoContext _db, int _idAuto, 
            DateTime _fecha, DateTime _hora, bool _estanqueLleno, 
            float _cantidadLitros, int _costoUnitario, int _Kilometraje, Proveedor _proveedor)
        {
            Auto auto = _db.Autos.Where(a => a.Id == _idAuto).FirstOrDefault();
            List<HistorialCargaCombustible> otrasCargas = auto.CargasCombustible.OrderBy(h => h.FechaHora).ToList();
            int costoTotal = Convert.ToInt32(_costoUnitario * _cantidadLitros);

            #region Actualizar datos del auto


            if (otrasCargas.Count > 0)
            {
                int primerKilometraje = otrasCargas.First().Kilometraje;
                int ultimoKilometraje = _Kilometraje;
                int kilometrosRecorrido = ultimoKilometraje - primerKilometraje;

                auto.RendimientoPromedio = kilometrosRecorrido / auto.LitrosTotalesConsumidos; //Kilometros por litro
                auto.CostoKilometroPromedio = auto.CostoTotalCombustible/ kilometrosRecorrido; //Costo por kilometro
                auto.CostoLitroPromedio = auto.CostoTotalCombustible/ auto.LitrosTotalesConsumidos; //Costo por litro
            }

            auto.KilometrajeActual = _Kilometraje;
            auto.LitrosTotalesConsumidos += _cantidadLitros;
            auto.CostoTotalCombustible += costoTotal;
            #endregion

            #region Actualizar Proveedor
            if (_proveedor != null)
            {
                _proveedor.GastoTotalCombustible += costoTotal;
            }
            #endregion

            float kilometrosRecorridos = -1;
            float costoPorKilometro = -1;
            float rendimiento = -1;


            if (otrasCargas.Count > 0)
            {
                HistorialCargaCombustible ultimaCarga = otrasCargas.Last();

                kilometrosRecorridos = _Kilometraje - ultimaCarga.Kilometraje;
                costoPorKilometro = ultimaCarga.CostoTotal / kilometrosRecorridos;
                rendimiento = kilometrosRecorridos / ultimaCarga.CantidadLitros;
            }                

            DateTime fecha = new DateTime(_fecha.Year, _fecha.Month, _fecha.Day, _hora.Hour, _hora.Minute, _hora.Second);
           
            HistorialCargaCombustible nuevaCarga = new HistorialCargaCombustible();
            nuevaCarga.FechaHora = fecha;
            nuevaCarga.EstanqueLleno = _estanqueLleno;
            nuevaCarga.CantidadLitros = _cantidadLitros;
            nuevaCarga.CostoUnitario = _costoUnitario;
            nuevaCarga.Kilometraje = _Kilometraje;
            nuevaCarga.CostoTotal = costoTotal;
            nuevaCarga.Proveedor = _proveedor;

            nuevaCarga.KilometrosRecorridos = kilometrosRecorridos;
            nuevaCarga.CostoPorKilometro = costoPorKilometro;
            nuevaCarga.Rendimiento = rendimiento;

            auto.CargasCombustible.Add(nuevaCarga);


            _db.SaveChanges();

        }

        public static void EditarCargaCombustible(ProyectoAutoContext _db, int _idAuto, int _idCargaOriginal,
            DateTime _fecha, DateTime _hora, bool _estanqueLleno,
            float _cantidadLitros, int _costoUnitario, int _Kilometraje, Proveedor _proveedor)
        {
            //considerar edicion de historial general del auto y el proveedor
            // editar tambien kilometros recorridos, costo por kilometro, rendimiento DE ESTE Y EL SIGUIENTE HISTORIAL


            Auto auto = _db.Autos.Where(a => a.Id == _idAuto).FirstOrDefault();
            HistorialCargaCombustible cargaOriginal = auto.CargasCombustible.Where(hcc => hcc.Id == _idCargaOriginal).FirstOrDefault();
            List < HistorialCargaCombustible> otrasCargas = auto.CargasCombustible.OrderBy(h => h.FechaHora).ToList();
            int costoTotal = Convert.ToInt32(_costoUnitario * _cantidadLitros);

            HistorialCargaCombustible cargaAnterior = null;
            HistorialCargaCombustible cargaSiguiente = null;
            for (int i = 0; i < otrasCargas.Count; i++)
            {
                if(otrasCargas[i].Id == cargaOriginal.Id)
                {
                    if ((i + 1) < otrasCargas.Count)
                        cargaSiguiente = otrasCargas[i + 1];
                    if ((i - 1) >= 0)
                        cargaAnterior = otrasCargas[i - 1];
                }
            }

            //Datos de auto que deberian cambiar de inmediato
            auto.LitrosTotalesConsumidos += _cantidadLitros - cargaOriginal.CantidadLitros;
            auto.CostoTotalCombustible += costoTotal - cargaOriginal.CostoTotal;

            #region Actualizar viejo y nuevo Proveedor
            Proveedor provAnterior = cargaOriginal.Proveedor;
            Proveedor provNuevo = _proveedor;

            if (provAnterior != null)
                provAnterior.GastoTotalCombustible -= cargaOriginal.CostoTotal;
            if (provNuevo != null)
                provNuevo.GastoTotalCombustible += costoTotal;

            #endregion

            #region Actualizar Datos de la carga original
            float kilometrosRecorridos = -1;
            float costoPorKilometro = -1;
            float rendimiento = -1;

            if (cargaAnterior != null)
            {

                kilometrosRecorridos = _Kilometraje - cargaAnterior.Kilometraje;
                costoPorKilometro = cargaAnterior.CostoTotal / kilometrosRecorridos;
                rendimiento = kilometrosRecorridos / cargaAnterior.CantidadLitros;
            }

            DateTime fecha = new DateTime(_fecha.Year, _fecha.Month, _fecha.Day, _hora.Hour, _hora.Minute, _hora.Second);
            cargaOriginal.FechaHora = fecha;
            cargaOriginal.EstanqueLleno = _estanqueLleno;
            cargaOriginal.CantidadLitros = _cantidadLitros;
            cargaOriginal.CostoUnitario = _costoUnitario;
            cargaOriginal.Kilometraje = _Kilometraje;
            cargaOriginal.CostoTotal = costoTotal;
            cargaOriginal.Proveedor = _proveedor;

            cargaOriginal.KilometrosRecorridos = kilometrosRecorridos;
            cargaOriginal.CostoPorKilometro = costoPorKilometro;
            cargaOriginal.Rendimiento = rendimiento;

            _db.SaveChanges();
            #endregion

            #region Actualizar Datos de la siguiente carga

            if(cargaSiguiente != null)
            {
                kilometrosRecorridos = cargaSiguiente.Kilometraje - cargaOriginal.Kilometraje;
                costoPorKilometro = cargaOriginal.CostoTotal / kilometrosRecorridos;
                rendimiento = kilometrosRecorridos / cargaOriginal.CantidadLitros;

                cargaSiguiente.KilometrosRecorridos = kilometrosRecorridos;
                cargaSiguiente.CostoPorKilometro = costoPorKilometro;
                cargaSiguiente.Rendimiento = rendimiento;
            }

            #endregion

            //datos del auto se actualizan al final cuando los hitoriales ya se regulan
            #region Actualizar datos del auto

            if (otrasCargas.Count > 1)
            {
                int primerKilometraje = otrasCargas.First().Kilometraje;
                int ultimoKilometraje = otrasCargas.Last().Kilometraje;
                int kilometrosRecorrido = ultimoKilometraje - primerKilometraje;

                auto.RendimientoPromedio = kilometrosRecorrido / auto.LitrosTotalesConsumidos; //Kilometros por litro
                auto.CostoKilometroPromedio = auto.CostoTotalCombustible / kilometrosRecorrido; //Costo por kilometro
                auto.CostoLitroPromedio = auto.CostoTotalCombustible / auto.LitrosTotalesConsumidos; //Costo por litro
            }

            auto.KilometrajeActual = otrasCargas.Last().Kilometraje;
            #endregion

            _db.SaveChanges();
           
        }

        public static void EliminarCargaCombustible(ProyectoAutoContext _db, int _idAuto, int _idCargaOriginal)
        {
            //Actualizar historial general auto, proveedor y carga siguiente

            Auto auto = _db.Autos.Where(a => a.Id == _idAuto).FirstOrDefault();
            HistorialCargaCombustible cargaOriginal = auto.CargasCombustible.Where(hcc => hcc.Id == _idCargaOriginal).FirstOrDefault();
            List<HistorialCargaCombustible> otrasCargas = auto.CargasCombustible.OrderBy(h => h.FechaHora).ToList();

            HistorialCargaCombustible cargaAnterior = null;
            HistorialCargaCombustible cargaSiguiente = null;
            for (int i = 0; i < otrasCargas.Count; i++)
            {
                if (otrasCargas[i].Id == cargaOriginal.Id)
                {
                    if ((i + 1) < otrasCargas.Count)
                        cargaSiguiente = otrasCargas[i + 1];
                    if ((i - 1) >= 0)
                        cargaAnterior = otrasCargas[i - 1];
                }
            }

            auto.LitrosTotalesConsumidos = auto.LitrosTotalesConsumidos - cargaOriginal.CantidadLitros;
            auto.CostoTotalCombustible = auto.CostoTotalCombustible - cargaOriginal.CostoTotal;

            #region Actualizar proveedor

            if (cargaOriginal.Proveedor != null)
                cargaOriginal.Proveedor.GastoTotalCombustible -= cargaOriginal.CostoTotal;

            #endregion

            #region Actualizar Carga siguiente
            if(cargaSiguiente != null)
            {
                if(cargaAnterior == null)
                {
                    cargaSiguiente.KilometrosRecorridos = -1;
                    cargaSiguiente.CostoPorKilometro = -1;
                    cargaSiguiente.Rendimiento = -1;
                }
                else
                {

                    float kilometrosRecorridos = cargaSiguiente.Kilometraje - cargaAnterior.Kilometraje;
                    float costoPorKilometro = cargaAnterior.CostoTotal / kilometrosRecorridos;
                    float rendimiento = kilometrosRecorridos / cargaAnterior.CantidadLitros;

                    cargaSiguiente.KilometrosRecorridos = kilometrosRecorridos;
                    cargaSiguiente.CostoPorKilometro = costoPorKilometro;
                    cargaSiguiente.Rendimiento = rendimiento;


                }
            }
            #endregion

            auto.CargasCombustible.Remove(cargaOriginal);
            _db.SaveChanges();

            //Actulaizar auto luego de ser eliminado el historial
            #region Actualizar datos del auto

            if (otrasCargas.Count > 1)
            {
                int primerKilometraje = otrasCargas.First().Kilometraje;
                int ultimoKilometraje = otrasCargas.Last().Kilometraje;
                int kilometrosRecorrido = ultimoKilometraje - primerKilometraje;

                auto.RendimientoPromedio = kilometrosRecorrido / auto.LitrosTotalesConsumidos; //Kilometros por litro
                auto.CostoKilometroPromedio = auto.CostoTotalCombustible / kilometrosRecorrido; //Costo por kilometro
                auto.CostoLitroPromedio = auto.CostoTotalCombustible / auto.LitrosTotalesConsumidos; //Costo por litro
            }

            auto.KilometrajeActual = otrasCargas.Last().Kilometraje;
            #endregion

            _db.SaveChanges();

        }
    }
}