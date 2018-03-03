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
    [Table("HistorialIncidente")]
    public class HistorialIncidente
    {
        [Key]
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

    }
}