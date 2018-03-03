using GMap.NET;
using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AEOnline.ClasesAdicionales
{
    public class Posicion
    {
        public string FechaHora { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }

        public static string ObtenerCalle(double _lat, double _lng)
        {
            int numeroIntentos = 20;

            List<Placemark> plc = null;
            var st = GMapProviders.GoogleMap.GetPlacemarks(new PointLatLng(_lat, _lng), out plc);

            int c = 0;
            if (plc == null && c < numeroIntentos)
            {
                st = GMapProviders.GoogleMap.GetPlacemarks(new PointLatLng(_lat, _lng), out plc);
                c++;
            }

            if (st == GeoCoderStatusCode.G_GEO_SUCCESS && plc != null)
            {
                //foreach (var pl in plc)
                //{
                //    if (pl.LocalityName == "Osorno")
                //}

                return plc[0].ThoroughfareName;
            }

            return "";
        }

        public static GDirections ObtenerDireccion(double _latInicio, double _lngInicio, double _latFinal, double _lngFinal)
        {
            int numeroIntentos = 10;

            GDirections direccion;
            PointLatLng puntoInicio = new PointLatLng(_latInicio, _lngInicio);
            PointLatLng puntoFinal = new PointLatLng(_latFinal, _lngFinal);

            var rutasDireccion = GMapProviders.GoogleMap.GetDirections(out direccion, puntoInicio, puntoFinal, false, false, true, false, false);

            int c = 0;
            while (direccion == null && c < numeroIntentos)
            {
                rutasDireccion = GMapProviders.GoogleMap.GetDirections(out direccion, puntoInicio, puntoFinal, false, false, true, false, false);
                c++;
            }

            return direccion; //puede ser null
        }
    }
}