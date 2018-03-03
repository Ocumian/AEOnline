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
using System.Device.Location;

namespace AEOnline.Controllers
{
    /*
    Puede que la clase WebApiConfig requiera cambios adicionales para agregar una ruta para este controlador. Combine estas instrucciones en el método Register de la clase WebApiConfig según corresponda. Tenga en cuenta que las direcciones URL de OData distinguen mayúsculas de minúsculas.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using AEOnline.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Auto>("Autos");
    builder.EntitySet<Flota>("Flotas"); 
    builder.EntitySet<HistorialVelocidad>("HistorialesVelocidad"); 
    builder.EntitySet<UsuarioAuto>("UsuarioAutos"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class AutosController : ODataController
    {
        private ProyectoAutoContext db = new ProyectoAutoContext();

        //POST: odata/Autos/FechaEjemplo
        //Parametros: Fecha
        public string FechaEjemplo(ODataActionParameters parameters)
        {
            if (parameters == null)
                return "Parametros null";

            string fechaString = (string)parameters["Fecha"];
            fechaString = fechaString.Replace('-', '/');

            DateTime fecha;
            bool result = DateTime.TryParseExact(fechaString, FormatoFecha.formato, FormatoFecha.provider, DateTimeStyles.None, out fecha);

            if (result)
                return "OK";
            else
                return "Error";

        }

        //POST: odata/Autos/FechaHoy
        public DateTime FechaHoy()
        {
            return DateTime.Now;
        }


        //POST: odata/Autos/AsignarPatente
        //Parametros: NuevaPatente, Email, Password
        public RespuestaOdata AsignarPatente(ODataActionParameters parameters)
        {
            //Busca un auto en la base de datos con la patente indicada
            //Debe estar ya creado de antemano en la base de datos
            //Si todo está bien, el servicio responde con la ID que el corresponde a ese auto

            if (parameters == null)
            {
                return new RespuestaOdata() { Id = -1, Mensaje = "error" };
            }

            string nuevaPatente = (string)parameters["NuevaPatente"];
            string emailUser = (string)parameters["Email"];
            string pass = (string)parameters["Password"];

            bool validado = false;
            Usuario userEncontrado = db.Usuarios.Where(u => u.Email == emailUser).FirstOrDefault();
            if (userEncontrado != null)
            {
                if (userEncontrado.Rol == Usuario.RolUsuario.SuperAdmin)
                    validado = PasswordHash.ValidatePassword(pass, userEncontrado.Password);
            }

            if (validado == false)
                return new RespuestaOdata() { Id = -1, Mensaje = "Email/Contraseña no válidos." };

            Auto auto = db.Autos.Where(a => a.Patente == nuevaPatente).FirstOrDefault();

            if (auto == null)
                return new RespuestaOdata() { Id = -1, Mensaje = "No existe auto con tal patente." };


            //Todo ok, responder con la ID  que le corresponde
            return new RespuestaOdata() { Id = auto.Id, Mensaje = "Patente asignada correctamente." };
        }

        //POST: odata/Autos/ActualizarVelocidadLista
        //Parametros: Id,Registros
        //(FechaHora,Valor)
        public IHttpActionResult ActualizarVelocidadLista(ODataActionParameters parameters)
        {
            if (parameters == null)
                return BadRequest();

            int id = (int)parameters["Id"];
            var registros = parameters["Registros"] as IEnumerable<RegistroHistorial>;
            List<RegistroHistorial> listaRegistros = registros.ToList();

            for (int i = 0; i < listaRegistros.Count; i++)
            {
                string horaRegistroString = listaRegistros[i].FechaHora;
                DateTime horaRegistro;
                bool result = DateTime.TryParseExact(horaRegistroString,
                         FormatoFecha.formato, FormatoFecha.provider, DateTimeStyles.None, out horaRegistro);

                if (result == false)
                    return BadRequest();

                listaRegistros[i].FechaDateTime = horaRegistro;
            }

            listaRegistros = listaRegistros.OrderBy(r => r.FechaDateTime).ToList();

            RegistroHistorial registroInicio = listaRegistros.First();
            RegistroHistorial registroFinal = listaRegistros.Last();
            RegistroHistorial registro1Cuarto = null;
            RegistroHistorial registroMitad = null;
            RegistroHistorial registro3Cuartos = null;
            RegistroHistorial registroMayor = null;
            RegistroHistorial registrosMenor = null;

            float valorMayor = 0;
            float valorMenor = float.MaxValue;
            float valor1Cuarto = (listaRegistros.Count / 4);
            float valorMitad = (listaRegistros.Count / 2);
            float valor3Cuartos = (listaRegistros.Count / 4) * 3;


            for (int i = 0; i < listaRegistros.Count; i++)
            {
                RegistroHistorial regActual = listaRegistros[i];

                if (regActual.Valor > valorMayor)
                {
                    registroMayor = regActual;
                    valorMayor = regActual.Valor;
                }

                if (regActual.Valor < valorMenor)
                {
                    registrosMenor = regActual;
                    valorMenor = regActual.Valor;
                }

                if(registro1Cuarto == null && i > valor1Cuarto)
                {
                    registro1Cuarto = regActual;
                }

                if (registroMitad == null && i > valorMitad)
                {
                    registroMitad = regActual;
                }

                if(registro3Cuartos == null && i > valor3Cuartos)
                {
                    registro3Cuartos = regActual;
                }
            }


            HistorialVelocidad nuevoHistorial = new HistorialVelocidad();
            nuevoHistorial.HoraRegistro = DateTime.Now;
            nuevoHistorial.HoraInicio = registroInicio.FechaDateTime;
            nuevoHistorial.HoraFinal = registroFinal.FechaDateTime;
            nuevoHistorial.HoraMenor = registrosMenor.FechaDateTime;
            nuevoHistorial.HoraMayor = registroMayor.FechaDateTime;
            nuevoHistorial.HoraMitad = registroMitad.FechaDateTime;
            nuevoHistorial.HoraUnCuarto = registro1Cuarto.FechaDateTime;
            nuevoHistorial.HoraTresCuartos = registro3Cuartos.FechaDateTime;

            nuevoHistorial.ValorInicio = registroInicio.Valor;
            nuevoHistorial.ValorFinal = registroFinal.Valor;
            nuevoHistorial.ValorMayor = registroMayor.Valor;
            nuevoHistorial.ValorMenor = registrosMenor.Valor;
            nuevoHistorial.ValorMitad = registroMitad.Valor;
            nuevoHistorial.ValorUnCuarto = registro1Cuarto.Valor;
            nuevoHistorial.ValorTresCuartos = registro3Cuartos.Valor;

            DateTime fecha = nuevoHistorial.HoraInicio;
            Auto auto = db.Autos.Where(a => a.Id == id).FirstOrDefault();
            List<HistorialDiario> historiales = auto.HistorialesDiarios.ToList();
            historiales.Reverse();

            DateTime fechaHoy = DateTime.Today;
            HistorialDiario historialHoy = historiales.Where(h => h.Fecha.Year == fechaHoy.Year
                                                                && h.Fecha.Month == fechaHoy.Month
                                                                && h.Fecha.Day == fechaHoy.Day).FirstOrDefault();

            if (historialHoy == null)
            {
                historialHoy = new HistorialDiario()
                {
                    Fecha = fechaHoy,
                    historialesEnergia = new List<HistorialEnergia>(),
                    historialesPosicion = new List<HistorialPosicion>(),
                    historialesVelocidad = new List<HistorialVelocidad>()
                };
                auto.HistorialesDiarios.Add(historialHoy);
            }

            historialHoy.historialesVelocidad.Add(nuevoHistorial);
            db.SaveChanges();

            return Ok();
        }

        //POST: odata/Autos/ActualizarEnergia
        //Parametros: Id,HoraRegistro,ValorInicio,HoraInicio,ValorFinal,HoraFinal,ValorMenor,HoraMenor,ValorMayor,HoraMayor,ValorMitad,HoraMitad
        public IHttpActionResult ActualizarEnergia(ODataActionParameters parameters)
        {
            if (parameters == null)
                return BadRequest();

            int id = (int)parameters["Id"];
            string horaRegistroString = (string)parameters["HoraRegistro"];
            DateTime horaRegistro;

            float valorInicio = (float)parameters["ValorInicio"];
            string horaInicioString = (string)parameters["HoraInicio"];
            DateTime horaInicio;

            float valorFinal = (float)parameters["ValorFinal"];
            string horaFinalString = (string)parameters["HoraFinal"];
            DateTime horaFinal;

            float valorMenor = (float)parameters["ValorMayor"];
            string horaMenorString = (string)parameters["HoraMayor"];
            DateTime horaMenor;

            float valorMayor = (float)parameters["ValorMenor"];
            string horaMayorString = (string)parameters["HoraMenor"];
            DateTime horaMayor;

            float valorMitad = (float)parameters["ValorMitad"];
            string horaMitadString = (string)parameters["HoraMitad"];
            DateTime horaMitad;

            bool resultFechas;

            #region Transformar strings a fechas
            resultFechas = DateTime.TryParseExact(horaRegistroString,
                FormatoFecha.formato, FormatoFecha.provider, DateTimeStyles.None, out horaRegistro);

            resultFechas = DateTime.TryParseExact(horaInicioString,
    FormatoFecha.formato, FormatoFecha.provider, DateTimeStyles.None, out horaInicio);

            resultFechas = DateTime.TryParseExact(horaFinalString,
    FormatoFecha.formato, FormatoFecha.provider, DateTimeStyles.None, out horaFinal);

            resultFechas = DateTime.TryParseExact(horaMenorString,
    FormatoFecha.formato, FormatoFecha.provider, DateTimeStyles.None, out horaMenor);

            resultFechas = DateTime.TryParseExact(horaMayorString,
    FormatoFecha.formato, FormatoFecha.provider, DateTimeStyles.None, out horaMayor);

            resultFechas = DateTime.TryParseExact(horaMitadString,
    FormatoFecha.formato, FormatoFecha.provider, DateTimeStyles.None, out horaMitad);

            #endregion

            if (resultFechas == false)
                return BadRequest();

            Auto auto = db.Autos.Where(a => a.Id == id).FirstOrDefault();
            List<HistorialDiario> historiales = auto.HistorialesDiarios.ToList();
            historiales.Reverse();

            DateTime fechaHoy = DateTime.Today;
            HistorialDiario historialHoy = historiales.Where(h => h.Fecha.Year == fechaHoy.Year
                                                                && h.Fecha.Month == fechaHoy.Month
                                                                && h.Fecha.Day == fechaHoy.Day).FirstOrDefault();


            if (historialHoy == null)
            {
                historialHoy = new HistorialDiario()
                {
                    Fecha = fechaHoy,
                    historialesEnergia = new List<HistorialEnergia>(),
                    historialesPosicion = new List<HistorialPosicion>(),
                    historialesVelocidad = new List<HistorialVelocidad>()
                };
                auto.HistorialesDiarios.Add(historialHoy);
            }

            HistorialEnergia nuevoHistorial = new HistorialEnergia();
            nuevoHistorial.HoraRegistro = horaRegistro;

            nuevoHistorial.HoraInicio = horaInicio;
            nuevoHistorial.HoraFinal = horaFinal;
            nuevoHistorial.HoraMayor = horaMayor;
            nuevoHistorial.HoraMenor = horaMenor;
            nuevoHistorial.HoraMitad = horaMitad;

            nuevoHistorial.ValorInicio = valorInicio;
            nuevoHistorial.ValorFinal = valorFinal;
            nuevoHistorial.ValorMayor = valorMayor;
            nuevoHistorial.ValorMenor = valorMenor;
            nuevoHistorial.ValorMitad = valorMitad;

            historialHoy.historialesEnergia.Add(nuevoHistorial);
            db.SaveChanges();

            return Ok();

        }

        //POST: odata/Autos/ActualizarPosicionLista
        //Parametros: Id,ListaPosiciones
        //(FechaHora,Latitud,Longitud)
        public IHttpActionResult ActualizarPosicionLista(ODataActionParameters parameters)
        {
            if (parameters == null)
                return BadRequest();

            int id = (int)parameters["Id"];
            var posiciones = parameters["ListaPosiciones"] as IEnumerable<Posicion>;
            List<Posicion> listaPosiciones = posiciones.ToList();
            List<HistorialPosicion> hisPosicion = new List<HistorialPosicion>();

            for (int i = 0; i < listaPosiciones.Count; i++)
            {
                double latitud = listaPosiciones[i].Latitud;
                double longitud = listaPosiciones[i].Longitud;
                string horaString = listaPosiciones[i].FechaHora;

                DateTime horaRegistro;
                bool result = DateTime.TryParseExact(horaString,
                         FormatoFecha.formato, FormatoFecha.provider, DateTimeStyles.None, out horaRegistro);

                if (result == false)
                    return BadRequest();

                HistorialPosicion hp = new HistorialPosicion();
                hp.FechaHora = horaRegistro;
                hp.Latitud = latitud;
                hp.Longitud = longitud;

                hisPosicion.Add(hp);
            }

            if (hisPosicion.Count == 0)
                return BadRequest();

            DateTime fecha = hisPosicion[0].FechaHora;
            Auto auto = db.Autos.Where(a => a.Id == id).FirstOrDefault();
            List<HistorialDiario> historiales = auto.HistorialesDiarios.ToList();
            historiales.Reverse();

            DateTime fechaHoy = DateTime.Today;
            HistorialDiario historialHoy = historiales.Where(h => h.Fecha.Year == fechaHoy.Year
                                                                && h.Fecha.Month == fechaHoy.Month
                                                                && h.Fecha.Day == fechaHoy.Day).FirstOrDefault();

            if (historialHoy == null)
            {
                historialHoy = new HistorialDiario()
                {
                    //Fecha = hisPosicion[hisPosicion.Count -1].FechaHora.Date,
                    Fecha = fechaHoy,
                    historialesEnergia = new List<HistorialEnergia>(),
                    historialesPosicion = new List<HistorialPosicion>(),
                    historialesVelocidad = new List<HistorialVelocidad>()
                };
                auto.HistorialesDiarios.Add(historialHoy);
            }


            for (int i = 0; i < hisPosicion.Count; i++)
            {
                historialHoy.historialesPosicion.Add(hisPosicion[i]);
            }

            HistorialPosicion ultimaPosicion = hisPosicion[hisPosicion.Count - 1];
            auto.Latitud = ultimaPosicion.Latitud;
            auto.Longitud = ultimaPosicion.Longitud;
            db.SaveChanges();

            return Ok();
        }


        //POST: odata/Autos/ObtenerHistorialesVelocidad
        //Parametros: Id,Fecha
        public List<HistorialVelocidad> ObtenerHistorialesVelocidad(ODataActionParameters parameters)
        {
            return new List<HistorialVelocidad>();
            //if (parameters == null)
            //    return new List<HistorialVelocidad>();

            //int id = (int)parameters["Id"];
            //string fechaString = (string)parameters["Fecha"];
            //fechaString = fechaString.Replace('-', '/');

            //DateTime fecha;
            //bool result = DateTime.TryParseExact(fechaString, FormatoFecha.formato, FormatoFecha.provider, DateTimeStyles.None, out fecha);

            //Auto auto = db.Autos.Where(a => a.Id == id).FirstOrDefault();

            //if(auto == null || result == false)
            //    return new List<HistorialVelocidad>();

            //List<HistorialVelocidad> historialFiltrado = auto.HistorialVelocidad.Where(h => h.FechaHora.Date == fecha.Date).ToList();

            //return historialFiltrado;
        }

        //POST: odata/Autos/ObtenerHistorialesPosicion
        //Parametros: Id,FechaInicio, FechaFinal
        public List<HistorialPosicion> ObtenerHistorialesPosicion(ODataActionParameters parameters)
        {
            if (parameters == null)
                return new List<HistorialPosicion>();

            int id = (int)parameters["Id"];

            string fechaInicioString = (string)parameters["FechaInicio"];
            fechaInicioString = fechaInicioString.Replace('-', '/');

            string fechaFinalString = (string)parameters["FechaFinal"];
            fechaFinalString = fechaFinalString.Replace('-', '/');

            DateTime fechaInicio;
            bool result = DateTime.TryParseExact(fechaInicioString, FormatoFecha.formato, FormatoFecha.provider, DateTimeStyles.None, out fechaInicio);

            DateTime fechaFinal;
            result = DateTime.TryParseExact(fechaFinalString, FormatoFecha.formato, FormatoFecha.provider, DateTimeStyles.None, out fechaFinal);

            Auto auto = db.Autos.Where(a => a.Id == id).FirstOrDefault();

            if (auto == null || result == false)
                return new List<HistorialPosicion>();

            List<HistorialPosicion> historialFiltrado = db.HistorialesPosicion.Where(h => h.FechaHora >= fechaInicio && h.FechaHora <= fechaFinal).ToList();

            return historialFiltrado;
        }




        // GET: odata/Autos
        [EnableQuery]
        public IQueryable<Auto> GetAutos()
        {
            return db.Autos;
        }

        // GET: odata/Autos(5)
        [EnableQuery]
        public SingleResult<Auto> GetAuto([FromODataUri] int key)
        {
            return SingleResult.Create(db.Autos.Where(auto => auto.Id == key));
        }

        // PUT: odata/Autos(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<Auto> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Auto auto = db.Autos.Find(key);
            if (auto == null)
            {
                return NotFound();
            }

            patch.Put(auto);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AutoExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(auto);
        }

        // POST: odata/Autos
        public IHttpActionResult Post(Auto auto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Autos.Add(auto);
            db.SaveChanges();

            return Created(auto);
        }

        // PATCH: odata/Autos(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Auto> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Auto auto = db.Autos.Find(key);
            if (auto == null)
            {
                return NotFound();
            }

            patch.Patch(auto);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AutoExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(auto);
        }

        // DELETE: odata/Autos(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Auto auto = db.Autos.Find(key);
            if (auto == null)
            {
                return NotFound();
            }

            db.Autos.Remove(auto);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AutoExists(int key)
        {
            return db.Autos.Count(e => e.Id == key) > 0;
        }


    }
}
