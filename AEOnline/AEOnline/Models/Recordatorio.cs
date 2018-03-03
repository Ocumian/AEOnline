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
    [Table("Recordatorio")]
    public class Recordatorio
    {
        [Key]
        public int Id { get; set; }

        //auto tine lista
        //no seguro si asociarlo a algo
    }
}