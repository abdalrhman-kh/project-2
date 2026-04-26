using System.ComponentModel.DataAnnotations;

namespace project_2.Areas.Admin.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
