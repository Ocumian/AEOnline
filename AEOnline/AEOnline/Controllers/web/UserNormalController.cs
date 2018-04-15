using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AEOnline.Models;
using AEOnline.Models.WebModels;
using System.Data;
using Newtonsoft.Json;
using System.Globalization;
using AEOnline.ClasesAdicionales;

namespace AEOnline.Controllers.web
{
    public class UserNormalController : Controller
    {
        private ProyectoAutoContext db = new ProyectoAutoContext();


        // GET: UserNormal
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Estadisticas()
        {
            //Modelo usado en la vista principal
            CreacionUsuario us = new CreacionUsuario();
            us.Fecha = DateTime.Today;

            SelectList tiposHistorial = new SelectList(HistorialWeb.ObtenerTiposHistorial());
            ViewBag.MyType = tiposHistorial;
            return View(us);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Estadisticas(HistorialWeb.TiposHistorial? MyType, string Fecha)
        {
            string fechaString = Fecha;
            fechaString = fechaString.Replace('-', '/');
            fechaString += " 00:00:00";

            DateTime fechaSeleccionada;  
            bool result = DateTime.TryParseExact(fechaString, FormatoFecha.formato, FormatoFecha.provider, DateTimeStyles.None, out fechaSeleccionada);

            #region Modelo Usado en vista principal
            if (Session["Nombre"] == null)
                return RedirectToAction("Index", "Login");

            string nombreSession = Session["Nombre"].ToString();
            Usuario userActual = db.Usuarios.Where(u => u.Nombre == nombreSession).FirstOrDefault();


            CreacionUsuario us = new CreacionUsuario();
            us.Fecha = fechaSeleccionada;
            us.AutoPatente = "";
            us.AutoId = -1;

            if (userActual.OperadorId != null)
            {
                if(userActual.Operador.Autos.Count > 0)
                {
                    us.AutoPatente = userActual.Operador.Autos.First().Patente;
                    us.AutoId = userActual.Operador.Autos.First().Id;
                }
                
                //us.AutoPatente = userActual.Operador.Auto.Patente;
                //us.AutoId = userActual.Operador.Auto.Id;
            }


            SelectList tiposHistorial = new SelectList(HistorialWeb.ObtenerTiposHistorial());
            ViewBag.MyType = tiposHistorial;
            ViewBag.HistorialSeleccionado = MyType;
            ViewBag.FechaSeleccionada = Fecha;

            #endregion


            //Modelo historial va al viewbag que se envía a la vista parcial
            HistorialWeb historialWeb = new HistorialWeb();
            historialWeb.FechaMostrar = fechaSeleccionada;
            historialWeb.PatenteAuto = us.AutoPatente;

            if (MyType == HistorialWeb.TiposHistorial.Velocidad)
            {
                historialWeb = HistorialesManager.PrepararHistorialVelocidad(db, historialWeb, fechaSeleccionada, us.AutoId);

                //Viewbag enviado a la vista parcial
                ViewBag.HistorialWeb = historialWeb;
            }
            else if(MyType == HistorialWeb.TiposHistorial.Posicion)
            {
                historialWeb = HistorialesManager.PrepararHistorialPosicion(db, historialWeb, fechaSeleccionada, us.AutoId);

                //ViewBag enviado a la vista parcial
                ViewBag.HistorialWeb = historialWeb;
            }
            else if (MyType == HistorialWeb.TiposHistorial.Energia)
            {
                historialWeb = HistorialesManager.PrepararHistorialEnergia(db, historialWeb, fechaSeleccionada, us.AutoId);

                //ViewBag enviado a la vista parcial
                ViewBag.HistorialWeb = historialWeb;
            }

            return View(us);
        }

        public ActionResult Mapa()
        {
            if (Session["Nombre"] == null)
                return RedirectToAction("Index", "Login");

            string nombreSession = Session["Nombre"].ToString();
            Usuario userActual = db.Usuarios.Where(u => u.Nombre == nombreSession).FirstOrDefault();

            Auto auto = new Auto();

            if (userActual.OperadorId != null)
            {
                ViewBag.TieneAuto = true;
                //auto = userActual.Operador.Auto;
                if(userActual.Operador.Autos.Count > 0)
                {
                    auto = userActual.Operador.Autos.First() ;
                }

            }
            else
                ViewBag.TieneAuto = false;

            

            return View(auto);
        }

        public ActionResult Delivery()
        {
            return View();
        }


        //Llamado desde javascript de la vista _EstadisticaPosicion
        [HttpGet]
        public ActionResult getPosicionesFiltradasYMas(string horaDesde, string horaHasta, int idAuto)
        {
            Auto auto = db.Autos.Where(a => a.Id == idAuto).FirstOrDefault();
            List<HistorialDiario> historialesDiarios = auto.HistorialesDiarios.ToList();

            horaDesde = horaDesde.Replace('-', '/');
            horaHasta = horaHasta.Replace('-', '/');

            string formato = "d/M/yyyy H:m:s";

            DateTime desde;
            bool resultDesde = DateTime.TryParseExact(horaDesde, formato, FormatoFecha.provider, DateTimeStyles.None, out desde);

            DateTime hasta;
            bool resultHasta = DateTime.TryParseExact(horaHasta, formato, FormatoFecha.provider, DateTimeStyles.None, out hasta);


            HistorialDiario historialHoy = null;

            List<HistorialDiario> historialesHoy = auto.HistorialesDiarios
                .Where(h => h.Fecha.Year == desde.Year
                && h.Fecha.Month == desde.Month
                && h.Fecha.Day == desde.Day).ToList();

            int nResultados = 0;

            foreach (HistorialDiario hd in historialesHoy)
            {
                if (hd.historialesPosicion.Count > nResultados)
                {
                    historialHoy = hd;
                    nResultados = hd.historialesPosicion.Count;
                }
            }

            List<Ruta> filtro = Ruta.CrearRutasEnRango(db, historialHoy, desde, hasta);

            //filtro = filtro.OrderBy(h => h.FechaHora).ToList();

            System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string sJSON = oSerializer.Serialize(filtro);

            return Json(new { respuesta = sJSON }, JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        public ActionResult getPosicionAuto(string patenteAuto)
        {
            Auto auto = db.Autos.Where(a => a.Patente == patenteAuto).FirstOrDefault();

            string latitud = auto.Latitud.ToString().Replace(',', '.');
            string longitud = auto.Longitud.ToString().Replace(',', '.');


            return Json(new { Latitud = latitud, Longitud = longitud }, JsonRequestBehavior.AllowGet);
        }
    }
}