using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;

namespace Invoice.Client.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Informar {0}")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Display(Name = "Lembrar-se")]
        public bool RememberMe { get; set; }
    }
}