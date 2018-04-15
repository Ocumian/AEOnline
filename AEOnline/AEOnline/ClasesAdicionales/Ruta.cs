using AEOnline.Models;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Web;

namespace AEOnline.ClasesAdicionales
{
    public class Ruta
    {
        public List<HistorialPosicion> Puntos { get; set; }
        public float VelocidadPromedio { get; set; }
        public double KilometrosRecorridos { get; set; } 
        public float LitrosConsumidos { get; set; }
        public float CostoEstimadoCombustible { get; set; }


        public string NombreCalleInicio { get; set; }
        public string NombreCiudadInicio { get; set; }
        public string NombreCalleFinal { get; set; }
        public string NombreCiudadFinal { get; set; }

        public Ruta(ProyectoAutoContext _db, List<HistorialPosicion> _puntos, HistorialDiario _histDia)
        {
            if (_puntos.Count == 0)
                return;

            List<HistorialPosicion> puntos = _puntos.OrderBy(p => p.FechaHora).ToList();

            #region Filtro SPIKES

            //la suma de los dos siguientes segmentos 
            //noo deberia ser mayor a la distancia del primer a tercer punto x 3

            //En este filtro el primer punto es aceptado por default
            //A partir del "indexOrigen", se evalua si se agrega al filtro el "indexOrigen" + 1

            float multiploLimite = 1.5f;
            List<HistorialPosicion> filtro = new List<HistorialPosicion>();
            filtro.Add(puntos[0]);

            //int indexOrigen = 0;
            for (int i = 0; i < puntos.Count; i++)
            {
                
                if ( i < puntos.Count - 2)
                {
                    var origen = new GeoCoordinate(puntos[i].Latitud, puntos[i].Longitud);
                    var evaluar = new GeoCoordinate(puntos[i+1].Latitud, puntos[i+1].Longitud);
                    var final = new GeoCoordinate(puntos[i + 2].Latitud, puntos[i + 2].Longitud);

                    if (puntos[i + 1].GPSOffBool == true)
                        continue;

                    double de1a2 = origen.GetDistanceTo(evaluar);
                    double de2a3 = evaluar.GetDistanceTo(final);
                    double de1a3 = origen.GetDistanceTo(final);

                    double suma = de1a2 + de2a3;

                    if(suma <= de1a3 * multiploLimite)
                    {
                        filtro.Add(puntos[i + 1]);
                    }

                }
                else if(i == puntos.Count - 2)
                {
                    filtro.Add(puntos[i + 1]);
                }
                    
            }
            filtro = filtro.OrderBy(h => h.FechaHora).ToList();

            #endregion
            puntos = filtro;

            #region Filtro distancia

            float distanciaLimite = 150; //metros
            float distanciaMinima = 6;
            List<HistorialPosicion> filtroDistancia = new List<HistorialPosicion>();
            //int indexOrigen = 0;
            for (int i = 0; i < puntos.Count - 1; i++)
            {
                if (puntos[i].GPSOffBool == true)
                    continue;

                var origen = new GeoCoordinate(puntos[i].Latitud, puntos[i].Longitud);
                var final = new GeoCoordinate(puntos[i + 1].Latitud, puntos[i + 1].Longitud);

                double distancia = origen.GetDistanceTo(final);

                if (distancia <= distanciaLimite && distancia >= distanciaMinima)
                {
                    filtroDistancia.Add(puntos[i]);
                }

            }
            filtroDistancia = filtroDistancia.OrderBy(h => h.FechaHora).ToList();

            #endregion
            puntos = filtroDistancia;


            if (puntos.Count == 0)
            {
                Puntos = new List<HistorialPosicion>();
                return;
            }
               

            Puntos = puntos;
            double metrosRecorridos = 0;
            for (int i = 0; i < puntos.Count - 1; i++)
            {
                var sCoord = new GeoCoordinate(puntos[i].Latitud, puntos[i].Longitud);
                var eCoord = new GeoCoordinate(puntos[i+1].Latitud, puntos[i+1].Longitud);

                metrosRecorridos += sCoord.GetDistanceTo(eCoord);
            }
            double km = metrosRecorridos / 1000f;
            KilometrosRecorridos = Math.Round(km, 2, MidpointRounding.AwayFromZero);

            DateTime desde = puntos.First().FechaHora;
            DateTime hasta = puntos.Last().FechaHora;

            List<float> filtroVelocidades = new List<float>();
            float sumaVelocidades = 0;
            foreach(HistorialVelocidad hv in _histDia.historialesVelocidad)
            {
                if(hv.HoraRegistro >= desde && hv.HoraRegistro <= hasta)
                {
                    filtroVelocidades.Add(hv.ValorInicio);
                    sumaVelocidades += hv.ValorInicio;

                    filtroVelocidades.Add(hv.ValorUnCuarto);
                    sumaVelocidades += hv.ValorUnCuarto;

                    filtroVelocidades.Add(hv.ValorMitad);
                    sumaVelocidades += hv.ValorMitad;

                    filtroVelocidades.Add(hv.ValorTresCuartos);
                    sumaVelocidades += hv.ValorTresCuartos;

                    filtroVelocidades.Add(hv.ValorFinal);
                    sumaVelocidades += hv.ValorFinal;

                    filtroVelocidades.Add(hv.ValorMayor);
                    sumaVelocidades += hv.ValorMayor;

                    filtroVelocidades.Add(hv.ValorMenor);
                    sumaVelocidades += hv.ValorMenor;
                }
            }

            if(filtroVelocidades.Count > 0)
                VelocidadPromedio = sumaVelocidades / filtroVelocidades.Count;

            HistorialPosicion inicioHP = puntos.First();
            HistorialPosicion finalHP = puntos.Last();

            if (inicioHP.NombreCalle == null)
                inicioHP.NombreCalle = "";
            if (inicioHP.NombreLocalidad == null)
                inicioHP.NombreLocalidad = "";
            if (finalHP.NombreCalle == null)
                finalHP.NombreLocalidad = "";
            if (finalHP.NombreLocalidad == null)
                finalHP.NombreLocalidad = "";

            if(inicioHP.NombreCalle == "" || inicioHP.NombreLocalidad == "")
            {
                List<Placemark> datosInicio = Posicion.ObtenerDatosPosición(puntos.First().Latitud, puntos.First().Longitud);
                if (datosInicio != null)
                {
                    if(inicioHP.NombreCalle == "")
                        inicioHP.NombreCalle = datosInicio[0].ThoroughfareName;
             
                    if(inicioHP.NombreLocalidad == "")
                        inicioHP.NombreLocalidad = datosInicio[0].LocalityName;            
                }
            }

            if(finalHP.NombreCalle == "" || finalHP.NombreLocalidad == "")
            {
                List<Placemark> datosFinal = Posicion.ObtenerDatosPosición(puntos.Last().Latitud, puntos.Last().Longitud);

                if (datosFinal != null)
                {
                    if(finalHP.NombreCalle == "")
                        finalHP.NombreCalle = datosFinal[0].ThoroughfareName;
                    if(finalHP.NombreLocalidad == "")
                        finalHP.NombreLocalidad = datosFinal[0].LocalityName;
                }
            }
            _db.SaveChanges();

            NombreCalleInicio = inicioHP.NombreCalle;
            NombreCiudadInicio = inicioHP.NombreLocalidad;

            NombreCalleFinal = finalHP.NombreCalle;
            NombreCiudadFinal = finalHP.NombreLocalidad;

            //NombreCalleInicio = "";
            //NombreCiudadInicio = "";
            //NombreCalleFinal = "";
            //NombreCiudadFinal = "";

            //List<Placemark> datosInicio = Posicion.ObtenerDatosPosición(puntos.First().Latitud, puntos.First().Longitud);
            //if (datosInicio != null)
            //{
            //    NombreCalleInicio = datosInicio[0].ThoroughfareName;
            //    NombreCiudadInicio = datosInicio[0].LocalityName;
            //}

            //List<Placemark> datosFinal = Posicion.ObtenerDatosPosición(puntos.Last().Latitud, puntos.Last().Longitud);

            //if(datosFinal != null)
            //{
            //    NombreCalleFinal = datosFinal[0].ThoroughfareName;
            //    NombreCiudadFinal = datosFinal[0].LocalityName;
            //}
        }


        public static List<Ruta> CrearRutasEnRango(ProyectoAutoContext _db, HistorialDiario _histDia, DateTime _desde, DateTime _hasta)
        {
            List<Ruta> rutas = new List<Ruta>();
            List<HistorialPosicion> listaActual = new List<HistorialPosicion>();

            for (int i = 0; i < _histDia.historialesPosicion.Count; i++)
            {
                HistorialPosicion puntoActual = _histDia.historialesPosicion[i];
                if (puntoActual.Inicio == true)
                {
                    if (listaActual.Count > 0)
                    {
                        Ruta nruta = new Ruta(_db, listaActual, _histDia);
                        if(nruta.Puntos.Count > 0)
                        {
                            rutas.Add(nruta);
                        }                 
                    }

                    listaActual = new List<HistorialPosicion>();
                }

                if(puntoActual.FechaHora >= _desde && puntoActual.FechaHora <= _hasta)
                    listaActual.Add(puntoActual);
            }

            rutas.Add(new Ruta(_db, listaActual, _histDia));

            return rutas;
        }
    }
}