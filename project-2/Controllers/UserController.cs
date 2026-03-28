using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using project_2.Extenstions;
using project_2.Helpers.Email;
using project_2.Helpers.OTPHelper;
using project_2.Models;
using project_2.Models.Repositories;
using project_2.ViewModels;

namespace project_2.Controllers
{
    public class UserController : Controller
    {
        IRepository<Appointment> repo;
        IRepository<Schedule> repoSchedule;
        IEmailSender emailSender;
        IOTPHelper otphelper;

        public UserController(IRepository<Appointment> repo, IRepository<Schedule> repoSchedule, IEmailSender emailSender, IOTPHelper otphelper)
        {
            this.repo = repo;
            this.repoSchedule = repoSchedule;
            this.emailSender = emailSender;
            this.otphelper = otphelper;
        }

        private List<SelectListItem> GenerateSlots(
        TimeSpan start,
        TimeSpan end,
        int slotMinutes = 60)
        {
            var slots = new List<SelectListItem>();
            var current = start;

            while (current.Add(TimeSpan.FromMinutes(slotMinutes)) <= end)
            {
                var label = $"{current:hh\\:mm} - {current.Add(TimeSpan.FromMinutes(slotMinutes)):hh\\:mm}";
                slots.Add(new SelectListItem
                {
                    Text = label,
                    Value = current.ToString(@"hh\:mm")
                });

                current = current.Add(TimeSpan.FromMinutes(slotMinutes));
            }

            return slots;
        }

        // GET: UserController
        public ActionResult Index()
        {
            var data = new AppointmentViewModel
            {
                SubjectList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "-- Chose --", Value = "-- Chose --" },
                    new SelectListItem { Text = "Dental Check-up", Value = "Dental Check-up" },
                    new SelectListItem { Text = "Teeth Cleaning", Value = "Teeth Cleaning" },
                    new SelectListItem { Text = "Tooth Filling", Value = "Tooth Filling" },
                    new SelectListItem { Text = "Tooth Extraction", Value = "Tooth Extraction" },
                    new SelectListItem { Text = "Root Canal Treatment", Value = "Root Canal" },
                    new SelectListItem { Text = "Dental Pain", Value = "Dental Pain" },
                    new SelectListItem { Text = "Consultation", Value = "Consultation" },
                    new SelectListItem { Text = "Cosmetic Dentistry", Value = "Cosmetic" },
                    new SelectListItem { Text = "Follow-up Appointment", Value = "Follow-up" },
                    new SelectListItem { Text = "Other", Value = "Other" }
                },
                AvailableSlots = new List<SelectListItem>(),
            };
            var Scheduler = repoSchedule.GetAll().Where(s => s.Status).Select(s => new
            {
                id = s.Id,
                date = s.Date.ToString("yyyy-MM-dd")
            }).ToList();
            ViewBag.ScheduleDates = Scheduler;
            return View(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AppointmentViewModel collection)
        {
            try
            {
                var isTaken = repo.GetAll().Any(x=>x.ScheduleId == collection.ScheduleId && x.SlotStartTime == collection.SlotStartTime && x.Status == collection.Status);
                if (isTaken) {
                    TempData["ErrorMessage"] = "This time slot had already been taken, please select another.";
                }
                repo.Add(collection.ToModel());
                TempData["SuccessMessage"] = "Your appointment has been booked successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while booking the appointment.";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public IActionResult GetSlots(int scheduleId)
        {
            var schedule = repoSchedule.Get(scheduleId);
            if (schedule == null)
                return NotFound();

            var booked = repo.GetAll().Where(x=>x.ScheduleId == scheduleId).Select(x=>x.SlotStartTime).ToList();

            var slots = GenerateSlots(schedule.StartTime, schedule.EndTime).Where(x=> !booked.Contains(TimeSpan.Parse(x.Value))).ToList();

            return Json(slots);
        }
        [HttpPost]
        public async Task <IActionResult> RequestOTP([FromBody] OTPRequestViewModel model)
        {
            if (String.IsNullOrEmpty(model.Email))
            {
                return BadRequest("Email is required");
            }
            try
            {
                var OtpCode = await otphelper.GenerateOTP(model.Email);
                string subject = "Access Code";
                string body = $"Your verification code is: {OtpCode}";
                
                await emailSender.SendEmailAsync(model.Email, subject, body);
                return Ok(new { message = "A verification code has been sent to your email." });
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> VerifyOTP([FromBody] OTPVerifyViewModel model)
        {
            if (String.IsNullOrEmpty(model.Code)) { 
                return BadRequest("Invalid code");
            }
            try {
                var valid = await otphelper.VerifyOTP(model.Email, model.Code);
                if (!valid) { 
                    return BadRequest("Wrong code please try again");
                }
                else
                {
                    if (String.IsNullOrEmpty(model.Email))
                        return BadRequest("Email and phone number are required");
                    var Appointments = repo.GetAll().Where(x => x.Email == model.Email).ToList();
                    if (Appointments.Count == 0)
                        return BadRequest("No appointments found");
                    List<AppointmentViewModel> AppointmentList = new List<AppointmentViewModel>();
                    foreach (var item in Appointments)
                    {
                        var obj = new AppointmentViewModel
                        {
                            Subject = item.Subject,
                            PatientName = item.PatientName,
                            StartTime = repoSchedule.Get(item.ScheduleId).Date.ToDateTime(TimeOnly.FromTimeSpan(item.SlotStartTime)),
                            EndTime = repoSchedule.Get(item.ScheduleId).Date.ToDateTime(TimeOnly.FromTimeSpan(item.SlotStartTime).AddMinutes(15)),
                        };
                        AppointmentList.Add(obj);
                    }
                    return Json(AppointmentList);
                }
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        //[HttpGet]
        //public async Task<IActionResult> GetMyAppointments(String email)
        //{
        //    if (String.IsNullOrEmpty(email))
        //        return BadRequest("Email and phone number are required");
        //    var Appointments = repo.GetAll().Where(x => x.Email == email ).ToList();
        //    if(Appointments.Count == 0)
        //        return BadRequest("No appointments found");
        //    List<AppointmentViewModel> AppointmentList = new List<AppointmentViewModel>();
        //    foreach(var item in Appointments)
        //    {
        //        var obj = new AppointmentViewModel
        //        {
        //            Subject = item.Subject,
        //            PatientName = item.PatientName,
        //            StartTime = repoSchedule.Get(item.ScheduleId).Date.ToDateTime(TimeOnly.FromTimeSpan(item.SlotStartTime)),
        //            EndTime = repoSchedule.Get(item.ScheduleId).Date.ToDateTime(TimeOnly.FromTimeSpan(item.SlotStartTime).AddHours(1)),
        //        };
        //        AppointmentList.Add(obj);
        //    }
        //    return Json(AppointmentList);
        //}
    }
}
