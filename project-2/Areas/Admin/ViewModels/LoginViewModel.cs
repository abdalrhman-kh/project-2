using System.ComponentModel.DataAnnotations;

namespace project_2.Areas.Admin.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password must have a number and be 8 characters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
