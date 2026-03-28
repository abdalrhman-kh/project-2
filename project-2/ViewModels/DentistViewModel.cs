using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace project_2.ViewModels
{
    public class DentistViewModel
    {
        [Key]
        public int DentistId { get; set; }
        public String Name { get; set; }
        public String Experience { get; set; }
        public String Collage { get; set; }
        public String PhoneNumber { get; set; }
        public String DentistImage { get; set; }
        public IFormFile File { get; set; }
    }
}
