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
    builder.EntitySet<Flota>("Flotas");
    builder.EntitySet<Auto>("Autos"); 
    builder.EntitySet<UsuarioFlota>("UsuarioFlotas"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class FlotasController : ODataController
    {
        private ProyectoAutoContext db = new ProyectoAutoContext();

        // GET: odata/Flotas
        [EnableQuery]
        public IQueryable<Flota> GetFlotas()
        {
            return db.Flotas;
        }

        // GET: odata/Flotas(5)
        [EnableQuery]
        public SingleResult<Flota> GetFlota([FromODataUri] int key)
        {
            return SingleResult.Create(db.Flotas.Where(flota => flota.Id == key));
        }

        // PUT: odata/Flotas(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<Flota> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Flota flota = db.Flotas.Find(key);
            if (flota == null)
            {
                return NotFound();
            }

            patch.Put(flota);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlotaExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(flota);
        }

        // POST: odata/Flotas
        public IHttpActionResult Post(Flota flota)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Flotas.Add(flota);
            db.SaveChanges();

            return Created(flota);
        }

        // PATCH: odata/Flotas(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Flota> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Flota flota = db.Flotas.Find(key);
            if (flota == null)
            {
                return NotFound();
            }

            patch.Patch(flota);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlotaExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(flota);
        }

        // DELETE: odata/Flotas(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Flota flota = db.Flotas.Find(key);
            if (flota == null)
            {
                return NotFound();
            }

            db.Flotas.Remove(flota);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }


        // GET: odata/Flotas(5)/UsuarioFlota
        [EnableQuery]
        public SingleResult<UsuarioFlota> GetUsuarioFlota([FromODataUri] int key)
        {
            return SingleResult.Create(db.Flotas.Where(m => m.Id == key).Select(m => m.UsuarioFlota));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FlotaExists(int key)
        {
            return db.Flotas.Count(e => e.Id == key) > 0;
        }
    }
}
