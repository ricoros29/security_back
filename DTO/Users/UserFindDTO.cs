using Seguridad_API.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Seguridad_API.DTO.Users
{
    public class UserFindDTO : IUser
    {
        public int IdUsuario { get; set; }
        public string? Nombre { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Cuenta { get; set; }
        public int? IdEstado { get; set; }
        public short? IdDependenciaOrigen { get; set; }
        public string? NoEmpleado { get; set; }
        public string? Rfc { get; set; }
        public string? Cargo { get; set; }
        public string? UnidadAdministrativa { get; set; }
        public string? CorreoElectronico { get; set; }
        public byte? IdModulo { get; set; }
        public int? IdRol { get; set; }

        public string? Estado { get; set; }
        public string? DependenciaOrigen { get; set; }
        public string? Modulo { get; set; }
        public string? Rol { get; set; }
        public bool IdEstatus { get; set; }
        public string? Estatus { get; set; }
    }
}
