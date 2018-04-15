using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AEOnline.Models.WebModels
{
    public class CreacionUsuario
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener un mínimo de 3 carácteres y máximo de 25")]
        public string Nombre { get; set; }

        [Required, StringLength(50, MinimumLength = 3, ErrorMessage = "El email debe tener un mínimo de 3 carácteres y un máximo de 50")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public Usuario.RolUsuario Rol { get; set; }


        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "La contraseña no coincide, intenta de nuevo.")]
        public string ConfirmPassword { get; set; }

        public int AutoId { get; set; }
        public string AutoPatente { get; set; }
        public string AutoNombre { get; set; }

        public int FlotaId { get; set; }
        public string FlotaNombre { get; set; }
        public DateTime Fecha { get; set; }

    }
}