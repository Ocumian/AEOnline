using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AEOnline.Models;

namespace AEOnline.Controllers.web
{
    public class PackServiciosController : Controller
    {
        private ProyectoAutoContext db = new ProyectoAutoContext();

        // GET: PackServicios
        public ActionResult Index()
        {
            return View(db.PackServicios.ToList());
        }

        // GET: PackServicios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackServicio packServicio = db.PackServicios.Find(id);
            if (packServicio == null)
            {
                return HttpNotFound();
            }
            return View(packServicio);
        }

        // GET: PackServicios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PackServicios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,NumeroVehiculos,NumeroOperadores,ModuloCombustible,ModuloMantencion")] PackServicio packServicio)
        {
            if (ModelState.IsValid)
            {
                db.PackServicios.Add(packServicio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(packServicio);
        }

        // GET: PackServicios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackServicio packServicio = db.PackServicios.Find(id);
            if (packServicio == null)
            {
                return HttpNotFound();
            }
            return View(packServicio);
        }

        // POST: PackServicios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,NumeroVehiculos,NumeroOperadores,ModuloCombustible,ModuloMantencion")] PackServicio packServicio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(packServicio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(packServicio);
        }

        // GET: PackServicios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackServicio packServicio = db.PackServicios.Find(id);
            if (packServicio == null)
            {
                return HttpNotFound();
            }
            return View(packServicio);
        }

        // POST: PackServicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PackServicio packServicio = db.PackServicios.Find(id);
            List<Flota> flotas = db.Flotas.ToList();
            
            foreach(Flota f in flotas)
            {
                if(f.PackServicio == packServicio)
                {
                    f.PackServicio = null;
                }
            }

            db.PackServicios.Remove(packServicio);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
