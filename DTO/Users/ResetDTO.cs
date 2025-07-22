using System.ComponentModel.DataAnnotations;

namespace Seguridad_API.DTO.Users
{
    public class ResetDTO
    {
        [Required(ErrorMessage = "{0} es obligatorio.")]
        public int IdUsuario { get; set; }
        [Required(ErrorMessage = "{0} es obligatorio.")]
        public string Cuenta { get; set; } = string.Empty;
        [Required(ErrorMessage = "{0} es obligatorio.")]
        //[RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0 - 9])$", ErrorMessage = "Debe contener al menos 8 caracteres, un dígito, una letra mayúscula y una letra minúscula.")]
        //^(?=.*[A-Z].*[A-Z])(?=.*[!@#$&*])(?=.*[0-9].*[0-9])(?=.*[a-z].*[a-z].*[a-z]).{8}$
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "{0} es obligatorio.")]
        //[RegularExpression(@"(?=.*[A-Z])(?=.*[a-z])(?=.*[0 - 9])", ErrorMessage = "Debe contener al menos 8 caracteres, un dígito, una letra mayúscula y una letra minúscula.")]
        public string PasswordConfirm { get; set; } = string.Empty;

    }
}
