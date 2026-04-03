using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace project_2.ViewModels
{
    public class DentistViewModel
    {
        [Key]
        public int DentistId { get; set; }
        public String Name { get; set; }
        public string Experience { get; set; }
        public String Collage { get; set; }
        [RegularExpression(@"^07[789]\d{7}$", ErrorMessage = "Invalid phone number")]
        public String PhoneNumber { get; set; }
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@gmail\.com$", ErrorMessage = "Invalid email")]
        public String Email { get; set; }
        [ValidateNever]
        public String DentistImage { get; set; }
        public IFormFile? File { get; set; }
    }
}
