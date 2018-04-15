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

namespace AEOnline.Controllers.web
{
    public class SuperAdminController : Controller
    {
        private ProyectoAutoContext db = new ProyectoAutoContext();

        List<Auto> autos;
        List<Flota> flotas;
        List<Usuario> choferes;
        List<Usuario> adminFlotas;

        void RellenarTodosAutos()
        {
            Auto autoNulo = new Auto() { Patente = "Sin Asignar" };
            autos = new List<Auto>();
            autos.Add(autoNulo);
            foreach (Auto a in db.Autos)
                    autos.Add(a);              
        }

        void RellenarAutosSinChofer(string _patenteMio)
        {
            Auto autoNulo = new Auto() { Patente = "Sin Asignar" };
            autos = new List<Auto>();
            autos.Add(autoNulo);

            foreach(Auto a in db.Autos)
            {
                if(a.OperadorId == null || a.Patente == _patenteMio)
                {
                    autos.Add(a);
                }

            }
        }

        void RellenarTodasFlotas()
        {
            Flota flotaNula = new Flota() { Nombre = "Sin Asignar" };
            flotas = new List<Flota>();
            flotas.Add(flotaNula);
            foreach (Flota f in db.Flotas)
            {
                flotas.Add(f);
            }         
        }

        void RellenarFlotasSinAdmin(string _nombreMio = "")
        {
            Flota flotaNula = new Flota() { Nombre = "Sin Asignar" };
            flotas = new List<Flota>();
            flotas.Add(flotaNula);
            foreach (Flota f in db.Flotas)
            {
                if(_nombreMio != "")
                {
                    if (f.UsuarioFlotaId == null || f.Nombre == _nombreMio)
                        flotas.Add(f);
                }
                else
                {
                    if(f.UsuarioFlotaId == null)
                        flotas.Add(f);
                }
                   
            }

        }

        void RellenarTodosChoferes()
        {
            Usuario choferNulo = new Usuario() { Nombre = "Sin Asignar" };
            choferes = new List<Usuario>();
            choferes.Add(choferNulo);
            foreach (Usuario u in db.Usuarios)
            {
                if (u.Rol == Usuario.RolUsuario.Normal && u.OperadorId == null)
                        choferes.Add(u);
            }
        }

        void RellenarChoferesSinAuto(string _nombreMio)
        {
            Usuario choferNulo = new Usuario() { Nombre = "Sin Asignar" };
            choferes = new List<Usuario>();
            choferes.Add(choferNulo);
            foreach (Usuario u in db.Usuarios)
            {
                if ((u.Rol == Usuario.RolUsuario.Normal && u.OperadorId == null)
                    || u.Nombre ==_nombreMio)
                {
                    choferes.Add(u);
                }
                    
            }
        }

        void RellenarTodosAdminFlotas()
        {
            Usuario adminNulo = new Usuario() { Nombre = "Sin Asignar" };
            adminFlotas = new List<Usuario>();
            adminFlotas.Add(adminNulo);
            foreach(Usuario u in db.Usuarios)
            {
                if (u.Rol == Usuario.RolUsuario.AdminDeFlota && u.UsuarioFlotaId == null)
                    adminFlotas.Add(u);
            }
        }

        void RellenarAdminsSinFlota(int _idMio = 0)
        {
            Usuario adminNulo = new Usuario() { Nombre = "Sin Asignar" };
            adminFlotas = new List<Usuario>();
            adminFlotas.Add(adminNulo);
            foreach (Usuario u in db.Usuarios)
            {
                if(_idMio != 0)
                {
                    if ((u.Rol == Usuario.RolUsuario.AdminDeFlota &&u.UsuarioFlotaId == null) 
                        || u.Id == _idMio)
                        adminFlotas.Add(u);
                }
                else
                {
                    if (u.Rol == Usuario.RolUsuario.AdminDeFlota && u.UsuarioFlotaId == null)
                        adminFlotas.Add(u);
                }

            }
        }


        void ReparacionBaseDeDatos()
        {
            List<Auto> autos = db.Autos.ToList();
            List<Usuario> usuarios = db.Usuarios.ToList();

            foreach(Auto a in autos)
            {
                if(a.Operador != null)
                {
                    a.OperadorId = a.Operador.Id;

                    a.Operador.Autos.Clear();
                    a.Operador.Autos.Add(a);

                }
            }
            foreach(Usuario u in usuarios)
            {
                if(u.Operador != null)
                {
                    u.OperadorId = u.Operador.Id;
                    u.Operador.Usuario = u;
                }
            }

            db.SaveChanges();
        }


        // GET: SuperAdmin
        public ActionResult Index()
        {
            //ReparacionBaseDeDatos();
            return View();
        }

        #region ------------ADMINISTRACION DE USUARIOS------------


        // GET: SuperAdmin/AdminUsuarios
        public ActionResult AdminUsuarios(string SortOrder, string SearchString)
        {
            List<Usuario> usuarios = db.Usuarios.ToList();

            if(String.IsNullOrEmpty(SearchString) == false)
            {
                SearchString = SearchString.ToUpper();
                List<Usuario> filtrados = new List<Usuario>();

                for (int i = 0; i < usuarios.Count; i++)
                {
                    if (usuarios[i].Nombre.ToUpper().Contains(SearchString) || usuarios[i].Email.ToUpper().Contains(SearchString))
                        filtrados.Add(usuarios[i]);
                }

                usuarios = filtrados;
            }

            if (String.IsNullOrEmpty(SortOrder))
            {
                ViewBag.OrdenNombre = "OrdenNombreDes";
                ViewBag.OrdenEmail = "OrdenEmailDes";
            }
            else
            {
                #region Orden Nombres
                if (SortOrder == "OrdenNombreDes")
                {
                    usuarios = usuarios.OrderBy(u => u.Nombre).ToList();
                    ViewBag.OrdenNombre = "OrdenNombreAs";
                }
                else if (SortOrder == "OrdenNombreAs")
                {
                    usuarios = usuarios.OrderBy(u => u.Nombre).Reverse().ToList();
                    ViewBag.OrdenNombre = "OrdenNombreDes";
                }
                #endregion

                #region Orden Email

                if (SortOrder == "OrdenEmailDes")
                {
                    usuarios = usuarios.OrderBy(u => u.Email).ToList();
                    ViewBag.OrdenNombre = "OrdenEmailAs";
                }
                else if (SortOrder == "OrdenEmailAs")
                {
                    usuarios = usuarios.OrderBy(u => u.Email).Reverse().ToList();
                    ViewBag.OrdenNombre = "OrdenEmailDes";
                }

                #endregion
            }


            return View(usuarios);
        }

        // GET: SuperAdmin/CrearUsuario
        public ActionResult CrearUsuario()
        {
            RellenarFlotasSinAdmin();
            ViewBag.FlotaId = new SelectList(flotas, "Id", "Nombre");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearUsuario([Bind(Include = "Id,Nombre,Rol,Password,ConfirmPassword,Email,AutoPatente,FlotaNombre,FlotaId")] CreacionUsuario model)
        {
            string pass = "";
            string confirmpass = "";

            if (model.Password != null)
                pass = model.Password.Trim();
            if (model.ConfirmPassword != null)
                confirmpass = model.ConfirmPassword.Trim();

            if (pass == "" || confirmpass == "")
            {
                ModelState.Clear();
                if (pass == "")
                    ModelState.AddModelError("Password", "Campo password es obligatorio.");
                if (confirmpass == "")
                    ModelState.AddModelError("ConfirmPassword", "Campo confirmar password es obligatorio.");

                TryValidateModel(model);
            }

            if (ModelState.IsValid == false)
            {
                RellenarFlotasSinAdmin();
                ViewBag.FlotaId = new SelectList(flotas, "Id", "Nombre");
                return View("CrearUsuario", model);
            }



            bool repetido = Usuario.VerificarRepetido(model.Email);

            if (repetido)
            {
                ModelState.AddModelError("Email", "Ese correo ya existe.");

                RellenarFlotasSinAdmin();
                ViewBag.FlotaId = new SelectList(flotas, "Id", "Nombre");
                return View("CrearUsuario", model);
            }

            int flotaId = model.FlotaId;
            Usuario.CrearUsuario(db, model.Nombre,model.Email,model.Password,model.Rol,model.FlotaId);

            return RedirectToAction("AdminUsuarios");
        }


        // GET: SuperAdmin/EditarUsuario
        public ActionResult EditarUsuario(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }


            CreacionUsuario modeloUser = new CreacionUsuario()
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol,
                AutoPatente = "",
                FlotaNombre = "",
                FlotaId = 0
            };


            if(usuario.UsuarioFlota != null)
            {
                modeloUser.FlotaNombre = usuario.UsuarioFlota.Flota.Nombre;
                modeloUser.FlotaId = usuario.UsuarioFlota.Flota.Id;
            }

            RellenarFlotasSinAdmin(modeloUser.FlotaNombre);
            SelectList selectFlotas = new SelectList(flotas, "Id", "Nombre", modeloUser.FlotaId);

            ViewBag.FlotaId = selectFlotas;
            return View(modeloUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarUsuario([Bind(Include = "Id,Nombre,Rol,Email,AutoPatente,Password,ConfirmPassword,FlotaNombre,FlotaId")] CreacionUsuario model)
        {
            string pass = "";
            string confirmpass = "";

            if (model.Password != null)
                pass = model.Password.Trim();
            if (model.ConfirmPassword != null)
                confirmpass = model.ConfirmPassword.Trim();


            if (ModelState.IsValid == false)
            {
                RellenarFlotasSinAdmin(model.FlotaNombre);

                SelectList selectFlotas = new SelectList(flotas, "Id", "Nombre", model.FlotaId);
                ViewBag.FlotaId = selectFlotas;
                return View("EditarUsuario", model);
            }

            Usuario userOriginal = db.Usuarios.Where(u => u.Id == model.Id).FirstOrDefault();
            bool repetido = Usuario.VerificarRepetido(model.Email, userOriginal.Email);

            if (repetido)
            {
                ModelState.AddModelError("Email", "Ese correo ya existe.");

                RellenarFlotasSinAdmin(model.FlotaNombre);
                SelectList selectFlotas = new SelectList(flotas, "Id", "Nombre", model.FlotaId);
                ViewBag.FlotaId = selectFlotas;
                return View("EditarUsuario", model);
            }

            //No borrar ultimo superAdmin
            if(userOriginal.Rol == Usuario.RolUsuario.SuperAdmin && model.Rol != Usuario.RolUsuario.SuperAdmin
                && db.Usuarios.Where(u => u.Rol == Usuario.RolUsuario.SuperAdmin).ToList().Count == 1)
            {
                ModelState.AddModelError("Rol", "No puede remover este rol, porque no quedarían usuarios SuperAdmin en el sistema.");

                RellenarFlotasSinAdmin(model.FlotaNombre);
                SelectList selectFlotas = new SelectList(flotas, "Id", "Nombre", model.FlotaId);
                ViewBag.FlotaId = selectFlotas;

                return View("EditarUsuario", model);
            }


            Usuario.EditarUsuario(db, userOriginal.Id, model.Nombre, model.Email, model.Rol, model.FlotaId);

            Usuario.EditarPasswordUsuario(db, userOriginal.Id, pass);

            return RedirectToAction("AdminUsuarios");

        }


        public ActionResult EliminarUsuario(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarUsuario(int id)
        {
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            Usuario.EliminarUsuario(db, id);

            return RedirectToAction("AdminUsuarios");
        }


        #endregion

        #region ------------ADMINISTRACION DE AUTOS---------------

        // GET: SuperAdmin/AdminAutos
        public ActionResult AdminAutos()
        {
            List<Auto> a = db.Autos.ToList();
            return View(a);
        }

        // GET: SuperAdmin/CrearAuto
        public ActionResult CrearAuto()
        {
            RellenarTodasFlotas();
            ViewBag.FlotaId = new SelectList(flotas, "Id", "Nombre");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearAuto([Bind(Include =
            "Id,NombreVehiculo,Marca,Modelo,Year,Patente,KilometrajeActual,TipoVehiculoId,FlotaId")] CreacionAuto model)
        {
            if (ModelState.IsValid == false)
            {
                RellenarTodasFlotas();
                ViewBag.FlotaId = new SelectList(flotas, "Id", "Nombre");
                return View("CrearAuto", model);
            }

            Auto.CrearAuto(db, model.NombreVehiculo, model.Patente, null, model.Marca, model.Modelo, model.Year, model.KilometrajeActual,  model.FlotaId);

            return RedirectToAction("AdminAutos");
        }

        // GET: SuperAdmin/EditarAuto/0
        public ActionResult EditarAuto(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Auto auto = db.Autos.Find(id);

            if (auto == null)
            {
                return HttpNotFound();
            }

            CreacionAuto modeloAuto = new CreacionAuto()
            {
                Id = auto.Id,
                NombreVehiculo = auto.NombreVehiculo,
                Patente = auto.Patente,
                Marca = auto.Marca,
                Modelo = auto.Modelo,
                Year = auto.Year,
                KilometrajeActual = auto.KilometrajeActual,
                FlotaId = 0
            };

            if (auto.Flota != null)
            {
                modeloAuto.FlotaId = auto.Flota.Id;
            }
                

            RellenarTodasFlotas();
            SelectList selectFlotas = new SelectList(flotas, "Id", "Nombre", modeloAuto.FlotaId);
            ViewBag.FlotaId = selectFlotas;
            return View(modeloAuto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAuto([Bind(Include = 
            "Id,NombreVehiculo,Marca,Modelo,Year,Patente,KilometrajeActual,TipoVehiculoId,FlotaId")] CreacionAuto model)
        {
            if (ModelState.IsValid == false)
            {
                RellenarTodasFlotas();
                SelectList selectFlotas = new SelectList(flotas, "Id", "Nombre", model.FlotaId);
                ViewBag.FlotaId = selectFlotas;
                return View("EditarAuto", model);
            }

            Auto.EditarAuto(db, model.Id, model.NombreVehiculo, model.Patente, null, model.Marca, model.Modelo, model.Year, model.KilometrajeActual, model.FlotaId);

            return RedirectToAction("AdminAutos");
        }


        public ActionResult EliminarAuto(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Auto auto = db.Autos.Find(id);
            if (auto == null)
            {
                return HttpNotFound();
            }

            return View(auto);
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

            return RedirectToAction("AdminAutos");
        }

        public ActionResult RegistrosCelular(int? id, int? tipo) //tipo 0: chofer, tipo 1: auto
        {

            if (id == null || tipo == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Auto auto = null;

            if (tipo == 0)
            {
                Operador operador = db.Operadores.Where(o => o.Id == id).FirstOrDefault();

                if (operador == null)
                    return HttpNotFound();

                //if (operador.Auto != null)
                //    auto = operador.Auto;

                if (operador.Autos.Count > 0)
                    auto = operador.Autos.First();
            }
            else if (tipo == 1)
            {
                auto = db.Autos.Where(u => u.Id == id).FirstOrDefault();
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

        #endregion

        #region ------------ADMINISTRACION DE FLOTAS---------------

        // GET: SuperAdmin/AdminFlotas
        public ActionResult AdminFlotas()
        {
            List<Flota> f = db.Flotas.ToList();

            return View(f);
        }

        public ActionResult CrearFlota()
        {
            RellenarTodosAdminFlotas();
            ViewBag.AdminId = new SelectList(adminFlotas, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearFlota([Bind(Include = "Id,Nombre,AdminNombre,AdminId")] CreacionFlota model)
        {
            if (ModelState.IsValid == false)
            {
                RellenarTodosAdminFlotas();
                ViewBag.AdminId = new SelectList(adminFlotas, "Id", "Nombre");
                return View("CrearFlota", model);
            }

            Flota nombreRepetido = db.Flotas.Where(f => f.Nombre == model.Nombre).FirstOrDefault();
            if(nombreRepetido != null)
            {
                ModelState.AddModelError("Nombre", "Ese nombre ya existe.");
                RellenarTodosAdminFlotas();
                ViewBag.AdminId = new SelectList(adminFlotas, "Id", "Nombre");
                return View("CrearFlota", model);
            }

            
            Flota.CrearFlota(db, model.Nombre, model.AdminId);

            return RedirectToAction("AdminFlotas");
        }


        public ActionResult EditarFlota(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Flota flota = db.Flotas.Find(id);

            if (flota == null)
            {
                return HttpNotFound();
            }

            CreacionFlota modeloFlota = new CreacionFlota()
            {
                Id = flota.Id,
                Nombre = flota.Nombre,
                AdminNombre = "",
                AdminId = 0,
                PackServicioId = 0
            };

            if (flota.UsuarioFlotaId != null)
            {
                modeloFlota.AdminNombre = flota.UsuarioFlota.Usuario.Nombre;
                modeloFlota.AdminId = flota.UsuarioFlota.Usuario.Id;
            }
               
            if(flota.PackId != null)
            {
                modeloFlota.PackServicioId = flota.PackServicio.Id;
            }

            RellenarAdminsSinFlota(modeloFlota.AdminId);
            SelectList selectAdmins = new SelectList(adminFlotas, "Id", "Nombre", modeloFlota.AdminId);
            SelectList packs = new SelectList(db.PackServicios.ToList(), "Id", "Nombre", modeloFlota.PackServicioId);

            ViewBag.AdminId = selectAdmins;
            ViewBag.PackServicioId = packs;

            return View(modeloFlota);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarFlota([Bind(Include = "Id,Nombre,AdminNombre,PackServicioId,AdminId,")] CreacionFlota model)
        {
            if (ModelState.IsValid == false)
            {
                RellenarAdminsSinFlota(model.AdminId);
                SelectList selectAdmins = new SelectList(adminFlotas, "Id", "Nombre", model.AdminId);
                SelectList packs = new SelectList(db.PackServicios.ToList(), "Id", "Nombre", model.PackServicioId);

                ViewBag.AdminId = selectAdmins;
                ViewBag.PackServicioId = packs;
                return View("EditarFlota", model);
            }

            Flota flotaOriginal = db.Flotas.Where(f => f.Id == model.Id).FirstOrDefault();
            Flota nombreRepetido = db.Flotas.Where(f => f.Nombre == model.Nombre).FirstOrDefault();

            if(nombreRepetido != null && flotaOriginal.Nombre != model.Nombre)
            {
                ModelState.AddModelError("Nombre", "Ese nombre ya existe.");

                RellenarAdminsSinFlota(model.AdminId);
                SelectList selectAdmins = new SelectList(adminFlotas, "Id", "Nombre", model.AdminId);
                SelectList packs = new SelectList(db.PackServicios.ToList(), "Id", "Nombre", model.PackServicioId);

                ViewBag.AdminId = selectAdmins;
                ViewBag.PackServicioId = packs;
                return View("EditarFlota", model);
            }

            Flota.EditarFlota(db, flotaOriginal.Id, model.Nombre, model.AdminId,model.PackServicioId);

            return RedirectToAction("AdminFlotas");
        }

        public ActionResult EliminarFlota(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Flota flota = db.Flotas.Where(f => f.Id == id).FirstOrDefault();
            if (flota == null)
            {
                return HttpNotFound();
            }

            return View(flota);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarFlota(int id)
        {
            Flota flota = db.Flotas.Where(f => f.Id == id).FirstOrDefault();
            if (flota == null)
            {
                return HttpNotFound();
            }

            Flota.EliminarFlota(db, id);

            return RedirectToAction("AdminFlotas");
        }

#region -------OPERADORES DE FLOTA ----------------

        public ActionResult OperadoresFlota(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Flota flota = db.Flotas.Where(f => f.Id == id).FirstOrDefault();

            if(flota == null)
                return HttpNotFound();

            List<Operador> miembros = flota.Operadores.ToList();

            ViewBag.NombreFlota = flota.Nombre;
            ViewBag.FlotaId = flota.Id;
            return View(miembros);
        }

        public ActionResult CrearOperadorEnFlota(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Flota flota = db.Flotas.Where(f => f.Id == id).FirstOrDefault();
            if (flota == null)
            {
                return HttpNotFound();
            }

            List<Auto> autosCandidatos = flota.Autos.Where(a => a.OperadorId == null).ToList();
            autosCandidatos.Add(new Auto() { Id = 0, NombreVehiculo = "No asignar" });
            autosCandidatos.Reverse();

            List<Usuario> usuariosCandidatos = db.Usuarios.Where(u => u.OperadorId == null && u.Rol == Usuario.RolUsuario.Normal).ToList();
            usuariosCandidatos.Add(new Usuario() { Id = 0, Email = "No asignar" });
            usuariosCandidatos.Reverse();

            ViewBag.UsuarioId = new SelectList(usuariosCandidatos, "Id", "Email");
            ViewBag.AutoId = new SelectList(autosCandidatos, "Id", "NombreVehiculo");

            ViewBag.FlotaId = flota.Id;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearOperadorEnFlota([Bind(Include = "Id,Nombre,TipoLicencia,UsuarioId,AutoId,FlotaId")] CreacionOperador model)
        {
            if (ModelState.IsValid == false)
            {
                Flota flota = db.Flotas.Where(f => f.Id == model.FlotaId).FirstOrDefault();

                List<Auto> autosCandidatos = flota.Autos.Where(a => a.OperadorId == null).ToList();
                autosCandidatos.Add(new Auto() { Id = 0, NombreVehiculo = "No asignar" });
                autosCandidatos.Reverse();

                List<Usuario> usuariosCandidatos = db.Usuarios.Where(u => u.OperadorId == null && u.Rol == Usuario.RolUsuario.Normal).ToList();
                usuariosCandidatos.Add(new Usuario() { Id = 0, Email = "No asignar" });
                usuariosCandidatos.Reverse();

                ViewBag.UsuarioId = new SelectList(usuariosCandidatos, "Id", "Email");
                ViewBag.AutoId = new SelectList(autosCandidatos, "Id", "NombreVehiculo");

                ViewBag.FlotaId = flota.Id;
                return View("CrearOperadorEnFlota", model);
            }

            Operador.CrearOperador(db, model.FlotaId, model.Nombre, model.TipoLicencia, model.UsuarioId, model.AutoId);

            return RedirectToAction("OperadoresFlota", new { id = model.FlotaId });
        }

        public ActionResult EditarOperadorDeFlota(int? id, int? idFlota)
        {
            if (id == null || idFlota == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            if (flota == null)
            {
                return HttpNotFound();
            }
            Operador operador = flota.Operadores.Where(o => o.Id == id).FirstOrDefault();

            if (operador == null)
            {
                return HttpNotFound();
            }

            CreacionOperador modeloOperador = new CreacionOperador()
            {
                Id = operador.Id,
                Nombre = operador.Nombre,
                FlotaId = flota.Id,
                UsuarioId = 0,
                AutoId = 0
            };

            List<Usuario> usuariosCandidatos = db.Usuarios.Where(u => u.OperadorId == null && u.Rol == Usuario.RolUsuario.Normal).ToList();
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
            if (operador.Autos.Count > 0)
            {
                modeloOperador.AutoId = operador.Autos.First().Id;
                Auto autoActual = flota.Autos.Where(a => a.Id == modeloOperador.AutoId).FirstOrDefault();
                autosCandidatos.Add(autoActual);
            }
            autosCandidatos.Add(new Auto() { Id = 0, NombreVehiculo = "No asignar" });
            autosCandidatos.Reverse();

            ViewBag.UsuarioId = new SelectList(usuariosCandidatos, "Id", "Email", modeloOperador.UsuarioId);
            ViewBag.AutoId = new SelectList(autosCandidatos, "Id", "NombreVehiculo", modeloOperador.AutoId);

            ViewBag.FlotaId = flota.Id;
            return View(modeloOperador);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarOperadorDeFlota([Bind(Include = "Id,Nombre,TipoLicencia,UsuarioId,AutoId,FlotaId")] CreacionOperador model)
        {
            Flota flota = db.Flotas.Where(f => f.Id == model.FlotaId).FirstOrDefault();

            if (ModelState.IsValid == false)
            {
                Operador operador = flota.Operadores.Where(a => a.Id == model.Id).FirstOrDefault();
                List<Usuario> usuariosCandidatos = db.Usuarios.Where(u => u.OperadorId == null && u.Rol == Usuario.RolUsuario.Normal).ToList();
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
                if (operador.Autos.Count > 0)
                {
                    int idAuto = operador.Autos.First().Id;
                    Auto autoActual = flota.Autos.Where(a => a.Id == idAuto).FirstOrDefault();
                    autosCandidatos.Add(autoActual);
                    autosCandidatos.Add(new Auto() { Id = 0, NombreVehiculo = "No asignar" });
                    autosCandidatos.Reverse();
                }

                ViewBag.UsuarioId = new SelectList(usuariosCandidatos, "Id", "Email");
                ViewBag.AutoId = new SelectList(autosCandidatos, "Id", "NombreVehiculo");
                ViewBag.FlotaId = flota.Id;
                return View("CrearOperadorEnFlota", model);
            }

            Operador.EditarOperador(db, model.FlotaId, model.Id, model.Nombre, model.TipoLicencia, model.UsuarioId, model.AutoId);

            return RedirectToAction("OperadoresFlota", new { id = model.FlotaId });
        }

        public ActionResult EliminarOperadorDeFlota(int? id, int? idFlota)
        {
            if (id == null || idFlota == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            if (flota == null)
            {
                return HttpNotFound();
            }
            Operador operador = flota.Operadores.Where(o => o.Id == id).FirstOrDefault();

            if (operador == null)
            {
                return HttpNotFound();
            }

            Operador.EliminarOperador(db, operador.Id);

            return RedirectToAction("OperadoresFlota", new { id = idFlota });
        }

        #endregion

#region -----------AUTOS DE FLOTA --------------------

        public ActionResult AutosFlota(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Flota flota = db.Flotas.Where(f => f.Id == id).FirstOrDefault();

            if (flota == null)
                return HttpNotFound();

            List<Auto> autos = flota.Autos.ToList();

            ViewBag.NombreFlota = flota.Nombre;
            ViewBag.FlotaId = flota.Id;
            return View(autos);
        }

        public ActionResult CrearAutoEnFlota(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Flota flota = db.Flotas.Where(f => f.Id == id).FirstOrDefault();
            if (flota == null)
            {
                return HttpNotFound();
            }

            //List<Operador> operadoresSinAuto = flota.Operadores.Where(o => o.Auto == null).ToList();
            List<Operador> operadoresSinAuto = flota.Operadores.Where(o => o.Autos.Count == 0).ToList();
            operadoresSinAuto.Add(new Operador() { Id = 0, Nombre = "No asignar" });
            operadoresSinAuto.Reverse();

            List<TipoVehiculo> tipoVehiculos = flota.TiposVehiculo.ToList();
            tipoVehiculos.Add(new TipoVehiculo() { Id = 0, Tipo = "No asignar" });
            tipoVehiculos.Reverse();

            ViewBag.OperadorId = new SelectList(operadoresSinAuto, "Id", "Nombre");
            ViewBag.TipoVehiculoId = new SelectList(tipoVehiculos, "Id", "Tipo");

            ViewBag.FlotaId = flota.Id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearAutoEnFlota([Bind(Include = "Id,NombreVehiculo,Patente,Marca,Modelo,Year,KilometrajeActual,TipoVehiculoId,OperadorId,FlotaId")] CreacionAuto model)
        {
            Flota flota = db.Flotas.Where(f => f.Id == model.FlotaId).FirstOrDefault();

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
                ViewBag.FlotaId = flota.Id;
                return View("CrearAutoEnFlota", model);
            }


            TipoVehiculo tipo = flota.TiposVehiculo.Where(t => t.Id == model.TipoVehiculoId).FirstOrDefault();
            Auto.CrearAuto(db, model.NombreVehiculo, model.Patente, tipo, model.Marca, model.Modelo, model.Year, model.KilometrajeActual, model.FlotaId, model.OperadorId);

            return RedirectToAction("AutosFlota", new { id = model.FlotaId });
        }

        public ActionResult EditarAutoDeFlota(int? id, int? idFlota)
        {
            if (id == null || idFlota == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            if (flota == null)
                return HttpNotFound();

            Auto auto = flota.Autos.Where(o => o.Id == id).FirstOrDefault();

            if (auto == null)
                return HttpNotFound();

            CreacionAuto modeloAuto = new CreacionAuto()
            {
                Id = auto.Id,
                NombreVehiculo = auto.NombreVehiculo,
                Patente = auto.Patente,
                Marca = auto.Marca,
                Modelo = auto.Modelo,
                Year = auto.Year,
                KilometrajeActual = auto.KilometrajeActual,
                FlotaId = flota.Id,
                OperadorId = 0,
                TipoVehiculoId = 0
            };

            List<Operador> operadoresSinAuto = flota.Operadores.Where(o => o.Autos.Count == 0).ToList();
            //List<Operador> operadoresSinAuto = flota.Operadores.Where(o => o.Auto == null).ToList();
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

            ViewBag.FlotaId = flota.Id;
            return View(modeloAuto);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAutoDeFlota([Bind(Include = "Id,NombreVehiculo,Patente,Marca,Modelo,Year,KilometrajeActual,TipoVehiculoId,OperadorId,FlotaId")] CreacionAuto model)
        {
            Flota flota = db.Flotas.Where(f => f.Id == model.FlotaId).FirstOrDefault();


            if (ModelState.IsValid == false)
            {
                Auto auto = flota.Autos.Where(a => a.Id == model.Id).FirstOrDefault();
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
                ViewBag.FlotaId = flota.Id;
                return View("EditarAutoDeFlota", model);
            }

            TipoVehiculo tipo = flota.TiposVehiculo.Where(t => t.Id == model.TipoVehiculoId).FirstOrDefault();
            Auto.EditarAuto(db, model.Id, model.NombreVehiculo, model.Patente, tipo, model.Marca, model.Modelo, model.Year, model.KilometrajeActual, model.FlotaId, model.OperadorId);

            return RedirectToAction("AutosFlota", new { id = model.FlotaId });
        }

        public ActionResult EliminarAutoDeFlota(int? id, int? idFlota)
        {
            if (id == null || idFlota == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Flota flota = db.Flotas.Where(f => f.Id == idFlota).FirstOrDefault();

            if (flota == null)
            {
                return HttpNotFound();
            }
            Auto auto = flota.Autos.Where(a => a.Id == id).FirstOrDefault();

            if (auto == null)
            {
                return HttpNotFound();
            }

            Auto.ElimiarAuto(db, auto.Id);

            return RedirectToAction("AutosFlota", new { id = flota.Id });
        }
        #endregion


        #endregion



    }
}