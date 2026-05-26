using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using project_2.Models;

namespace project_2.ViewModels
{
    public class AppointmentViewModel
    {
        public int AppointmentId {  get; set; }

        public int ScheduleId { get; set; }
        [ValidateNever]
        public List<SelectListItem> AvailableDates { get; set; }

        public String Subject { get; set; }
        [ValidateNever]
        public List<SelectListItem> SubjectList {  get; set; }
        public String PatientName { get; set; }
        [RegularExpression(@"^07[789]\d{7}$", ErrorMessage = "Invalid phone number")]
        public String PhoneNumber { get; set; }
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@gmail\.com$", ErrorMessage = "Invalid email")]
        public String Email { get; set; }
        public string SlotStartTimeString { get; set; }
        public TimeSpan SlotStartTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        [ValidateNever]
        public List<SelectListItem> AvailableSlots { get; set; }
        public bool ReminderFlag { get; set; }
    }
}
