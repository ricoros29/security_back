using Seguridad_API.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Seguridad_API.DTO.Users
{
    public class UserEditDTO : IUser
    {
        [Required(ErrorMessage = "{0} es obligatorio.")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        [MaxLength(50, ErrorMessage = "Ingresar máximo 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten caracteres alfabéticos.")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        [MaxLength(50, ErrorMessage = "Ingresar máximo 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten caracteres alfabéticos.")]
        public string? ApellidoPaterno { get; set; }

        [MaxLength(50, ErrorMessage = "Ingresar máximo 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten caracteres alfabéticos.")]
        public string? ApellidoMaterno { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        [RegularExpression(@"^[a-zA-z_0-9]+$", ErrorMessage = "Solo se permiten caracteres alfanuméricos.")]
        public string? Cuenta { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        public int? IdEstado { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        public short? IdDependenciaOrigen { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Solo se permiten caracteres numéricos.")]
        public string? NoEmpleado { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        [RegularExpression(@"^[a-zA-z0-9]+$", ErrorMessage = "Solo se permiten caracteres alfanuméricos.")]
        public string? Rfc { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        [MaxLength(150, ErrorMessage = "Ingresar máximo 150 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten caracteres alfabéticos.")]
        public string? Cargo { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        [MaxLength(150, ErrorMessage = "Ingresar máximo 150 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Solo se permiten caracteres alfabéticos.")]
        public string? UnidadAdministrativa { get; set; }


        [Required(ErrorMessage = "{0} es obligatorio.")]
        [MaxLength(50, ErrorMessage = "Ingresar máximo 50 caracteres.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "El formato del correo no es válido.")]
        public string? CorreoElectronico { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        public byte? IdModulo { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        public int? IdRol { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        public bool IdEstatus { get; set; }
    }
}
