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

            ViewBag.TiposHistorial = HistorialWeb.ObtenerTiposHistorial();

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
                us.AutoPatente = userActual.Operador.Auto.Patente;
                us.AutoId = userActual.Operador.Auto.Id;
            }


            ViewBag.TiposHistorial = HistorialWeb.ObtenerTiposHistorial();
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
                auto = userActual.Operador.Auto;

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
        public ActionResult getPosicionesFiltradas(string horaDesde, string horaHasta, int idAuto)
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

            List<HistorialPosicion> historiales = historialesDiarios.Where(h => h.Fecha.Year == desde.Year
                                                                               && h.Fecha.Month == desde.Month
                                                                               && h.Fecha.Day == desde.Day).FirstOrDefault().historialesPosicion.ToList();

            List<HistorialPosicion> filtro = new List<HistorialPosicion>();
            foreach(HistorialPosicion hp in historiales)
            {
                if(hp.FechaHora >= desde && hp.FechaHora <= hasta)
                {
                    filtro.Add(hp);
                }
            }

            filtro = filtro.OrderBy(h => h.FechaHora).ToList();

            System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string sJSON = oSerializer.Serialize(filtro);

            return Json(new { respuesta = sJSON }, JsonRequestBehavior.AllowGet);
        }


        //Llamado desde javascript de la vista _EstadisticaPosicion
        [HttpGet]
        public ActionResult getPosicionesFiltradasV2(string horaDesde, string horaHasta, int idAuto)
        {
            try
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


                HistorialDiario historialHoy = historialesDiarios.Where(h => h.Fecha.Year == desde.Year
                                                                                   && h.Fecha.Month == desde.Month
                                                                                   && h.Fecha.Day == desde.Day).FirstOrDefault();

                List<HistorialPosicion> historiales = historialHoy.historialesPosicion.OrderBy(h => h.FechaHora).ToList();
                //TEST
                //List<HistorialPosicion> historiales = HistorialesManager.CrearHistorialPosicionejemplo().OrderBy(h => h.FechaHora).ToList();

                HistorialPosicion noProcesado = historiales.Where(h => h.Procesado == false).FirstOrDefault();
                if (noProcesado != null)
                {
                    if(noProcesado.Id != historiales.Last().Id)
                        historiales = HistorialesManager.ProcesarHistorialPosicion(db, historialHoy, historiales);
                }
                    

                List<HistorialPosicion> filtro = new List<HistorialPosicion>();
                foreach (HistorialPosicion hp in historiales)
                {
                    if (hp.FechaHora >= desde && hp.FechaHora <= hasta)
                    {
                        filtro.Add(hp);
                    }
                }

                filtro = filtro.OrderBy(h => h.FechaHora).ToList();

                System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string sJSON = oSerializer.Serialize(filtro);

                return Json(new { respuesta = sJSON }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                List<HistorialPosicion> filtro = new List<HistorialPosicion>();

                System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string sJSON = oSerializer.Serialize(filtro);

                return Json(new { respuesta = sJSON }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult ProcesarPuntosDia(string horaDesde, string horaHasta, int idAuto)
        {
            try
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


                HistorialDiario historialHoy = historialesDiarios.Where(h => h.Fecha.Year == desde.Year
                                                                                   && h.Fecha.Month == desde.Month
                                                                                   && h.Fecha.Day == desde.Day).FirstOrDefault();

                List<HistorialPosicion> historiales = historialHoy.historialesPosicion.OrderBy(h => h.FechaHora).ToList();
                //TEST
                //List<HistorialPosicion> historiales = HistorialesManager.CrearHistorialPosicionejemplo().OrderBy(h => h.FechaHora).ToList();

                HistorialPosicion noProcesado = historiales.Where(h => h.Procesado == false).FirstOrDefault();
                if (noProcesado != null)
                {
                    if (noProcesado.Id != historiales.Last().Id)
                        historiales = HistorialesManager.ProcesarHistorialPosicion(db, historialHoy, historiales);
                }


                List<HistorialPosicion> filtro = new List<HistorialPosicion>();
                foreach (HistorialPosicion hp in historiales)
                {
                    if (hp.FechaHora >= desde && hp.FechaHora <= hasta)
                    {
                        filtro.Add(hp);
                    }
                }

                filtro = filtro.OrderBy(h => h.FechaHora).ToList();

                System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string sJSON = oSerializer.Serialize(filtro);

                return Json(new { respuesta = sJSON }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                List<HistorialPosicion> filtro = new List<HistorialPosicion>();

                System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string sJSON = oSerializer.Serialize(filtro);

                return Json(new { respuesta = sJSON }, JsonRequestBehavior.AllowGet);
            }
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