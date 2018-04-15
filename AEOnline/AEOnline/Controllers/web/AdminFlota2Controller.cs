using AEOnline.ClasesAdicionales;
using AEOnline.Models;
using AEOnline.Models.WebModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static AEOnline.Controllers.web.UserNormalController;

namespace AEOnline.Controllers.web
{
    public class AdminFlota2Controller : Controller
    {
        private ProyectoAutoContext db = new ProyectoAutoContext();

        // GET: AdminFlota
        public ActionResult Index()
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            List<Auto> autos = flota.Autos.Where(a => a.Atajo == true).ToList();

            AtajosPanelInicial atajos = new AtajosPanelInicial();
            atajos.Autos = autos;

            return View(atajos);
        }

        public ActionResult MapaDeFlota()
        {
            if (Session["Email"] == null)
                return RedirectToAction("Index", "Login");
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            return View(flota);
        }

        public ActionResult MisVehiculos()
        {
            return View();
        }



        #region ----------------------ADMIN VEHICULOS ------------------------------------------

        public ActionResult AdministrarAutos()
        {
            if (Session["Email"] == null)
                return RedirectToAction("Index", "Login");
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            List<Auto> autos = flota.Autos.ToList();

            return View(autos);
        }

        public ActionResult CrearAuto()
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            //List<Operador> operadoresSinAuto = flota.Operadores.Where(o => o.Auto == null).ToList();
            List<Operador> operadoresSinAuto = flota.Operadores.Where(o => o.Autos.Count == 0).ToList();
            operadoresSinAuto.Add(new Operador() { Id = 0, Nombre = "No asignar" });
            operadoresSinAuto.Reverse();

            List<TipoVehiculo> tipoVehiculos = flota.TiposVehiculo.ToList();
            tipoVehiculos.Add(new TipoVehiculo() { Id = 0, Tipo = "No asignar" });
            tipoVehiculos.Reverse();

            ViewBag.OperadorId = new SelectList(operadoresSinAuto, "Id", "Nombre");
            ViewBag.TipoVehiculoId = new SelectList(tipoVehiculos, "Id", "Tipo");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearAuto([Bind(Include = "Id,NombreVehiculo,Patente,Marca,Modelo,Year,KilometrajeActual,TipoVehiculoId,OperadorId")] CreacionAuto model)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            if(flota.PackId != null)
            {
                if(flota.Autos.Count >= flota.PackServicio.NumeroVehiculos)
                {
                    TempData["msg"] = "No puede superar el número máximo de vehículos. Su pack de servicios tiene un límite de "+flota.PackServicio.NumeroVehiculos;
                    return RedirectToAction("AdministrarAutos");
                }
            }

            Auto repetido = flota.Autos.Where(a => a.NombreVehiculo == model.NombreVehiculo).FirstOrDefault();
            if (repetido != null)
            {
                ModelState.Clear();
                ModelState.AddModelError("NombreVehiculo", "Ya existe un vehículo en su flota con el mismo nombre.");
                TryValidateModel(model);
            }


            if (ModelState.IsValid == false)
            {
                List<Operador> operadoresSinAuto = flota.Operadores.Where(o => o.Autos.Count == 0).ToList();
                //List<Operador> operadoresSinAuto = flota.Operadores.Where(o => o.Auto == null).ToList();
                operadoresSinAuto.Add(new Operador() { Id = 0, Nombre = "No asignar" });
                operadoresSinAuto.Reverse();

                List<TipoVehiculo> tipoVehiculos = flota.TiposVehiculo.ToList();
                tipoVehiculos.Add(new TipoVehiculo() { Id = 0, Tipo = "No asignar" });
                tipoVehiculos.Reverse();

                ViewBag.OperadorId = new SelectList(operadoresSinAuto, "Id", "Nombre");
                ViewBag.TipoVehiculoId = new SelectList(tipoVehiculos, "Id", "Tipo");

                return View("CrearAuto", model);
            }


            TipoVehiculo tipo = flota.TiposVehiculo.Where(t => t.Id == model.TipoVehiculoId).FirstOrDefault();
            Auto.CrearAuto(db, model.NombreVehiculo, model.Patente, tipo, model.Marca, model.Modelo, model.Year, model.KilometrajeActual, idFlota, model.OperadorId);

            TempData["msg"] = "Vehículo creado correctamente";
            return RedirectToAction("AdministrarAutos");
        }


        public ActionResult EditarAuto(int? id)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Auto auto = db.Autos.Find(id);

            if (auto == null)
            {
                return HttpNotFound();
            }

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            CreacionAuto modeloAuto = new CreacionAuto()
            {
                Id = auto.Id,
                NombreVehiculo = auto.NombreVehiculo,
                Patente = auto.Patente,
                Marca = auto.Marca,
                Modelo = auto.Modelo,
                Year = auto.Year,
                KilometrajeActual = auto.KilometrajeActual,
                FlotaId = idFlota,
                OperadorId = 0,
                TipoVehiculoId = 0
            };


            //List<Operador> operadoresSinAuto = flota.Operadores.Where(o => o.Auto == null).ToList();
            List<Operador> operadoresSinAuto = flota.Operadores.Where(o => o.Autos.Count == 0).ToList();
            if (auto.OperadorId != null)
            {
                modeloAuto.OperadorId = auto.Operador.Id;
                Operador operadorActual = flota.Operadores.Where(o => o.Id == auto.Operador.Id).FirstOrDefault();
                operadoresSinAuto.Add(operadorActual);
            }
            operadoresSinAuto.Add(new Operador() { Id = 0, Nombre = "No asignar" });
            operadoresSinAuto.Reverse();

            List<TipoVehiculo> tipoVehiculos = flota.TiposVehiculo.ToList();
            if (auto.TipoVehiculo != null)
                modeloAuto.TipoVehiculoId = auto.TipoVehiculo.Id;
            tipoVehiculos.Add(new TipoVehiculo() { Id = 0, Tipo = "No asignar" });
            tipoVehiculos.Reverse();

            ViewBag.OperadorId = new SelectList(operadoresSinAuto, "Id", "Nombre", modeloAuto.OperadorId);
            ViewBag.TipoVehiculoId = new SelectList(tipoVehiculos, "Id", "Tipo", modeloAuto.TipoVehiculoId);

            return View(modeloAuto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAuto([Bind(Include = "Id,NombreVehiculo,Patente,Marca,Modelo,Year,KilometrajeActual,TipoVehiculoId,OperadorId")] CreacionAuto model)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();
            Auto auto = flota.Autos.Where(a => a.Id == model.Id).FirstOrDefault();

            Auto repetido = flota.Autos.Where(a => a.NombreVehiculo == model.NombreVehiculo && a.Id != auto.Id).FirstOrDefault();
            if (repetido != null)
            {
                ModelState.Clear();
                ModelState.AddModelError("NombreVehiculo", "Ya existe un vehículo en su flota con el mismo nombre.");
                TryValidateModel(model);
            }

            if (ModelState.IsValid == false)
            {

                List<Operador> operadoresSinAuto = flota.Operadores.Where(o => o.Autos.Count == 0).ToList();
                //List<Operador> operadoresSinAuto = flota.Operadores.Where(o => o.Auto == null).ToList();
                operadoresSinAuto.Add(new Operador() { Id = 0, Nombre = "No asignar" });
                operadoresSinAuto.Reverse();

                List<TipoVehiculo> tipoVehiculos = flota.TiposVehiculo.ToList();
                tipoVehiculos.Add(new TipoVehiculo() { Id = 0, Tipo = "No asignar" });
                tipoVehiculos.Reverse();

                int operadorSeleccionado = 0;
                int tipoVehiculoSeleccionado = 0;

                if (auto.OperadorId != null)
                    operadorSeleccionado = auto.Operador.Id;
                if (auto.TipoVehiculo != null)
                    tipoVehiculoSeleccionado = auto.TipoVehiculo.Id;

                ViewBag.OperadorId = new SelectList(operadoresSinAuto, "Id", "Nombre", operadorSeleccionado);
                ViewBag.TipoVehiculoId = new SelectList(tipoVehiculos, "Id", "Tipo", tipoVehiculoSeleccionado);
                return View("EditarAuto", model);
            }

            TipoVehiculo tipo = flota.TiposVehiculo.Where(t => t.Id == model.TipoVehiculoId).FirstOrDefault();
            Auto.EditarAuto(db, model.Id, model.NombreVehiculo, model.Patente, tipo, model.Marca, model.Modelo, model.Year, model.KilometrajeActual, idFlota, model.OperadorId);

            TempData["msg"] = "Vehículo editado correctamente";
            return RedirectToAction("AdministrarAutos");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarAuto(int id)
        {
            Auto auto = db.Autos.Find(id);
            if (auto == null)
            {
                return HttpNotFound();
            }

            Auto.ElimiarAuto(db, id);

            TempData["msg"] = "Vehículo eliminado correctamente";
            return RedirectToAction("AdministrarAutos");
        }


        public ActionResult AdministrarTipos()
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            List<TipoVehiculo> tipos = flota.TiposVehiculo.ToList();

            return View(tipos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearTipo([Bind(Include = "Tipo")] TipoVehiculo model)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            if (ModelState.IsValid == false)
            {
                List<ModelErrorCollection> errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();

                return Json(new
                {
                    exito = false,
                    mensaje = "Debe ingresar un valor válido para el nuevo tipo."
                }, JsonRequestBehavior.AllowGet);
            }

            int idFlota = (int)Session["Flota"];
            TipoVehiculo.CrearTipoVehiculo(db, idFlota, model.Tipo);

            return Json(new
            {
                exito = true,
                mensaje = "Creación exitosa."
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarTipo(int id)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            TipoVehiculo tipo = flota.TiposVehiculo.Where(t => t.Id == id).FirstOrDefault();

            if (tipo == null)
            {
                return HttpNotFound();
            }

            TipoVehiculo.EliminarTipo(db, flota, tipo);
            return RedirectToAction("AdministrarTipos");
        }

        public ActionResult FichaAuto(int? id)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            if (id == null )
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();
            Auto auto = flota.Autos.Where(a => a.Id == id).FirstOrDefault();

            if (auto == null)
                return HttpNotFound();

            List<HistorialCargaCombustible> cargasCombustible = auto.CargasCombustible.OrderBy(hcc => hcc.FechaHora).ToList();
            cargasCombustible.Reverse();
            List<HistorialMantencion> mantenciones = auto.Mantenciones.OrderBy(hm => hm.Fecha).ToList();
            mantenciones.Reverse();

            ViewBag.CargasCombustible = cargasCombustible;
            ViewBag.Mantenciones = mantenciones;

            return View(auto);
        }

        public ActionResult RegistrosCelular(int? id, int? tipo) //tipo 0: chofer, tipo 1: auto
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            if (id == null || tipo == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            Auto auto = null;

            if (tipo == 0)
            {
                Operador operador = flota.Operadores.Where(o => o.Id == id).FirstOrDefault();

                if (operador == null)
                    return HttpNotFound();

                //if (operador.Auto != null)
                //    auto = operador.Auto;
                if (operador.Autos.Count > 0)
                    auto = operador.Autos.First();
            }
            else if (tipo == 1)
            {
                auto = flota.Autos.Where(u => u.Id == id).FirstOrDefault();
            }


            if (auto == null)
                return HttpNotFound();


            //SelectList tiposHistorial = new SelectList(HistorialWeb.ObtenerTiposHistorial());
            //ViewBag.MyType = tiposHistorial;

            CreacionUsuario cu = new CreacionUsuario()
            {
                Fecha = DateTime.Today,
                AutoPatente = auto.Patente,
                AutoId = auto.Id,
                AutoNombre = auto.NombreVehiculo
            };

            return View(cu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrosCelular(HistorialWeb.TiposHistorial? MyType, string Fecha, int AutoId)
        {
            string fechaString = Fecha;
            fechaString = fechaString.Replace('-', '/');
            fechaString += " 00:00:00";

            DateTime fechaSeleccionada;
            bool result = DateTime.TryParseExact(fechaString, FormatoFecha.formato, FormatoFecha.provider, DateTimeStyles.None, out fechaSeleccionada);

            #region Modelo Usado en vista principal

            Auto auto = db.Autos.Where(a => a.Id == AutoId).FirstOrDefault();

            CreacionUsuario us = new CreacionUsuario();
            us.Fecha = fechaSeleccionada;
            us.AutoPatente = auto.Patente;
            us.AutoId = auto.Id;
            us.AutoNombre = auto.NombreVehiculo;

            SelectList tiposHistorial = new SelectList(HistorialWeb.ObtenerTiposHistorial());
            ViewBag.MyType = tiposHistorial;
            ViewBag.HistorialSeleccionado = MyType.ToString();
            ViewBag.FechaSeleccionada = Fecha;

            #endregion


            //Modelo historial va al viewbag que se envía a la vista parcial
            HistorialWeb historialWeb = new HistorialWeb();
            historialWeb.FechaMostrar = fechaSeleccionada;
            historialWeb.PatenteAuto = auto.Patente;
            historialWeb.IdAuto = auto.Id;

            if (MyType == HistorialWeb.TiposHistorial.Velocidad)
            {
                historialWeb = HistorialesManager.PrepararHistorialVelocidad(db, historialWeb, fechaSeleccionada, us.AutoId);

                //Viewbag enviado a la vista parcial
                ViewBag.HistorialWeb = historialWeb;
            }
            else if (MyType == HistorialWeb.TiposHistorial.Posicion)
            {
                historialWeb = HistorialesManager.PrepararHistorialPosicion(db, historialWeb, fechaSeleccionada, us.AutoId);

                //Viewbag enviado a la vista parcial
                ViewBag.HistorialWeb = historialWeb;
            }
            else if (MyType == HistorialWeb.TiposHistorial.Energia)
            {
                historialWeb = HistorialesManager.PrepararHistorialEnergia(db, historialWeb, fechaSeleccionada, us.AutoId);

                //Viewbag enviado a la vista parcial
                ViewBag.HistorialWeb = historialWeb;
            }

            return View(us);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarAtajoAuto(int AutoId)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            Auto auto = flota.Autos.Where(a => a.Id == AutoId).FirstOrDefault();

            if(auto == null)
                return HttpNotFound();

            auto.Atajo = !auto.Atajo;
            db.SaveChanges();
            return Json(new { respuesta = "ok" });
        }

        #endregion


        #region ------------------------ADMIN OPERADORES----------------------------------------

        public ActionResult AdministrarOperadores()
        {
            if (Session["Email"] == null)
                return RedirectToAction("Index", "Login");
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();
            List<Operador> operadores = flota.Operadores.ToList();

            return View(operadores);
        }

        public ActionResult CrearOperador()
        {
            if (Session["Email"] == null)
                return RedirectToAction("Index", "Login");
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();



            List<Auto> autosCandidatos = flota.Autos.Where(a => a.OperadorId == null).ToList();
            autosCandidatos.Add(new Auto() { Id = 0, NombreVehiculo = "No asignar" });
            autosCandidatos.Reverse();

            List<Usuario> usuariosCandidatos = db.Usuarios.Where(u => u.OperadorId == null ).ToList();
            usuariosCandidatos.Add(new Usuario() { Id = 0, Email = "No asignar" });
            usuariosCandidatos.Reverse();

            ViewBag.UsuarioId = new SelectList(usuariosCandidatos, "Id", "Email");
            ViewBag.AutoId = new SelectList(autosCandidatos, "Id", "NombreVehiculo");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearOperador([Bind(Include = "Id,Nombre,TipoLicencia,UsuarioId,AutoId")] CreacionOperador model)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            if (flota.PackId != null)
            {
                if (flota.Operadores.Count >= flota.PackServicio.NumeroOperadores)
                {
                    TempData["msg"] = "No puede superar el número máximo de operadores. Su pack de servicios tiene un límite de " + flota.PackServicio.NumeroOperadores;
                    return RedirectToAction("AdministrarOperadores");
                }
            }

            Operador repetido = flota.Operadores.Where(a => a.Nombre == model.Nombre).FirstOrDefault();
            if (repetido != null)
            {
                ModelState.Clear();
                ModelState.AddModelError("Nombre", "Ya existe un operador en su flota con el mismo nombre.");
                TryValidateModel(model);
            }

            if (ModelState.IsValid == false)
            {
                List<Auto> autosCandidatos = flota.Autos.Where(a => a.OperadorId == null).ToList();
                autosCandidatos.Add(new Auto() { Id = 0, NombreVehiculo = "No asignar" });
                autosCandidatos.Reverse();

                List<Usuario> usuariosCandidatos = db.Usuarios.Where(u => u.OperadorId == null ).ToList();
                usuariosCandidatos.Add(new Usuario() { Id = 0, Email = "No asignar" });
                usuariosCandidatos.Reverse();

                ViewBag.UsuarioId = new SelectList(usuariosCandidatos, "Id", "Email");
                ViewBag.AutoId = new SelectList(autosCandidatos, "Id", "NombreVehiculo");
                return View("CrearOperador", model);
            }

            Operador.CrearOperador(db, idFlota, model.Nombre, model.TipoLicencia, model.UsuarioId, model.AutoId);

            return RedirectToAction("AdministrarOperadores");
        }

        public ActionResult EditarOperador(int? id)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            Operador operador = flota.Operadores.Where(o => o.Id == id).FirstOrDefault();

            if (operador == null)
            {
                return HttpNotFound();
            }

            CreacionOperador modeloOperador = new CreacionOperador()
            {
                Id = operador.Id,
                Nombre = operador.Nombre,
                TipoLicencia = operador.TipoLicencia,
                UsuarioId = 0,
                AutoId = 0
            };

            List<Usuario> usuariosCandidatos = db.Usuarios.Where(u => u.OperadorId == null ).ToList();
            List<Auto> autosCandidatos = flota.Autos.Where(a => a.OperadorId == null).ToList();

            if (operador.Usuario != null)
            {
                modeloOperador.UsuarioId = operador.Usuario.Id;
                Usuario userActual = db.Usuarios.Where(u => u.Id == operador.Usuario.Id).FirstOrDefault();
                usuariosCandidatos.Add(userActual);
            }
            usuariosCandidatos.Add(new Usuario() { Id = 0, Email = "No asignar" });
            usuariosCandidatos.Reverse();

            //if (operador.Auto != null)
            //{
            //    modeloOperador.AutoId = operador.Auto.Id;
            //    Auto autoActual = flota.Autos.Where(a => a.Id == operador.Auto.Id).FirstOrDefault();
            //    autosCandidatos.Add(autoActual);
            //}
            if(operador.Autos.Count > 0)
            {
                modeloOperador.AutoId = operador.Autos.First().Id;
                Auto autoActual = flota.Autos.Where(a => a.Id == modeloOperador.AutoId).FirstOrDefault();
                autosCandidatos.Add(autoActual);
            }

            autosCandidatos.Add(new Auto() { Id = 0, NombreVehiculo = "No asignar" });
            autosCandidatos.Reverse();

            ViewBag.UsuarioId = new SelectList(usuariosCandidatos, "Id", "Email", modeloOperador.UsuarioId);
            ViewBag.AutoId = new SelectList(autosCandidatos, "Id", "NombreVehiculo", modeloOperador.AutoId);

            return View(modeloOperador);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarOperador([Bind(Include = "Id,Nombre,TipoLicencia,UsuarioId,AutoId")] CreacionOperador model)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();
            Operador operador = flota.Operadores.Where(a => a.Id == model.Id).FirstOrDefault();

            Operador repetido = flota.Operadores.Where(a => a.Nombre == model.Nombre && a.Id != operador.Id).FirstOrDefault();
            if (repetido != null)
            {
                ModelState.Clear();
                ModelState.AddModelError("Nombre", "Ya existe un operador en su flota con el mismo nombre.");
                TryValidateModel(model);
            }

            if (ModelState.IsValid == false)
            {
                
                List<Usuario> usuariosCandidatos = db.Usuarios.Where(u => u.OperadorId == null ).ToList();
                List<Auto> autosCandidatos = flota.Autos.Where(a => a.OperadorId == null).ToList();

                if (operador.Usuario != null)
                {
                    Usuario userActual = db.Usuarios.Where(u => u.Id == operador.Usuario.Id).FirstOrDefault();
                    usuariosCandidatos.Add(userActual);
                    usuariosCandidatos.Add(new Usuario() { Id = 0, Email = "No asignar" });
                    usuariosCandidatos.Reverse();
                }

                //if (operador.Auto != null)
                //{
                //    Auto autoActual = flota.Autos.Where(a => a.Id == operador.Auto.Id).FirstOrDefault();
                //    autosCandidatos.Add(autoActual);
                //    autosCandidatos.Add(new Auto() { Id = 0, NombreVehiculo = "No asignar" });
                //    autosCandidatos.Reverse();
                //}

                if(operador.Autos.Count > 0)
                {
                    int idAuto = operador.Autos.First().Id;
                    Auto autoActual = flota.Autos.Where(a => a.Id == idAuto).FirstOrDefault();
                    autosCandidatos.Add(autoActual);
                    autosCandidatos.Add(new Auto() { Id = 0, NombreVehiculo = "No asignar" });
                    autosCandidatos.Reverse();
                }

                ViewBag.UsuarioId = new SelectList(usuariosCandidatos, "Id", "Email");
                ViewBag.AutoId = new SelectList(autosCandidatos, "Id", "NombreVehiculo");

                return View("EditarOperador", model);
            }

            Operador.EditarOperador(db, idFlota, model.Id, model.Nombre,model.TipoLicencia, model.UsuarioId, model.AutoId);

            return RedirectToAction("AdministrarOperadores");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarOperador(int id)
        {
            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            Operador.EliminarOperador(db, id);

            return RedirectToAction("AdministrarOperadores");
        }




        #endregion


        #region -------------------- ADMIN MANTENCIONES ----------------------------------------

        public ActionResult AdministrarMantenciones()
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            if (flota.PackId != null)
            {
                if (flota.PackServicio.ModuloMantencion == false)
                    return RedirectToAction("Index");
            }

            List<HistorialMantencion> historialesMantencion = new List<HistorialMantencion>();

            foreach (Auto a in flota.Autos)
            {
                historialesMantencion.AddRange(a.Mantenciones);
            }
            historialesMantencion = historialesMantencion.OrderBy(h => h.Fecha).ToList();
            historialesMantencion.Reverse();

            return View(historialesMantencion);
        }

        public ActionResult AplicarMantenimiento()
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            if (flota.PackId != null)
            {
                if (flota.PackServicio.ModuloMantencion == false)
                    return RedirectToAction("Index");
            }

            //proveedores, servicios y autos
            List<Auto> autos = flota.Autos.ToList();
            List<Servicio> servicios = flota.Servicios.ToList();
            List<Proveedor> proveedores = flota.Proveedores.ToList();
            proveedores.Add(new Proveedor() { Id = 0, NombreComercial = "No asignar" });
            proveedores.Reverse();

            ViewBag.ProveedorId = new SelectList(proveedores, "Id", "NombreComercial");
            ViewBag.ServiciosId = new SelectList(servicios, "Id", "NombreServicio");
            ViewBag.AutoId = new SelectList(autos, "Id", "NombreVehiculo");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AplicarMantenimiento([Bind(Include = "Id,Fecha,AutoId,Kilometraje,TipoDeMantenimiento,ProveedorId,ServiciosId,Costo")] CreacionMantencion model)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();
            Auto auto = flota.Autos.Where(a => a.Id == model.AutoId).FirstOrDefault();

            if(auto == null)
            {
                ModelState.Clear();
                ModelState.AddModelError("AutoId", "Debe seleccionarse un vehículo para la mantención.");
                TryValidateModel(model);
            }
            else if (auto.Mantenciones.Count > 0)
            {
                HistorialMantencion ultimaMantencion = auto.Mantenciones.OrderBy(h => h.Fecha).Last();
                DateTime fechaMantencion = model.Fecha;

                ModelState.Clear();
                if (model.Kilometraje <= ultimaMantencion.Kilometraje)
                    ModelState.AddModelError("Kilometraje", "Kilometraje inválido. Debe seguir el orden lógico de los registros. El último registro marcó: " + ultimaMantencion.Kilometraje + " KMs");
                if (fechaMantencion < ultimaMantencion.Fecha)
                    ModelState.AddModelError("Fecha", "La fecha ingresada es anterior al último registro de combustible, lo cual no es lógico. El último registro fue el: " + ultimaMantencion.Fecha);
                TryValidateModel(model);
            }

            if (ModelState.IsValid == false)
            {
                List<Auto> autos = flota.Autos.ToList();
                List<Servicio> servicios = flota.Servicios.ToList();
                List<Proveedor> proveedores = flota.Proveedores.ToList();
                proveedores.Add(new Proveedor() { Id = 0, NombreComercial = "No asignar" });
                proveedores.Reverse();

                ViewBag.ProveedorId = new SelectList(proveedores, "Id", "NombreComercial");
                ViewBag.ServiciosId = new SelectList(servicios, "Id", "NombreServicio");
                ViewBag.AutoId = new SelectList(autos, "Id", "NombreVehiculo");

                return View("AplicarMantenimiento", model);
            }

            Proveedor prov = flota.Proveedores.Where(p => p.Id == model.ProveedorId).FirstOrDefault();
            List<Servicio> serviciosHechos = new List<Servicio>();

            if (model.ServiciosId != null)
            {
                foreach (int idServicio in model.ServiciosId)
                {
                    Servicio servicio = flota.Servicios.Where(s => s.Id == idServicio).FirstOrDefault();
                    serviciosHechos.Add(servicio);
                }
            }

            HistorialMantencion.NuevoRegistroDeServicio(db, auto.Id, model.Fecha, serviciosHechos, model.Kilometraje, model.TipoDeMantenimiento, model.Costo, prov);

            return RedirectToAction("AdministrarMantenciones");
        }

        public ActionResult CatalogoServicios()
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            if (flota.PackId != null)
            {
                if (flota.PackServicio.ModuloMantencion == false)
                    return RedirectToAction("Index");
            }

            List<Servicio> servicios = flota.Servicios.ToList();

            return View(servicios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearServicio([Bind(Include = "NombreServicio")] Servicio model)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            if (ModelState.IsValid == false)
            {
                List<ModelErrorCollection> errors = ModelState.Select(x => x.Value.Errors)
                          .Where(y => y.Count > 0)
                          .ToList();

                return Json(new
                {
                    exito = false,
                    mensaje = "Debe ingresar un valor válido para el nuevo servicio."
                }, JsonRequestBehavior.AllowGet);
            }

            int idFlota = (int)Session["Flota"];
            Servicio.CrearServicio(db, idFlota, model.NombreServicio);

            return Json(new
            {
                exito = true,
                mensaje = "Creación exitosa"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarServicio(int id)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            Servicio servicio = flota.Servicios.Where(s => s.Id == id).FirstOrDefault();

            if (servicio == null)
            {
                return HttpNotFound();
            }

            Servicio.EliminarServicio(db, idFlota, servicio.Id);

            return RedirectToAction("CatalogoServicios");
        }

        public ActionResult EditarMantencion(int? id, int? autoId)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            if (id == null || autoId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();
            Auto auto = flota.Autos.Where(a => a.Id == autoId).FirstOrDefault();

            if (flota.PackId != null)
            {
                if (flota.PackServicio.ModuloMantencion == false)
                    return RedirectToAction("Index");
            }

            if (auto == null)
                return HttpNotFound();

            HistorialMantencion mantencion = auto.Mantenciones.Where(c => c.Id == id).FirstOrDefault();

            if (mantencion == null)
                return HttpNotFound();

            int provId = 0;

            if (mantencion.Proveedor != null)
                provId = mantencion.Proveedor.Id;

            CreacionMantencion modelMantencion = new CreacionMantencion()
            {
                Id = mantencion.Id,
                AutoId = mantencion.Auto.Id,
                Fecha = mantencion.Fecha,
                Kilometraje = mantencion.Kilometraje,
                TipoDeMantenimiento = mantencion.TipoDeMantenimiento,
                Costo = mantencion.Costo,
                ProveedorId = 0,
                ServiciosId = new List<int>()
            };

            if (mantencion.Proveedor != null)
                modelMantencion.ProveedorId = mantencion.Proveedor.Id;

            foreach(MantencionServicio ms in mantencion.ServiciosAplicados)
            {
                modelMantencion.ServiciosId.Add(ms.Servicio.Id);
            }
            int[] serviciosId = modelMantencion.ServiciosId.ToArray();

            List<Servicio> servicios = flota.Servicios.ToList();
            List<Proveedor> proveedores = flota.Proveedores.ToList();
            proveedores.Add(new Proveedor() { Id = 0, NombreComercial = "No asignar" });
            proveedores.Reverse();



            ViewBag.ProveedorId = new SelectList(proveedores, "Id", "NombreComercial", modelMantencion.ProveedorId);
            ViewBag.ServiciosId = new SelectList(servicios, "Id", "NombreServicio", serviciosId);

            ViewBag.VehiculoNombre = auto.NombreVehiculo;
            //NO SE EDITA EL AUTO AL QUE SE LE HACE LA MANTENCION
            return View(modelMantencion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarMantencion([Bind(Include = "Id,AutoId,Fecha,AutoId,Kilometraje,TipoDeMantenimiento,ProveedorId,ServiciosId,Costo")] CreacionMantencion model)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();
            Auto auto = flota.Autos.Where(a => a.Id == model.AutoId).FirstOrDefault();

            List<HistorialMantencion> mantenciones = auto.Mantenciones.OrderBy(hcc => hcc.Kilometraje).ToList();
            if (mantenciones.Count > 1)
            {
                HistorialMantencion mantencionActual = null;
                HistorialMantencion mantencionProxima = null;
                HistorialMantencion mantencionAnterior = null;

                for (int i = 0; i < mantenciones.Count; i++)
                {
                    if (mantenciones[i].Id == model.Id)
                    {
                        mantencionActual = mantenciones[i];

                        if ((i + 1) < mantenciones.Count)
                            mantencionProxima = mantenciones[i + 1];
                        if ((i - 1) >= 0)
                            mantencionAnterior = mantenciones[i - 1];
                    }
                }

                ModelState.Clear();

                if (mantencionProxima != null)
                {
                    if (mantencionProxima.Kilometraje <= model.Kilometraje)
                        ModelState.AddModelError("Kilometraje", "Kilometraje inválido. No debe superar los " + mantencionProxima.Kilometraje + " KMs del registro siguiente.");
                    if (mantencionProxima.Fecha <= model.Fecha)
                        ModelState.AddModelError("Fecha", "La fecha debe ser menor que el próximo registro, la cual se hizo el: " + mantencionProxima.Fecha);
                }

                if (mantencionAnterior != null)
                {
                    if (mantencionAnterior.Kilometraje >= model.Kilometraje)
                        ModelState.AddModelError("Kilometraje", "Kilometraje inválido. No debe ser inferior a los " + mantencionAnterior.Kilometraje + " KMs del registro anterior.");
                    if (mantencionAnterior.Fecha >= model.Fecha)
                        ModelState.AddModelError("Fecha", "La fecha debe ser mayor que el registro anterior, la cual se hizo el: " + mantencionAnterior.Fecha);
                }
                TryValidateModel(model);
            }

            List<Servicio> servicios = flota.Servicios.ToList();
            if (ModelState.IsValid == false)
            {
                List<Proveedor> proveedores = flota.Proveedores.ToList();
                proveedores.Add(new Proveedor() { Id = 0, NombreComercial = "No asignar" });
                proveedores.Reverse();

                ViewBag.ProveedorId = new SelectList(proveedores, "Id", "NombreComercial");
                ViewBag.ServiciosId = new SelectList(servicios, "Id", "NombreServicio");

                return View("EditarMantencion", model);
            }

            //EDITAR
            Proveedor proveedor = flota.Proveedores.Where(p => p.Id == model.ProveedorId).FirstOrDefault();

            servicios = new List<Servicio>();
            foreach(int sid in model.ServiciosId)
            {
                Servicio serv = flota.Servicios.Where(s => s.Id == sid).FirstOrDefault();
                servicios.Add(serv);
            }

            HistorialMantencion.EditarHistorialMantencion(db, auto.Id, model.Id, model.Fecha, servicios, model.Kilometraje, model.TipoDeMantenimiento, model.Costo, proveedor);

            return RedirectToAction("FichaAuto", new { id = auto.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarMantencion(int id, int autoid)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            HistorialMantencion.EliminarHistorialMantencion(db, autoid, id);

            return RedirectToAction("FichaAuto", new { id = autoid });
        }

        #endregion

        #region ----------------------- ADMIN COMBUSTIBLE -------------------------------------------


        public ActionResult HistorialesCombustible()
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            if(flota.PackId != null)
            {
                if (flota.PackServicio.ModuloCombustible == false)
                    return RedirectToAction("Index");
            }

            List<HistorialCargaCombustible> historialesCombustible = new List<HistorialCargaCombustible>();

            foreach (Auto a in flota.Autos)
            {
                historialesCombustible.AddRange(a.CargasCombustible);
            }
            historialesCombustible = historialesCombustible.OrderBy(h => h.FechaHora).ToList();
            historialesCombustible.Reverse();

            return View(historialesCombustible);
        }


        public ActionResult CargarCombustible(int? id)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            if (flota.PackId != null)
            {
                if (flota.PackServicio.ModuloCombustible == false)
                    return RedirectToAction("Index");
            }

            int idAuto = 0;
            if (id != null)
                idAuto = id.Value;

            List<Proveedor> proveedores = flota.Proveedores.ToList();
            proveedores.Add(new Proveedor() { Id = 0, NombreComercial = "No asignar" });
            proveedores.Reverse();

            ViewBag.ProveedorId = new SelectList(proveedores, "Id", "NombreComercial");

            List<Auto> autos = flota.Autos.ToList();
            ViewBag.VehiculoId = new SelectList(autos, "Id", "NombreVehiculo",idAuto);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CargarCombustible([Bind(Include = "Id,Fecha,Hora,EstanqueLleno,CantidadLitros,CostoUnitario,CostoTotal,Kilometraje,RutEstacion,NumeroDeBoleta,ProveedorId,VehiculoId")] CargaCombustible model)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();
            Auto auto = flota.Autos.Where(a => a.Id == model.VehiculoId).FirstOrDefault();

            if (auto.CargasCombustible.Count > 0)
            {
                HistorialCargaCombustible ultimaCarga = auto.CargasCombustible.OrderBy(h => h.FechaHora).Last();
                DateTime fechaCarga = new DateTime(model.Fecha.Year, model.Fecha.Month, model.Fecha.Day, model.Hora.Hour, model.Hora.Minute, model.Hora.Second);

                ModelState.Clear();
                if (model.Kilometraje <= ultimaCarga.Kilometraje)
                    ModelState.AddModelError("Kilometraje", "Kilometraje inválido. Debe seguir el orden lógico de los registros. El último registro marcó: " + ultimaCarga.Kilometraje + " KMs");
                if (fechaCarga <= ultimaCarga.FechaHora)
                    ModelState.AddModelError("Fecha", "La combinación de fecha y hora da una fecha anterior al último registro de combustible, lo cual no es lógico. El último registro fue el: " + ultimaCarga.FechaHora);
                TryValidateModel(model);
            }

            if (ModelState.IsValid == false)
            {
                List<Proveedor> proveedores = flota.Proveedores.ToList();
                proveedores.Add(new Proveedor() { Id = 0, NombreComercial = "No asignar" });
                proveedores.Reverse();

                ViewBag.ProveedorId = new SelectList(proveedores, "Id", "NombreComercial");
                List<Auto> autos = flota.Autos.ToList();
                ViewBag.VehiculoId = new SelectList(autos, "Id", "NombreVehiculo", model.VehiculoId);

                return View("CargarCombustible", model);
            }
            Proveedor prov = flota.Proveedores.Where(p => p.Id == model.ProveedorId).FirstOrDefault();
            HistorialCargaCombustible.NuevaCargaCombustible(db, model.VehiculoId, model.Fecha, model.Hora, model.EstanqueLleno, model.CantidadLitros, model.CostoUnitario, model.Kilometraje, prov,model.RutEstacion,model.NumeroDeBoleta);

            TempData["msg"] = "Carga de combustible exitosa al vehículo " + auto.NombreVehiculo;
            return RedirectToAction("HistorialesCombustible");
        }

        public ActionResult EditarCargaCombustible(int? id, int? autoId)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            if (id == null || autoId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();
            Auto auto = flota.Autos.Where(a => a.Id == autoId).FirstOrDefault();

            if (flota.PackId != null)
            {
                if (flota.PackServicio.ModuloCombustible == false)
                    return RedirectToAction("Index");
            }

            if (auto == null)
                return HttpNotFound();

            HistorialCargaCombustible carga = auto.CargasCombustible.Where(c => c.Id == id).FirstOrDefault();

            if (carga == null)
                return HttpNotFound();

            int provId = 0;

            if (carga.Proveedor != null)
                provId = carga.Proveedor.Id;

            CargaCombustible modeloCarga = new CargaCombustible()
            {
                Id = carga.Id,
                Fecha = carga.FechaHora.Date,
                Hora = carga.FechaHora,
                EstanqueLleno = carga.EstanqueLleno,
                CantidadLitros = carga.CantidadLitros,
                CostoUnitario = carga.CostoUnitario,
                Kilometraje = carga.Kilometraje,
                ProveedorId = provId,
                VehiculoId = auto.Id,
                VehículoNombre = auto.NombreVehiculo,
                RutEstacion = carga.RutEstacion,
                NumeroDeBoleta = carga.NumeroDeBoleta
            };

            List<Proveedor> proveedores = flota.Proveedores.ToList();
            proveedores.Add(new Proveedor() { Id = 0, NombreComercial = "No asignar" });
            proveedores.Reverse();

            ViewBag.ProveedorId = new SelectList(proveedores, "Id", "NombreComercial", provId);

            return View(modeloCarga);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCargaCombustible([Bind(Include = "Id,Fecha,Hora,EstanqueLleno,CantidadLitros,CostoUnitario,CostoTotal,Kilometraje,RutEstacion,NumeroDeBoleta,ProveedorId,VehiculoId")] CargaCombustible model)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();
            Auto auto = flota.Autos.Where(a => a.Id == model.VehiculoId).FirstOrDefault();

            List<HistorialCargaCombustible> cargasCombustible = auto.CargasCombustible.OrderBy(hcc => hcc.Kilometraje).ToList();
            if (cargasCombustible.Count > 1)
            {
                HistorialCargaCombustible cargaActual = null;
                HistorialCargaCombustible cargaProxima = null;
                HistorialCargaCombustible cargaAnterior = null;

                for (int i = 0; i < cargasCombustible.Count; i++)
                {
                    if (cargasCombustible[i].Id == model.Id)
                    {
                        cargaActual = cargasCombustible[i];

                        if ((i + 1) < cargasCombustible.Count)
                            cargaProxima = cargasCombustible[i + 1];
                        if ((i - 1) >= 0)
                            cargaAnterior = cargasCombustible[i - 1];
                    }
                }
                DateTime fechaCarga = new DateTime(model.Fecha.Year, model.Fecha.Month, model.Fecha.Day, model.Hora.Hour, model.Hora.Minute, model.Hora.Second);

                ModelState.Clear();

                if (cargaProxima != null)
                {
                    if (cargaProxima.Kilometraje <= model.Kilometraje)
                        ModelState.AddModelError("Kilometraje", "Kilometraje inválido. No debe superar los " + cargaProxima.Kilometraje + " KMs del registro siguiente.");
                    if (cargaProxima.FechaHora <= fechaCarga)
                        ModelState.AddModelError("Fecha", "La combinación de fecha y hora debe ser menor que el próximo registro, la cual se hizo el: " + cargaProxima.FechaHora);
                }

                if (cargaAnterior != null)
                {
                    if (cargaAnterior.Kilometraje >= model.Kilometraje)
                        ModelState.AddModelError("Kilometraje", "Kilometraje inválido. No debe ser inferior a los " + cargaAnterior.Kilometraje + " KMs del registro anterior.");
                    if (cargaAnterior.FechaHora >= fechaCarga)
                        ModelState.AddModelError("Fecha", "La combinación de fecha y hora debe ser mayor que el registro anterior, la cual se hizo el: " + cargaAnterior.FechaHora);
                }
                TryValidateModel(model);
            }

            if (ModelState.IsValid == false)
            {
                List<Proveedor> proveedores = flota.Proveedores.ToList();
                proveedores.Add(new Proveedor() { Id = 0, NombreComercial = "No asignar" });
                proveedores.Reverse();

                ViewBag.ProveedorId = new SelectList(proveedores, "Id", "NombreComercial");

                return View("EditarCargaCombustible", model);
            }

            //EDITAR
            Proveedor proveedor = flota.Proveedores.Where(p => p.Id == model.ProveedorId).FirstOrDefault();
            HistorialCargaCombustible.EditarCargaCombustible(db, auto.Id, model.Id, model.Fecha, model.Hora, model.EstanqueLleno, model.CantidadLitros, model.CostoUnitario, model.Kilometraje, proveedor,model.RutEstacion,model.NumeroDeBoleta);

            return RedirectToAction("FichaAuto", new { id = auto.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarCargaCombustible(int id, int autoid)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            HistorialCargaCombustible.EliminarCargaCombustible(db, autoid, id);

            return RedirectToAction("FichaAuto", new { id = autoid });
        }



        #endregion

        #region --------------------ADMIN PROVEEDORES --------------------------------------------


        public ActionResult AdministrarProveedores()
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            if (flota.PackId != null)
            {
                if (flota.PackServicio.ModuloMantencion == false && flota.PackServicio.ModuloCombustible == false)
                    return RedirectToAction("Index");
            }

            List<Proveedor> proveedores = flota.Proveedores.ToList();

            return View(proveedores);
        }

        public ActionResult CrearProveedor()
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearProveedor([Bind(Include =
            "Id,NombreComercial,Telefono,Direccion,PersonaContacto,TelefonoContacto,EmailContacto")] Proveedor model)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            if (ModelState.IsValid == false)
            {
                return View("CrearProveedor", model);
            }
            int idFlota = (int)Session["Flota"];


            Proveedor.CrearProveedor(db, idFlota, model.NombreComercial, model.Telefono, model.Direccion, model.PersonaContacto, model.TelefonoContacto, model.EmailContacto);

            return RedirectToAction("AdministrarProveedores");
        }

        public ActionResult EditarProveedor(int? id)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();
            Proveedor proveedor = flota.Proveedores.Where(p => p.Id == id).FirstOrDefault();

            if (flota.PackId != null)
            {
                if (flota.PackServicio.ModuloMantencion == false && flota.PackServicio.ModuloCombustible == false)
                    return RedirectToAction("Index");
            }

            if (proveedor == null)
            {
                return HttpNotFound();
            }

            return View(proveedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProveedor([Bind(Include =
            "Id,NombreComercial,Telefono,Direccion,PersonaContacto,TelefonoContacto,EmailContacto")] Proveedor model)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];

            if (ModelState.IsValid == false)
            {
                return View("EditarProveedor", model);
            }

            Proveedor.EditarProveedor(db, model.Id, model.NombreComercial, model.Telefono, model.Direccion, model.PersonaContacto, model.TelefonoContacto, model.EmailContacto);
            return RedirectToAction("AdministrarProveedores");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarProveedor(int id)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];

            Proveedor.EliminarProveedor(db, idFlota, id);

            return RedirectToAction("AdministrarProveedores");
        }

        public ActionResult FichaProveedor(int? id)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();
            Proveedor proveedor = flota.Proveedores.Where(p => p.Id == id).FirstOrDefault();

            if (flota.PackId != null)
            {
                if (flota.PackServicio.ModuloMantencion == false && flota.PackServicio.ModuloCombustible == false)
                    return RedirectToAction("Index");
            }

            if (proveedor == null)
            {
                return HttpNotFound();
            }

            return View(proveedor);
        }

        #endregion


        public ActionResult GraficosCombustible(int? year)
        {
            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            int idFlota = (int)Session["Flota"];
            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            if (flota.PackId != null)
            {
                if (flota.PackServicio.ModuloCombustible == false)
                    return RedirectToAction("Index");
            }

            int año = DateTime.Today.Year;

            if (year != null)
                año = year.Value;

            List<Auto> autos = flota.Autos;
            List<HistorialCargaCombustible> historiales = new List<HistorialCargaCombustible>();
            List<Operador> operadores = new List<Operador>();
            List<Proveedor> proveedores = new List<Proveedor>();

            foreach(Auto a in autos)
            {
                foreach(HistorialCargaCombustible hcc in a.CargasCombustible)
                {
                    if(hcc.Proveedor != null)
                    {
                        if (proveedores.Contains(hcc.Proveedor) == false)
                            proveedores.Add(hcc.Proveedor);
                    }
                    if(hcc.Operador != null)
                    {
                        if (operadores.Contains(hcc.Operador) == false)
                            operadores.Add(hcc.Operador);
                    }

                    historiales.Add(hcc);
                }
            }
            historiales = historiales.OrderBy(h => h.FechaHora).ToList();

            GraficosCombustible modelo = new GraficosCombustible();
            modelo.Historiales = historiales;
            modelo.Proveedores = proveedores;
            modelo.Operadores = operadores;
            modelo.Vehiculos = autos;
            modelo.Year = año;

            return View(modelo);
        }


        public ActionResult SinFlota()
        {
            return View();
        }

        [HttpGet]
        public ActionResult getPosicionesFlota(int _idFlota)
        {
            List<Auto> autos = db.Flotas.Where(f => f.Id == _idFlota).FirstOrDefault().Autos.ToList();

            //List<Operador> operadores = db.Flotas.Where(f => f.Id == _idFlota).FirstOrDefault().Operadores.ToList();
            //List<Auto> autos = new List<Auto>();

            //foreach (Operador o in operadores)
            //{
            //    if (o.Auto != null)
            //        autos.Add(o.Auto);
            //}
            List<object> datos = new List<object>();
            foreach(Auto a in autos)
            {
                string nombreOperador = "-";
                if (a.OperadorId != null)
                    nombreOperador = a.Operador.Nombre;

                datos.Add(new { Id = a.Id, Nombre = a.NombreVehiculo, Patente = a.Patente, Latitud = a.Latitud, Longitud = a.Longitud, Operador = nombreOperador });
            }


            //var datos = autos.Select(c => new { Id = c.Id, Patente = c.Patente, Latitud = c.Latitud, Longitud = c.Longitud }).ToList();

            System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string sJSON = oSerializer.Serialize(datos);

            return Json(new { respuesta = sJSON }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HistorialFlota()
        {
            //retorna lista de historiales web de esa flota del dia de hoy??

            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            DateTime fecha = DateTime.Today;
            int idFlota = (int)Session["Flota"];

            List<HistorialWeb> historialesFlota = HistorialesManager.ObtenerHistorialesFlota(db, fecha, idFlota);


            return View(historialesFlota);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HistorialFlota(string txtFecha)
        {
            //recibe nueva fecha y retorna lista correspondiente
            txtFecha += " 00:00:00";
            txtFecha = txtFecha.Replace('-', '/');

            if (Session["Flota"] == null)
                return RedirectToAction("SinFlota");

            DateTime fecha;
            bool result = DateTime.TryParseExact(txtFecha, FormatoFecha.formato, FormatoFecha.provider, DateTimeStyles.None, out fecha);
            int idFlota = (int)Session["Flota"];

            List<HistorialWeb> historialesFlota = HistorialesManager.ObtenerHistorialesFlota(db, fecha, idFlota);
            return View(historialesFlota);

        }

        [HttpGet]
        public ActionResult getHistorialFlotaFiltrado(string nombreFlota, string horaDesde, string horaHasta)
        {
            horaDesde = horaDesde.Replace('-', '/');
            horaHasta = horaHasta.Replace('-', '/');

            string formato = "d/M/yyyy H:m:ss";

            DateTime desde;
            bool resultDesde = DateTime.TryParseExact(horaDesde, formato, FormatoFecha.provider, DateTimeStyles.None, out desde);

            DateTime hasta;
            bool resultHasta = DateTime.TryParseExact(horaHasta, formato, FormatoFecha.provider, DateTimeStyles.None, out hasta);

            Flota flota = db.Flotas.Where(f => f.Nombre == nombreFlota).FirstOrDefault();

            List<HistorialWeb> historialesWeb = new List<HistorialWeb>();

            //List<Usuario> usuarios = db.Flotas.Where(f => f.Nombre == nombreFlota).FirstOrDefault().UsuariosMiembros.ToList();
            //List<Auto> autos = new List<Auto>();

            //foreach (Usuario u in usuarios)
            //{
            //    if (u.UsuarioAutoId != null)
            //        autos.Add(u.UsuarioAuto.Auto);
            //}

            //foreach(Auto a in autos)
            //{
            //    //
            //}


            //foreach (HistorialWeb hw in historialesFlota)
            //{
            //    hw.historialesPosicion = HistorialesManager.FiltrarHistorialPosicionPorHoras(hw.historialesPosicion, desde, hasta).OrderBy(h => h.FechaHora).ToList();
            //    hw.historialesVelocidad = HistorialesManager.FiltrarHistorialVelocidadPorHoras(hw.historialesVelocidad, desde, hasta).OrderBy(h => h.FechaHora).ToList();
            //    hw.historialesEnergia = HistorialesManager.FiltrarHistorialEnergiaPorHoras(hw.historialesEnergia, desde, hasta).OrderBy(h => h.FechaHora).ToList();

            //}


            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(historialesFlota);

            //return Json(new { respuesta = sJSON }, JsonRequestBehavior.AllowGet);

            return null;
        }
    }
}