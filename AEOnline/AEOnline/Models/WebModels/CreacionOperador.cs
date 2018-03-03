using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace AEOnline.Models.WebModels
{
    public class CreacionOperador
    {

        [Key]
        public int Id { get; set; }

        [Required, StringLength(25, MinimumLength = 3, ErrorMessage = "El nombre del operario debe tener un mínimo de 3 carácteres y máximo de 25")]
        public string Nombre { get; set; }
        public string TipoLicencia { get; set; }

        public int UsuarioId { get; set; }
        public int AutoId { get; set; }
        public int FlotaId { get; set; }
    }
}