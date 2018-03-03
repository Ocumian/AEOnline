using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AEOnline.Models.WebModels
{
    public class CreacionFlota
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(25, MinimumLength = 3, ErrorMessage = "El nombre debe tener un mínimo de 3 carácteres y máximo de 25"),
            Index("NombreIndex", IsUnique = true)]
        public string Nombre { get; set; }


        public virtual List<Usuario> Miembros { get; set; }

        public int AdminId { get; set; }
        public string AdminNombre { get; set; }

        public string MiembroEmail { get; set; }
        public string AutoPatente { get; set; }
    }
}