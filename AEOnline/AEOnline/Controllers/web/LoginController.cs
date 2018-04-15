using AEOnline.ClasesAdicionales;
using AEOnline.Models;
using AEOnline.Models.WebModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AEOnline.Controllers.web
{
    public class LoginController : Controller
    {
        private ProyectoAutoContext db = new ProyectoAutoContext();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CerrarSesion()
        {
            Session["Nombre"] = "";
            Session["Email"] ="";
            Session["Rol"] = "";  

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Authenticate(String emailID, string passID)
        {
            int superAdmins = db.Usuarios.Where(u => u.Rol == Usuario.RolUsuario.SuperAdmin).ToList().Count;

            if(superAdmins == 0)
            {
                Session["Nombre"] = "first";
                return RedirectToAction("PrimerUsuario");
            }


            bool validado = false;
            Usuario userEncontrado = db.Usuarios.Where(u => u.Email == emailID).FirstOrDefault();
            if(userEncontrado != null)
            {
                //validado = true;
                validado = PasswordHash.ValidatePassword(passID, userEncontrado.Password);
            }


            if (validado)
            {
                Session["Nombre"] = userEncontrado.Nombre;
                Session["Email"] = userEncontrado.Email;
                Session["Rol"] = userEncontrado.Rol;

                if(userEncontrado.Rol == Usuario.RolUsuario.Bloqueado)
                {
                    TempData["msg"] = "Cuenta con acceso bloqueado.";
                    return RedirectToAction("Index");
                }
                else if(userEncontrado.Rol == Usuario.RolUsuario.SuperAdmin)
                {
                    return RedirectToAction("Index","SuperAdmin");
                }
                else if(userEncontrado.Rol == Usuario.RolUsuario.AdminDeFlota)
                {
                    if(userEncontrado.UsuarioFlotaId == null)
                    {

                        Flota f = Flota.CrearFlota(db, userEncontrado.Nombre + "Flota"+userEncontrado.Id, userEncontrado.Id, "Default");

                        Session["Flota"] = f.Id;
                    }
                    else if (userEncontrado.UsuarioFlotaId != null)
                        Session["Flota"] = userEncontrado.UsuarioFlota.Flota.Id;

                    return RedirectToAction("Index", "AdminFlota2");
                }
                else
                {
                    //user normal
                    TempData["msg"] = "Este acceso es exclusivo de administradores.";
                    return RedirectToAction("Index");
                    //return RedirectToAction("Index", "UserNormal");
                }
               
            }
            else
            {
                TempData["msg"] = "Usuario/Contraseña no existentes.";
                return RedirectToAction("Index");
            }

        }

        //GET: Login/PrimerUsuario
        public ActionResult PrimerUsuario()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearPrimerUsuario([Bind(Include = "Id,Nombre,Password,ConfirmPassword,Email")] CreacionUsuario model)
        {
            if(ModelState.IsValid == false)       
                return View("PrimerUsuario", model);


            string passEncriptada = PasswordHash.CreateHash(model.Password.Trim());

            Usuario u = new Usuario()
            {
                Nombre = model.Nombre,
                Email = model.Email,
                Password = passEncriptada, 
                Rol = Usuario.RolUsuario.SuperAdmin
            };

            db.Usuarios.Add(u);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult RegistroUsuario()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistroUsuario([Bind(Include = "Id,Nombre,Password,ConfirmPassword,Email")] CreacionUsuario model)
        {
            if (ModelState.IsValid == false)
            {
                return View("RegistroUsuario", model);
            }

            bool repetido = Usuario.VerificarRepetido(model.Email);

            if (repetido)
            {
                ModelState.AddModelError("Email", "Correo electronico ya ocupado.");
                return View("RegistroUsuario", model);
            }

            Usuario.CrearUsuario(db, model.Nombre, model.Email, model.Password, Usuario.RolUsuario.AdminDeFlota, 0);


            return RedirectToAction("Index");
        }

        public ActionResult AcercaDe()
        {
            return View();
        }
    }
}