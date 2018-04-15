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
    [Table("Flota")]
    public class Flota
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(25, MinimumLength = 3, ErrorMessage = "El nombre debe tener un mínimo de 3 carácteres y máximo de 25"),
            Index("NombreIndex", IsUnique = true)]
        public string Nombre { get; set; }


        public virtual List<Usuario> SubAdmins { get; set; }
        public virtual List<Auto> Autos { get; set; }
        public virtual List<Operador> Operadores { get; set; }
        public virtual List<GrupoFlota> Grupos { get; set; }
        public virtual List<TipoVehiculo> TiposVehiculo { get; set; }
        public virtual List<Servicio> Servicios { get; set; }
        public virtual List<Proveedor> Proveedores { get; set; }

        public virtual List<Recordatorio> Recordatorios { get; set; }
        public virtual List<Alerta> Alerttas { get; set; }


        public int? UsuarioFlotaId { get; set; }
        [ForeignKey("UsuarioFlotaId")]
        public virtual UsuarioFlota UsuarioFlota { get; set; }

        public int? PackId { get; set; }
        [ForeignKey("PackId")]
        public virtual PackServicio PackServicio { get; set; }


        public static Flota CrearFlota(ProyectoAutoContext _db, string _nombre, int _idAdmin)
        {
            Flota nuevaFlota = new Flota();
            nuevaFlota.Nombre = _nombre;
            nuevaFlota.Servicios = new List<Servicio>();
            nuevaFlota.TiposVehiculo = new List<TipoVehiculo>();

            _db.Flotas.Add(nuevaFlota);

            if (_idAdmin != 0)
            {
                Usuario admin = _db.Usuarios.Where(u => u.Id == _idAdmin).FirstOrDefault();
                UsuarioFlota uf = new UsuarioFlota()
                {
                    Usuario = admin,
                    Flota = nuevaFlota
                };
                _db.UsuarioFlotas.Add(uf);
                _db.SaveChanges();
                nuevaFlota.UsuarioFlota = uf;
                admin.UsuarioFlota = uf;
            }

            _db.SaveChanges();

            //Creacion de datos default

            List<Servicio> serviciosDefault = CrearServiciosDefault();
            List<TipoVehiculo> tiposVehiculoDefault = CrearTiposVehiculoDefault();

            for (int i = 0; i < serviciosDefault.Count; i++)
            {
                nuevaFlota.Servicios.Add(serviciosDefault[i]);
            }

            for (int i = 0; i < tiposVehiculoDefault.Count; i++)
            {
                nuevaFlota.TiposVehiculo.Add(tiposVehiculoDefault[i]);
            }
            _db.SaveChanges();

            return nuevaFlota;
        }

        public static Flota CrearFlota(ProyectoAutoContext _db, string _nombre, int _idAdmin, string _nombrePackInicial)
        {
            Flota nuevaFlota = new Flota();
            nuevaFlota.Nombre = _nombre;
            nuevaFlota.Servicios = new List<Servicio>();
            nuevaFlota.TiposVehiculo = new List<TipoVehiculo>();

            _db.Flotas.Add(nuevaFlota);

            if (_idAdmin != 0)
            {
                Usuario admin = _db.Usuarios.Where(u => u.Id == _idAdmin).FirstOrDefault();
                UsuarioFlota uf = new UsuarioFlota()
                {
                    Usuario = admin,
                    Flota = nuevaFlota
                };
                _db.UsuarioFlotas.Add(uf);
                _db.SaveChanges();
                nuevaFlota.UsuarioFlota = uf;
                admin.UsuarioFlota = uf;
            }

            PackServicio packInicial = _db.PackServicios.Where(p => p.Nombre == _nombrePackInicial).FirstOrDefault();
            nuevaFlota.PackServicio = packInicial;

            _db.SaveChanges();

            //Creacion de datos default

            List<Servicio> serviciosDefault = CrearServiciosDefault();
            List<TipoVehiculo> tiposVehiculoDefault = CrearTiposVehiculoDefault();

            for (int i = 0; i < serviciosDefault.Count; i++)
            {
                nuevaFlota.Servicios.Add(serviciosDefault[i]);
            }

            for (int i = 0; i < tiposVehiculoDefault.Count; i++)
            {
                nuevaFlota.TiposVehiculo.Add(tiposVehiculoDefault[i]);
            }
            _db.SaveChanges();

            return nuevaFlota;
        }

        public static void EditarFlota(ProyectoAutoContext _db, int _idOriginal, string _nombreNuevo, int _idAdminNuevo, int _idPackServicio)
        {
            Flota flotaOriginal = _db.Flotas.Where(f => f.Id == _idOriginal).FirstOrDefault();
            Usuario nuevoAdmin = _db.Usuarios.Where(u => u.Id == _idAdminNuevo).FirstOrDefault();

            int idAdminOriginal = 0;
            if (flotaOriginal.UsuarioFlotaId != null)
                idAdminOriginal = flotaOriginal.UsuarioFlota.Usuario.Id;
            int idPackOriginal = 0;
            if (flotaOriginal.PackId != null)
                idPackOriginal = flotaOriginal.PackServicio.Id;


            if (_idAdminNuevo != idAdminOriginal)
            {
                // Si tiene una relacion anterior Desaparece
                if (flotaOriginal.UsuarioFlotaId != null)
                {
                    flotaOriginal.UsuarioFlota.Usuario.UsuarioFlota = null;
                    _db.UsuarioFlotas.Remove(flotaOriginal.UsuarioFlota);
                }

                //Si se selecciono algo diferente a "Sin Asignar", hay que hacer la nueva relacion
                if (_idAdminNuevo != 0)
                {
                    UsuarioFlota uf = new UsuarioFlota()
                    {
                        Usuario = nuevoAdmin,
                        Flota = flotaOriginal
                    };
                    _db.UsuarioFlotas.Add(uf);
                    _db.SaveChanges();
                    nuevoAdmin.UsuarioFlota = uf;
                    flotaOriginal.UsuarioFlota = uf;
                }
            }

            if(_idPackServicio != idPackOriginal)
            {
                PackServicio pack = _db.PackServicios.Where(p => p.Id == _idPackServicio).FirstOrDefault();
                flotaOriginal.PackServicio = pack;
            }

            flotaOriginal.Nombre = _nombreNuevo;

            _db.SaveChanges();

        }

        public static void EliminarFlota(ProyectoAutoContext _db, int _idFlota)
        {
            Flota flota = _db.Flotas.Where(f => f.Id == _idFlota).FirstOrDefault();

            List<int> autosEliminables = new List<int>();
            List<int> operadoresEliminables = new List<int>();

            foreach(Auto a in flota.Autos)
            {
                autosEliminables.Add(a.Id);
            }
            foreach(int aId in autosEliminables)
            {
                Auto.ElimiarAuto(_db, aId);
            }

            foreach(Operador o in flota.Operadores)
            {
                operadoresEliminables.Add(o.Id);
            }
            foreach (int oId in operadoresEliminables)
            {
                Operador.EliminarOperador(_db, oId);
            }

            if(flota.UsuarioFlotaId != null)
            {
                flota.UsuarioFlota.Usuario.UsuarioFlota = null;
                _db.UsuarioFlotas.Remove(flota.UsuarioFlota);
            }

            _db.Proveedores.RemoveRange(flota.Proveedores);
            _db.TiposVehiculo.RemoveRange(flota.TiposVehiculo);
            _db.Servicios.RemoveRange(flota.Servicios);
            _db.Alertas.RemoveRange(flota.Alerttas);
            _db.Recordatorios.RemoveRange(flota.Recordatorios);
            _db.SaveChanges();

            _db.Flotas.Remove(flota);
            _db.SaveChanges();
        }

        public static List<Servicio> CrearServiciosDefault()
        {
            List<Servicio> servicios = new List<Servicio>();

            servicios.Add(new Servicio() { NombreServicio = "Arreglo de motor" });
            servicios.Add(new Servicio() { NombreServicio = "Cambio de aceite" });
            servicios.Add(new Servicio() { NombreServicio = "Cambio de rueda" });

            return servicios;
        }

        public static List<TipoVehiculo> CrearTiposVehiculoDefault()
        {
            List<TipoVehiculo> tipoVehiculos = new List<TipoVehiculo>();

            tipoVehiculos.Add(new TipoVehiculo() { Tipo = "Automóvil" });
            tipoVehiculos.Add(new TipoVehiculo() { Tipo = "Camioneta" });
            tipoVehiculos.Add(new TipoVehiculo() { Tipo = "Bus" });

            return tipoVehiculos;
        }


    }
}