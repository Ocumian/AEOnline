using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AEOnline.Models.WebModels
{
    public class HistorialWeb
    {
        public enum TiposHistorial { Velocidad, Posicion, Energia }

        public int IdAuto { get; set; }
        public string PatenteAuto { get; set; }

        public DateTime FechaMostrar { get; set; }

        public double ValorMaximo { get; set; }

        public List<PuntoGrafico> historialesVelocidad { get; set; }
        public List<HistorialPosicion> historialesPosicion { get; set; }
        public List<PuntoGrafico> historialesEnergia { get; set; }

        public List<TiposHistorial> historialesComparables { get; set; }

        public static List<TiposHistorial> ObtenerTiposHistorial()
        {
            List<TiposHistorial> tiposValidos = new List<TiposHistorial>();
            tiposValidos.Add(TiposHistorial.Velocidad);
            tiposValidos.Add(TiposHistorial.Posicion);
            //tiposValidos.Add(TiposHistorial.Energia);


            return tiposValidos;
        }

    }
}