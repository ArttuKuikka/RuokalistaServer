using System.ComponentModel.DataAnnotations;

namespace RuokalistaServer.Auth
{
    public class LoginModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
