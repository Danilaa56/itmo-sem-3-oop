using System.ComponentModel.DataAnnotations;

namespace Reports.WebApp.Models.Auth
{
    public class RegisterModel
    {
        [DataType(DataType.Text)]
        public string Username { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
        [DataType(DataType.Text)]
        public string Name { get; set; }
        [DataType(DataType.Text)]
        public string Surname { get; set; }
        public string DirectorId { get; set; }
    }
}