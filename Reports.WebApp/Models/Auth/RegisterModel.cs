using System.ComponentModel.DataAnnotations;

namespace Reports.WebApp.Models.Auth
{
    public class RegisterModel
    {
        [DataType(DataType.Text)]
        public string Username { get; set; } = null!;
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; } = null!;
        [DataType(DataType.Text)]
        public string Name { get; set; } = null!;
        [DataType(DataType.Text)]
        public string Surname { get; set; } = null!;
        public string DirectorId { get; set; } = null!;
    }
}