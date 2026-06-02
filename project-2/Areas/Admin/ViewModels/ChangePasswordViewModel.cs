using System.ComponentModel.DataAnnotations;

namespace project_2.Areas.Admin.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required(ErrorMessage ="This field is required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password does not match")]
        public string NewPasswordConfirmation { get; set; }
    }
}
