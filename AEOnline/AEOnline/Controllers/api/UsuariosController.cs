using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using AEOnline.Models;
using AEOnline.Models.WebModels;
using AEOnline.ClasesAdicionales;
using System.Globalization;
using GMap.NET;

namespace AEOnline.Controllers
{
    /*
    Puede que la clase WebApiConfig requiera cambios adicionales para agregar una ruta para este controlador. Combine estas instrucciones en el método Register de la clase WebApiConfig según corresponda. Tenga en cuenta que las direcciones URL de OData distinguen mayúsculas de minúsculas.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using AEOnline.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Usuario>("Usuarios");
    builder.EntitySet<UsuarioAuto>("UsuarioAutos"); 
    builder.EntitySet<UsuarioFlota>("UsuarioFlotas"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class UsuariosController : ODataController
    {
        private ProyectoAutoContext db = new ProyectoAutoContext();


        //POST: odata/Usuarios/IniciarSesion
        //Parametros: Email, Password
        public RespuestaOdata IniciarSesion(ODataActionParameters parameters)
        {
            //Inicia sesión y si todo está ok, responde con la patente e ID del vehículo

            if (parameters == null)
            {
                return new RespuestaOdata() { Id = -1, Patente ="", Mensaje = "error" };
            }

            string emailUser = (string)parameters["Email"];
            string pass = (string)parameters["Password"];

            bool validado = false;
            Usuario userEncontrado = db.Usuarios.Where(u => u.Email == emailUser).FirstOrDefault();
            if (userEncontrado != null)
            {
                validado = PasswordHash.ValidatePassword(pass, userEncontrado.Password);
            }

            if (validado == false)
                return new RespuestaOdata() { Id = -1, Patente = "", Mensaje = "Email/Contraseña no válidos." };
            if(userEncontrado.OperadorId == null)
                return new RespuestaOdata() { Id = -1, Patente="", Mensaje = "Usted no tiene auto asignado."};
            if(userEncontrado.Rol == Usuario.RolUsuario.Bloqueado)
                return new RespuestaOdata() { Id = -1, Patente="", Mensaje = "Cuenta bloqueada" };

            int idAuto = userEncontrado.Operador.Auto.Id;
            string patenteAuto = userEncontrado.Operador.Auto.Patente;

            //Todo ok, responder con la ID y patente que le corresponde
            return new RespuestaOdata() { Id = idAuto, Patente = patenteAuto , Mensaje = "Sesión iniciada correctamente." };
        }

        //POST: odata/Usuarios/CrearCuentaUsuario
        //Parametros: Nombre, Email, Password
        public RespuestaOdata CrearCuentaUsuario(ODataActionParameters parameters)
        {
            if (parameters == null)
            {
                return new RespuestaOdata() { Id = -1, Mensaje = "error" };
            }

            string nombre = (string)parameters["Nombre"];
            string emailUser = (string)parameters["Email"];
            string pass = (string)parameters["Password"];

            string passEncriptada = PasswordHash.CreateHash(pass.Trim());

            Usuario repetido = db.Usuarios.Where(u => u.Email == emailUser).FirstOrDefault();

            if(repetido != null)
                return new RespuestaOdata() { Id = -1, Mensaje = "Email ingresado ya se encuentra ocupado." };

            Usuario nuevoUser = new Usuario()
            {
                Nombre = nombre,
                Email = emailUser,
                Password = passEncriptada,
                Rol = Usuario.RolUsuario.Normal
            };

            db.Usuarios.Add(nuevoUser);
            db.SaveChanges();

            return new RespuestaOdata() { Id = 0, Mensaje = "Cuenta creada correctamente." };
        }

        //POST: odata/Usuarios/ProveedoresActuales
        //Parametros: IdAuto
        public List<Proveedor> ProveedoresActuales(ODataActionParameters parameters)
        {
            List<Proveedor> proveedores = new List<Proveedor>();
            proveedores.Add(new Proveedor() { Id = 0, NombreComercial = "No asignar" });

            if (parameters == null)
            {
                return proveedores;
            }
            int idAuto = (int)parameters["IdAuto"];

            Auto auto = db.Autos.Where(a => a.Id == idAuto).FirstOrDefault();

            if (auto == null)
                return proveedores;

            if (auto.Flota == null)
                return proveedores;

            proveedores.AddRange(auto.Flota.Proveedores);

            return proveedores;
        }

        //POST: odata/Usuarios/CargarCombustible
        //Parametros: idAuto,FechaHora,EstanqueLLeno,CantidadLitros,CostoUnitario,Kilometraje,IdProveedor
        public RespuestaOdata CargarCombustible(ODataActionParameters parameters)
        {
            if (parameters == null)
                return new RespuestaOdata() { Id = -1, Mensaje = "error" };

            int idAuto = (int)parameters["IdAuto"];
            string fechaString = (string)parameters["FechaHora"];
            DateTime fechaHoraCarga;
            bool estanqueLleno = (bool)parameters["EstanqueLleno"];
            float cantidadLitros = (float)parameters["CantidadLitros"];
            int costoUnitario = (int)parameters["CostoUnitario"];
            int kilometraje = (int)parameters["Kilometraje"];
            int idProveedor = (int)parameters["IdProveedor"];

            fechaString = fechaString.Replace('-', '/');
            bool result = DateTime.TryParseExact(fechaString, FormatoFecha.formato, FormatoFecha.provider, DateTimeStyles.None, out fechaHoraCarga);

            if (result == false)
                return new RespuestaOdata() { Id = -1, Mensaje = "Error de fecha" };

            Auto auto = db.Autos.Where(a => a.Id == idAuto).FirstOrDefault();

            if (auto == null)
                return new RespuestaOdata() { Id = -1, Mensaje = "Auto no existente" };

            if (auto.CargasCombustible.Count > 0)
            {
                HistorialCargaCombustible ultimaCarga = auto.CargasCombustible.OrderBy(h => h.FechaHora).Last();

                if (kilometraje <= ultimaCarga.Kilometraje || fechaHoraCarga <= ultimaCarga.FechaHora)
                {
                    RespuestaOdata resp = new RespuestaOdata();
                    resp.Id = -1;
                    resp.Mensaje = "Se han encontrado los siguientes errores:\n";

                    if (kilometraje <= ultimaCarga.Kilometraje)
                        resp.Mensaje += "-Kilometraje inválido. Debe seguir el orden lógico de los registros. El último registro marcó: " + ultimaCarga.Kilometraje + " KMs\n";
                    if (fechaHoraCarga <= ultimaCarga.FechaHora)
                        resp.Mensaje += "La combinación de fecha y hora da una fecha anterior al último registro de combustible, lo cual no es lógico. El último registro fue el: " + ultimaCarga.FechaHora;

                    return resp;
                }
            }

            Proveedor proveedor = null;
            if(idProveedor != 0)
                proveedor = db.Proveedores.Where(p => p.Id == idProveedor).FirstOrDefault();

            HistorialCargaCombustible.NuevaCargaCombustible(db, idAuto, fechaHoraCarga, fechaHoraCarga, estanqueLleno, cantidadLitros, costoUnitario, kilometraje, proveedor);

            return new RespuestaOdata() { Id = 1, Mensaje = "Carga de combustible existosa." };
        }

        //POST: odata/Usuarios/NombreCalle
        //Parametros: lat,lng
        public string NombreCalle(ODataActionParameters parameters)
        {
            if (parameters == null)
                return "";

            double lat = (double)parameters["lat"];
            double lng = (double)parameters["lng"];

            string calle = Posicion.ObtenerCalle(lat, lng);

            return calle;
        }

        //POST: odata/Usuarios/ObtenerDireccion
        //Parametros: latInicio,lngInicio,latFinal,lngFinal
        public GDirections ObtenerDireccion(ODataActionParameters parameters)
        {
            if (parameters == null)
                return null;

            double latInicio = (double)parameters["latInicio"];
            double lngInicio = (double)parameters["lngInicio"];

            double latFinal = (double)parameters["latFinal"];
            double lngFinal = (double)parameters["lngFinal"];

            GDirections direccion = Posicion.ObtenerDireccion(latInicio, lngInicio, latFinal, lngFinal);

            return direccion;
        }

        // GET: odata/Usuarios
        [EnableQuery]
        public IQueryable<Usuario> GetUsuarios()
        {
            return db.Usuarios;
        }

        // GET: odata/Usuarios(5)
        [EnableQuery]
        public SingleResult<Usuario> GetUsuario([FromODataUri] int key)
        {
            return SingleResult.Create(db.Usuarios.Where(usuario => usuario.Id == key));
        }

        // PUT: odata/Usuarios(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<Usuario> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Usuario usuario = db.Usuarios.Find(key);
            if (usuario == null)
            {
                return NotFound();
            }

            patch.Put(usuario);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(usuario);
        }

        // POST: odata/Usuarios
        public IHttpActionResult Post(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Usuarios.Add(usuario);
            db.SaveChanges();

            return Created(usuario);
        }

        // PATCH: odata/Usuarios(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Usuario> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Usuario usuario = db.Usuarios.Find(key);
            if (usuario == null)
            {
                return NotFound();
            }

            patch.Patch(usuario);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(usuario);
        }

        // DELETE: odata/Usuarios(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Usuario usuario = db.Usuarios.Find(key);
            if (usuario == null)
            {
                return NotFound();
            }

            db.Usuarios.Remove(usuario);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }



        // GET: odata/Usuarios(5)/UsuarioFlota
        [EnableQuery]
        public SingleResult<UsuarioFlota> GetUsuarioFlota([FromODataUri] int key)
        {
            return SingleResult.Create(db.Usuarios.Where(m => m.Id == key).Select(m => m.UsuarioFlota));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UsuarioExists(int key)
        {
            return db.Usuarios.Count(e => e.Id == key) > 0;
        }
    }
}
