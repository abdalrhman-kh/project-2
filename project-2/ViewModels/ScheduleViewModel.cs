using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace project_2.ViewModels
{
    public class ScheduleViewModel
    {
        public int ScheduleId { get; set; }
        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        public DateOnly? Date { get; set; }
        [Required(ErrorMessage = "Start Time is required")]
        public TimeOnly? StartTime { get; set; }
        [Required(ErrorMessage = "End Time is required")]
        public TimeOnly? EndTime { get; set; }
        [Required(ErrorMessage = "Status is required")]
        public bool Status { get; set; }
        public List<SelectListItem>? StatusList { get; set; }
    }
}
