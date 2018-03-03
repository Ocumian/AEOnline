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
    [Table("UsuarioFlota")]
    public class UsuarioFlota
    {

        [Key]
        public int Id { get; set; }

        //public int UsuarioId { get; set; }
        //public int FlotaId { get; set; }

        //[ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }
        //[ForeignKey("FlotaId")]
        public virtual Flota Flota { get; set; }
    }
}