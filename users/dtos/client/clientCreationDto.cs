using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace project.users.dto
{
    public class clientCreationDto
    {
        public string? userName { get; set; }

        [Required(ErrorMessage = "El campo Contraseña es requerido")]
        public string? password { get; set; }

        [Required(ErrorMessage = "El campo Correo es requerido")]
        public string email { get; set; } = null!;


    }
}