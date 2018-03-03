using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace AEOnline.Models
{
    [Table("GrupoFlota")]
    public class GrupoFlota
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public virtual List<Auto> Autos { get; set; }

    }
}