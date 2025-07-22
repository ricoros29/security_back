namespace Seguridad_API.DTO.Security
{
    public class ResultLoginDTO
    {
        public string Token { get; set; } = string.Empty;
        public DateTime? Expires { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
