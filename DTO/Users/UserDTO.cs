using Seguridad_API.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Seguridad_API.DTO.Users
{
    public class UserDTO : IUser, ICloneable
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "{0} es obligatorio.")]
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        [Required(ErrorMessage = "{0} es obligatorio.")]
        public string? Cuenta { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        public int? IdEstado { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        public short? IdDependenciaOrigen { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        public string? NoEmpleado { get; set; }
        [Required(ErrorMessage = "{0} es obligatorio.")]
        public string? Rfc { get; set; }
        [Required(ErrorMessage = "{0} es obligatorio.")]
        public string? Cargo { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        public string? UnidadAdministrativa { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        public string? CorreoElectronico { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        public byte? IdModulo { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio.")]
        public int? IdRol { get; set; }


        [Required(ErrorMessage = "{0} es obligatorio.")]
        public string? Password { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
