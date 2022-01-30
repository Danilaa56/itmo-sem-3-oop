using System.ComponentModel.DataAnnotations;

namespace Reports.WebApp.Models.Auth
{
    public class LoginModel
    {
        [DataType(DataType.Text)]
        public string Username { get; set; } = null!;
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}