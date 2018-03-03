using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.ComponentModel;
using AEOnline.ClasesAdicionales;

namespace AEOnline.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        public enum RolUsuario { Normal, AdminDeFlota, SuperAdmin, Bloqueado}


        [Key]
        public int Id { get; set; }

        [Required, StringLength(25, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener un mínimo de 3 carácteres y máximo de 25")]
        public string Nombre { get; set; }

        [Required]
        public RolUsuario Rol { get; set; }

        [Required]
        public string Password { get; set; }

        [Required, StringLength(50, MinimumLength = 3, ErrorMessage = "El email debe tener un mínimo de 3 carácteres y un máximo de 50")]
        [Index("EmailIndex", IsUnique = true)]
        [EmailAddress]
        public string Email { get; set; }

        public int? UsuarioFlotaId { get; set; }
        public int? OperadorId { get; set; }

        [ForeignKey("UsuarioFlotaId")]
        public virtual UsuarioFlota UsuarioFlota { get; set; }
        [ForeignKey("OperadorId")]
        public virtual Operador Operador { get; set; }

        private static ProyectoAutoContext db = new ProyectoAutoContext();


        public static List<Usuario> FiltrarPorRol(List<Usuario> _lista, RolUsuario _rol)
        {
            List<Usuario> filtro = new List<Usuario>();

            for (int i = 0; i < _lista.Count; i++)
            {
                if (_lista[i].Rol == _rol)
                    filtro.Add(_lista[i]);
            }

            return filtro;
        }


        public static bool VerificarRepetido(string _email, string _exclusion = "")
        {
            Usuario repetido;

            if(_exclusion == "")
                repetido = db.Usuarios.Where(u => u.Email == _email).FirstOrDefault();
            else
                repetido = db.Usuarios.Where(u => u.Email == _email && u.Email != _exclusion).FirstOrDefault();


            if (repetido != null)
                return true;
            else
                return false;
        }

        public static void CrearUsuario (ProyectoAutoContext _db, string _nombre, string _email, string _password, RolUsuario _rol , int _idFlota )
        {
            string passEncriptada = PasswordHash.CreateHash(_password.Trim());
            Usuario nuevoUser = new Usuario()
            {
                Nombre = _nombre,
                Email = _email,
                Password = passEncriptada,
                Rol = _rol
            };

            _db.Usuarios.Add(nuevoUser);

            int idFlota = _idFlota;

            if (idFlota != 0)
            {
                Flota flotaAsignar = _db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();
                UsuarioFlota uf = new UsuarioFlota()
                {
                    Usuario = nuevoUser,
                    Flota = flotaAsignar
                };
                _db.UsuarioFlotas.Add(uf);
                _db.SaveChanges();

                flotaAsignar.UsuarioFlota = uf;
                nuevoUser.UsuarioFlota = uf;
            }


            _db.SaveChanges();


        }

        public static void EditarUsuario(ProyectoAutoContext _db, int _idOriginal, string _nuevoNombre, string _nuevoEmail, RolUsuario _nuevoRol , int _nuevaFlotaId)
        {
            Usuario userOriginal = _db.Usuarios.Where(u => u.Id == _idOriginal).FirstOrDefault();

            //Dependiendo del rol, primero se remueven las relaciones que se tenian como el rol anterior
            if(_nuevoRol == RolUsuario.Normal)
            {
                if (userOriginal.UsuarioFlotaId != null)
                {
                    userOriginal.UsuarioFlota.Flota.UsuarioFlota = null;
                    _db.UsuarioFlotas.Remove(userOriginal.UsuarioFlota);
                }
            }
            else if (_nuevoRol == RolUsuario.AdminDeFlota)
            {
                if (userOriginal.OperadorId != null)
                {
                    userOriginal.Operador.Usuario = null;
                }

                int idFlotaOriginal = 0;
                if (userOriginal.UsuarioFlotaId != null)
                    idFlotaOriginal = userOriginal.UsuarioFlota.Flota.Id;
     

                if (_nuevaFlotaId != idFlotaOriginal)
                {
                    if (userOriginal.UsuarioFlotaId != null)
                    {
                        userOriginal.UsuarioFlota.Flota.UsuarioFlota = null;
                        _db.UsuarioFlotas.Remove(userOriginal.UsuarioFlota);
                    }


                    if (_nuevaFlotaId != 0)
                    {
                        Flota flotaBuscada = _db.Flotas.Where(f => f.Id == _nuevaFlotaId).FirstOrDefault();

                        UsuarioFlota uf = new UsuarioFlota()
                        {
                            Usuario = userOriginal,
                            Flota = flotaBuscada
                        };

                        _db.UsuarioFlotas.Add(uf);
                        _db.SaveChanges();
                        userOriginal.UsuarioFlota = uf;
                        flotaBuscada.UsuarioFlota = uf;
                    }
                }
            }
            else
            {


                if (userOriginal.OperadorId != null)
                {
                    userOriginal.Operador.Usuario = null;
                }


                if (userOriginal.UsuarioFlotaId != null)
                {
                    userOriginal.UsuarioFlota.Flota.UsuarioFlota = null;
                    _db.UsuarioFlotas.Remove(userOriginal.UsuarioFlota);
                }
            }


            userOriginal.Nombre = _nuevoNombre;
            userOriginal.Email = _nuevoEmail;
            userOriginal.Rol = _nuevoRol;

            _db.SaveChanges();


        }

        public static void EliminarUsuario(ProyectoAutoContext _db, int _idUser)
        {
            Usuario user = _db.Usuarios.Where(u => u.Id == _idUser).FirstOrDefault();

            if(user.OperadorId != null)
            {
                user.Operador.Usuario = null;
            }

            if(user.UsuarioFlotaId != null)
            {
                user.UsuarioFlota.Flota.UsuarioFlota = null;
                _db.UsuarioFlotas.Remove(user.UsuarioFlota);
                _db.SaveChanges();
            }
            _db.Usuarios.Remove(user);
            _db.SaveChanges();

        }

    }
}