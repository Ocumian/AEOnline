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
    [Table("Operador")]
    public class Operador
    {
        //Operador/Chofer de una flota
        //Creado por admin flota
        //asociado opcionalmente a un usuario y auto


        [Key]
        public int Id { get; set; }

        [Required, StringLength(25, MinimumLength = 3, ErrorMessage = "El nombre del operario debe tener un mínimo de 3 carácteres y máximo de 25")]
        public string Nombre { get; set; }

        public string TipoLicencia { get; set; }

        public virtual Usuario Usuario { get; set; }
        //public virtual Auto Auto { get; set; }
        public virtual List<Auto> Autos { get; set; }


        public static void CrearOperador(ProyectoAutoContext _db, int _idFlota, 
            string _nombre, string _tipoLicencia, int _idUsuario, int _idAuto)
        {
            Flota flota = _db.Flotas.Where(f => f.Id == _idFlota).FirstOrDefault();

            Auto auto = flota.Autos.Where(a => a.Id == _idAuto).FirstOrDefault();
            Usuario user = _db.Usuarios.Where(u => u.Id == _idUsuario).FirstOrDefault();

            Operador nuevoOperador = new Operador();
            nuevoOperador.Nombre = _nombre;
            nuevoOperador.TipoLicencia = _tipoLicencia;
            flota.Operadores.Add(nuevoOperador);

            _db.SaveChanges();

            if(auto != null)
            {
                //nuevoOperador.Auto = auto;
                nuevoOperador.Autos = new List<Auto>();
                nuevoOperador.Autos.Add(auto);

                auto.Operador = nuevoOperador;
            }

            if(user != null)
            {
                nuevoOperador.Usuario = user;
                user.Operador = nuevoOperador;
            }
            _db.SaveChanges();
        }


        public static void EditarOperador(ProyectoAutoContext _db, int _idFlota, int _idOriginal,
            string _nuevoNombre, string _tipoLicencia, int _idNuevoUsuario, int _idNuevoAuto)
        {
            Flota flota = _db.Flotas.Where(f => f.Id == _idFlota).FirstOrDefault();
            Operador operador = flota.Operadores.Where(o => o.Id == _idOriginal).FirstOrDefault();
            operador.Nombre = _nuevoNombre;
            operador.TipoLicencia = _tipoLicencia;

            if(_idNuevoAuto == 0)
            {
                //if(operador.Auto != null)
                //{
                //    operador.Auto.Operador = null;
                //    operador.Auto = null;
                //}

                if(operador.Autos.Count > 0)
                {
                    operador.Autos.First().Operador = null;
                    operador.Autos.Clear();
                }
            }
            else
            {
                //if (operador.Auto != null)
                //    operador.Auto.Operador = null;

                //Auto autoNuevo = flota.Autos.Where(a => a.Id == _idNuevoAuto).FirstOrDefault();
                //operador.Auto = autoNuevo;
                //autoNuevo.Operador = operador;

                if (operador.Autos.Count > 0)
                {
                    operador.Autos.First().Operador = null;
                    operador.Autos.Clear();
                }
                    
                Auto autoNuevo = flota.Autos.Where(a => a.Id == _idNuevoAuto).FirstOrDefault();
                operador.Autos.Add(autoNuevo);
                autoNuevo.Operador = operador;
            }


            if(_idNuevoUsuario == 0)
            {
                if(operador.Usuario != null)
                {
                    operador.Usuario.Operador = null;
                    operador.Usuario = null;
                }
            }
            else
            {
                if (operador.Usuario != null)
                    operador.Usuario.Operador = null;

                Usuario nuevoUser = _db.Usuarios.Where(u => u.Id == _idNuevoUsuario).FirstOrDefault();
                operador.Usuario = nuevoUser;
                nuevoUser.Operador = operador;
            }

            _db.SaveChanges();
        }

        public static void EliminarOperador(ProyectoAutoContext _db, int _idOperador)
        {
            Operador operador = _db.Operadores.Where(o => o.Id == _idOperador).FirstOrDefault();

            if (operador.Autos.Count > 0)
            {
                operador.Autos.First().Operador = null;
                operador.Autos.Clear();
            }
            //if (operador.Auto != null)
            //    operador.Auto.Operador = null;
            if (operador.Usuario != null)
                operador.Usuario.Operador = null;



            _db.Operadores.Remove(operador);
            _db.SaveChanges();

        }

    }
}