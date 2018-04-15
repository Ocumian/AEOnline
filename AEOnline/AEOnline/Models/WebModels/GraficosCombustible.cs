using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AEOnline.Models.WebModels
{
    public class GraficosCombustible
    {
        public int Year { get; set; }
        public List<HistorialCargaCombustible> Historiales { get; set; }
        public List<Proveedor> Proveedores { get; set; }
        public List<Operador> Operadores { get; set; }
        public List<Auto> Vehiculos { get; set; }

    }
}