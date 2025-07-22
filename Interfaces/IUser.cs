namespace Seguridad_API.Interfaces
{
    public interface IUser
    {
        int IdUsuario { get; set; }
        string? Nombre { get; set; }
        string? ApellidoPaterno { get; set; }
        string? ApellidoMaterno { get; set; }
        string? Cuenta { get; set; }
        int? IdEstado { get; set; }
        short? IdDependenciaOrigen { get; set; }
        string? NoEmpleado { get; set; }
        string? Rfc { get; set; }
        string? Cargo { get; set; }
        string? UnidadAdministrativa { get; set; }
        string? CorreoElectronico { get; set; }
        byte? IdModulo { get; set; }
        int? IdRol { get; set; }
    }
}
