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

namespace AEOnline.Controllers
{
    /*
    Puede que la clase WebApiConfig requiera cambios adicionales para agregar una ruta para este controlador. Combine estas instrucciones en el método Register de la clase WebApiConfig según corresponda. Tenga en cuenta que las direcciones URL de OData distinguen mayúsculas de minúsculas.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using AEOnline.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<HistorialVelocidad>("HistorialVelocidades");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class HistorialVelocidadesController : ODataController
    {
        private ProyectoAutoContext db = new ProyectoAutoContext();

        // GET: odata/HistorialVelocidades
        [EnableQuery]
        public IQueryable<HistorialVelocidad> GetHistorialVelocidades()
        {
            return db.HistorialesVelocidad;
        }

        // GET: odata/HistorialVelocidades(5)
        [EnableQuery]
        public SingleResult<HistorialVelocidad> GetHistorialVelocidad([FromODataUri] int key)
        {
            return SingleResult.Create(db.HistorialesVelocidad.Where(historialVelocidad => historialVelocidad.Id == key));
        }

        // PUT: odata/HistorialVelocidades(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<HistorialVelocidad> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            HistorialVelocidad historialVelocidad = db.HistorialesVelocidad.Find(key);
            if (historialVelocidad == null)
            {
                return NotFound();
            }

            patch.Put(historialVelocidad);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HistorialVelocidadExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(historialVelocidad);
        }

        // POST: odata/HistorialVelocidades
        public IHttpActionResult Post(HistorialVelocidad historialVelocidad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.HistorialesVelocidad.Add(historialVelocidad);
            db.SaveChanges();

            return Created(historialVelocidad);
        }

        // PATCH: odata/HistorialVelocidades(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<HistorialVelocidad> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            HistorialVelocidad historialVelocidad = db.HistorialesVelocidad.Find(key);
            if (historialVelocidad == null)
            {
                return NotFound();
            }

            patch.Patch(historialVelocidad);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HistorialVelocidadExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(historialVelocidad);
        }

        // DELETE: odata/HistorialVelocidades(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            HistorialVelocidad historialVelocidad = db.HistorialesVelocidad.Find(key);
            if (historialVelocidad == null)
            {
                return NotFound();
            }

            db.HistorialesVelocidad.Remove(historialVelocidad);
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

        private bool HistorialVelocidadExists(int key)
        {
            return db.HistorialesVelocidad.Count(e => e.Id == key) > 0;
        }
    }
}
