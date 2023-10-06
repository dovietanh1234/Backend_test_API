using System.ComponentModel.DataAnnotations;

namespace WEB_API.Models.Login
{
    public class LoginModel
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
