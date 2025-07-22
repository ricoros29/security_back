using System.ComponentModel.DataAnnotations;

namespace Seguridad_API.DTO.Security
{
    public class CredentialsDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
