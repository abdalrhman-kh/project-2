using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace project_2.Areas.Admin.ViewModels
{
    public class AddAppointmentViewModel
    {
        public int AppointmentId { get; set; }

        public int ScheduleId { get; set; }
        [ValidateNever]
        public List<SelectListItem> AvailableDates { get; set; }

        public String Subject { get; set; }
        [ValidateNever]
        public List<SelectListItem> SubjectList { get; set; }
        public String PatientName { get; set; }
        public string SlotStartTimeString { get; set; }
        public TimeSpan SlotStartTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        [ValidateNever]
        public List<SelectListItem> AvailableSlots { get; set; }
        public bool Status { get; set; }
    }
}
