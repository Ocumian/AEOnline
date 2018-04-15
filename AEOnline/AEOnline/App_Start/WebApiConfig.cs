using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;
using AEOnline.Models;
using AEOnline.Models.WebModels;
using AEOnline.ClasesAdicionales;
using GMap.NET;

namespace AEOnline
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de API web

            // Rutas de API web
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            

            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Alerta>("Alertas");
            builder.EntitySet<GrupoFlota>("GrupoFlota");
            builder.EntitySet<HistorialCargaCombustible>("HistorialesCargaCombustible");
            builder.EntitySet<HistorialIncidente>("HistorialesIncidente");
            builder.EntitySet<HistorialMantencion>("HistorialesMantencion");
            builder.EntitySet<Proveedor>("Proveedores");
            builder.EntitySet<Recordatorio>("Recordatorio");
            builder.EntitySet<Servicio>("Servicios");
            builder.EntitySet<TipoVehiculo>("TipoVehiculo");
            builder.EntitySet<Usuario>("Usuarios");
            builder.EntitySet<Flota>("Flotas");
            builder.EntitySet<Auto>("Autos");
            builder.EntitySet<Operador>("Operadores");
            builder.EntitySet<UsuarioFlota>("UsuarioFlotas");
            builder.EntitySet<HistorialDiario>("HistorialesDiarios");
            builder.EntitySet<HistorialVelocidad>("HistorialVelocidades");
            builder.EntitySet<HistorialPosicion>("HistorialPosiciones");
            builder.EntitySet<HistorialEnergia>("HistorialesEnergia");
            builder.EntitySet<MantencionServicio>("MantencionServicios");

            #region Actions testeos

            ActionConfiguration obtCalle = builder.Entity<Usuario>().Collection.Action("NombreCalle");
            obtCalle.Parameter<double>("lat");
            obtCalle.Parameter<double>("lng");
            obtCalle.Returns<string>();

            ActionConfiguration obtDireccion = builder.Entity<Usuario>().Collection.Action("ObtenerDireccion");
            obtDireccion.Parameter<double>("latInicio");
            obtDireccion.Parameter<double>("lngInicio");
            obtDireccion.Parameter<double>("latFinal");
            obtDireccion.Parameter<double>("lngFinal");
            obtDireccion.Returns<GDirections>();

            ActionConfiguration fechaHoy = builder.Entity<Auto>().Collection.Action("FechaHoy");
            fechaHoy.Returns<DateTime>();

            ActionConfiguration distanciaPuntos = builder.Entity<Auto>().Collection.Action("DistanciaPuntos");
            distanciaPuntos.Parameter<double>("latInicio");
            distanciaPuntos.Parameter<double>("lngInicio");
            distanciaPuntos.Parameter<double>("latFinal");
            distanciaPuntos.Parameter<double>("lngFinal");
            distanciaPuntos.Returns<double>();

            #endregion


            #region Actions Usuario

            ActionConfiguration crearCuentaUsuario = builder.Entity<Usuario>().Collection.Action("CrearCuentaUsuario");
            crearCuentaUsuario.Returns<RespuestaOdata>();
            crearCuentaUsuario.Parameter<string>("Nombre");
            crearCuentaUsuario.Parameter<string>("Email");
            crearCuentaUsuario.Parameter<string>("Password");

            ActionConfiguration inicioSesion = builder.Entity<Usuario>().Collection.Action("IniciarSesion");
            inicioSesion.Returns<RespuestaOdata>();
            inicioSesion.Parameter<string>("Email");
            inicioSesion.Parameter<string>("Password");

            ActionConfiguration proveedorsActivos = builder.Entity<Usuario>().Collection.Action("ProveedoresActuales");
            proveedorsActivos.ReturnsCollectionFromEntitySet<Proveedor>("Proveedores");
            proveedorsActivos.Parameter<int>("IdAuto");

            ActionConfiguration cargarCombustible = builder.Entity<Usuario>().Collection.Action("CargarCombustible");
            cargarCombustible.Returns<RespuestaOdata>();
            cargarCombustible.Parameter<int>("IdAuto");
            cargarCombustible.Parameter<string>("FechaHora");
            cargarCombustible.Parameter<bool>("EstanqueLleno");
            cargarCombustible.Parameter<float>("CantidadLitros");
            cargarCombustible.Parameter<int>("CostoUnitario");
            cargarCombustible.Parameter<int>("Kilometraje");
            cargarCombustible.Parameter<int>("IdProveedor");
            cargarCombustible.Parameter<string>("RutProveedor");
            cargarCombustible.Parameter<int>("NumeroBoleta");

            #endregion

            #region Actions AUTO

            ActionConfiguration fechaEjemplo = builder.Entity<Auto>().Collection.Action("FechaEjemplo");
            fechaEjemplo.Returns<string>();
            fechaEjemplo.Parameter<string>("Fecha");

            ActionConfiguration asignarPatente = builder.Entity<Auto>().Collection.Action("AsignarPatente");
            asignarPatente.Returns<RespuestaOdata>();
            asignarPatente.Parameter<string>("NuevaPatente");
            asignarPatente.Parameter<string>("Email");
            asignarPatente.Parameter<string>("Password");


            ActionConfiguration actualizarVelocidadListaDX = builder.Entity<Auto>().Collection.Action("ActualizarVelocidadListaDX");
            actualizarVelocidadListaDX.Returns<string>();
            actualizarVelocidadListaDX.Parameter<int>("Id");
            actualizarVelocidadListaDX.CollectionParameter<RegistroHistorial>("Registros");


            ActionConfiguration actualizarEnergia = builder.Entity<Auto>().Collection.Action("ActualizarEnergia");
            actualizarEnergia.Returns<IHttpActionResult>();
            actualizarEnergia.Parameter<int>("Id");
            actualizarEnergia.Parameter<string>("HoraRegistro");
            actualizarEnergia.Parameter<string>("HoraInicio");
            actualizarEnergia.Parameter<string>("HoraFinal");
            actualizarEnergia.Parameter<string>("HoraMenor");
            actualizarEnergia.Parameter<string>("HoraMayor");
            actualizarEnergia.Parameter<string>("HoraMitad");
            actualizarEnergia.Parameter<float>("ValorInicio");
            actualizarEnergia.Parameter<float>("ValorFinal");
            actualizarEnergia.Parameter<float>("ValorMenor");
            actualizarEnergia.Parameter<float>("ValorMayor");
            actualizarEnergia.Parameter<float>("ValorMitad");

            ActionConfiguration actualizarPosicionDX = builder.Entity<Auto>().Collection.Action("ActualizarPosicionListaDX");
            actualizarPosicionDX.Returns<string>();
            actualizarPosicionDX.Parameter<int>("Id");
            actualizarPosicionDX.CollectionParameter<Posicion>("ListaPosiciones");

            ActionConfiguration actualizarPosicionDXZ = builder.Entity<Auto>().Collection.Action("ActualizarPosicionListaDXZ");
            actualizarPosicionDXZ.Returns<string>();
            actualizarPosicionDXZ.Parameter<int>("Id");
            actualizarPosicionDXZ.CollectionParameter<Posicion>("ListaPosiciones");

            ActionConfiguration actualizarPosicionDXZgps = builder.Entity<Auto>().Collection.Action("ActualizarPosicionListaDXZGPS");
            actualizarPosicionDXZgps.Returns<string>();
            actualizarPosicionDXZgps.Parameter<int>("Id");
            actualizarPosicionDXZgps.CollectionParameter<Posicion>("ListaPosiciones");

            ActionConfiguration obtenerHistorialVel = builder.Entity<Auto>().Collection.Action("ObtenerHistorialesVelocidad");
            obtenerHistorialVel.ReturnsCollectionFromEntitySet<HistorialVelocidad>("HistorialVelocidades");
            obtenerHistorialVel.Parameter<int>("Id");
            obtenerHistorialVel.Parameter<string>("Fecha");

            ActionConfiguration obtenerHistorialPos = builder.Entity<Auto>().Collection.Action("ObtenerHistorialesPosicion");
            obtenerHistorialPos.ReturnsCollectionFromEntitySet<HistorialPosicion>("HistorialPosiciones");
            obtenerHistorialPos.Parameter<int>("Id");
            obtenerHistorialPos.Parameter<string>("FechaInicio");
            obtenerHistorialPos.Parameter<string>("FechaFinal");

            #endregion



            config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
        }
    }
}
